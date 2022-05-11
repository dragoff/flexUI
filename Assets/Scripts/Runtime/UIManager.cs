using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexUI.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlexUI
{
	/// <summary>UIManager controls opening and closing of views on scene</summary>
	[DefaultExecutionOrder(-1000)]
	[RequireComponent(typeof(SimpleGestures))]
	public class UIManager : Singleton<UIManager>
	{
		/// <summary> Play ShowAnimation when view already shown</summary>
		[SerializeField] private bool ReplayShowAnimation = false;

		/// <summary> Dynamically created Views will be destroyed after closing </summary>
		[SerializeField] private bool DestroyDynamicViews = true;

		[SerializeField] private AnimationLink ShowAnimation = default;
		[SerializeField] private AnimationLink HideAnimation = default;

		/// <summary> Prefab for Background Sounds </summary>
		[SerializeField] private AudioSource AudioSource = default;

		[SerializeField] private AudioClip ShowSound = default;
		[SerializeField] private AudioClip HideSound = default;
		[SerializeField] private AudioClip ButtonSound = default;
		[SerializeField] private AudioClip SwipeSound = default;

		[SerializeField] private float DefaultVolume = 1f;
		[SerializeField] private float FadeVolume = 0.25f;
		[SerializeField] private float VolumeChangeSpeed = 1f;

		public static Transform Transform => Instance.transform;
		public static Dictionary<string, BaseView> StaticViews { get; } = new Dictionary<string, BaseView>();

		public static Dragger Dragger = default;
		public static SimpleGestures Gestures = default;

		public SoundController SoundController { get; private set; } = default;

		protected override void OnAwake()
		{
			if (AudioSource == null && !TryGetComponent(out AudioSource))
			{
				AudioSource = gameObject.AddComponent<AudioSource>();
				AudioSource.volume = DefaultVolume;
			}

			StaticViews.Clear();

			Dragger = new Dragger();
			SoundController = new SoundController(this, DefaultVolume, FadeVolume, VolumeChangeSpeed);

			ViewShown += SoundController.OnViewShown;
			ViewHidden += SoundController.ViewHidden;

			InitGestures();

			SceneManager.sceneLoaded += (x, y) => InitViewsOnScene();
		}

		private void InitViewsOnScene()
		{
			//find all views
			var views = SceneInfoGrabber<BaseView>
				.GetUIComponentsOnScene()
				.ToList();

			//grab components for views
			foreach (var view in views)
			{
				view.GrabComponents();
				StaticViews[view.GetType().Name] = view;
			}

			//grab views for views
			foreach (var view in views)
			{
				view.GrabViews(StaticViews);
			}

			//init views
			foreach (var view in views)
				view.Initialize();

			//show views
			foreach (var view in views.Where(v => v.IsShowAtStart))
				Show(view, view.Owner, noAnimation: true);
		}

		#region Show/Hide/Back

		/// <summary>Show the view</summary>
		public static void Show(BaseView view, BaseView owner, Action animDoneCallback = null, bool noAnimation = false,
			bool concurrentAnimation = false)
		{
			if (owner == view)
			{
				Debug.LogError("<color=#ffff00ff>[FlexUI]</color> Owner cant be same view, set to NULL");
				owner = null;
			}

			//add view to list of children of owner
			AddOpenedChild(owner, view);

			//already opened?
			if (view.VisibleState == VisibleState.Visible && !Instance.ReplayShowAnimation)
			{
				animDoneCallback?.Invoke();
				return;
			}

			//hide neighbors
			if (view.IsHideNeighborsOnShow)
				GetConcurrentList(view)
					.Where(x => x != view && x.VisibleState != VisibleState.Hidden)
					.ForEach(x => Hide(x));

			//set visible state
			view.gameObject.SetActive(true);
			//Unity BUG: Layout wont recalculate without forcing, if a parent has layoutGroup...
			if (view.transform.parent is RectTransform rcParent)
				LayoutRebuilder.ForceRebuildLayoutImmediate(rcParent);

			OnViewShown(view);

			//get show animation
			var anim = view.Animations.IsOverrideAnimation ? view.Animations.ShowAnimation : Instance.ShowAnimation;

			//play show animation
			if (anim == null || anim.Animation == null || noAnimation)
				animDoneCallback?.Invoke();
			else
			{
				if (!concurrentAnimation)
					view.CurrentAnimation?.CurrentAnimation?.StopAndResetTransform();
				view.CurrentAnimation = AnimationPlayer.Play(view.RectTransform, anim.Animation, animDoneCallback, true, 1, 1);
			}
		}

		/// <summary>Hide the view and all children views</summary>
		public static void Hide(BaseView view, Action animationDoneCallback = null, bool isNoAnimation = false,
			bool isNeighborsAnimate = false)
		{
			//already hidden?
			if (view.VisibleState == VisibleState.Hidden)
			{
				// view.gameObject.SetActive(false);
				animationDoneCallback?.Invoke();
				ClearOwner(view);
				return;
			}

			//hide children w/o animation
			if (view.IsHideChildrenOnHide)
				HideAllChildren(view);

			OnViewHidden(view);

			//get hide animation
			var anim = view.Animations.IsOverrideAnimation ? view.Animations.HideAnimation : Instance.HideAnimation;

			if (isNoAnimation || anim == null || anim.Animation == null)
				AnimationDoneCallback();
			else
			{
				if (!isNeighborsAnimate)
					view.CurrentAnimation?.CurrentAnimation?.StopAndResetTransform();
				view.CurrentAnimation = AnimationPlayer.Play(view.RectTransform, anim.Animation, AnimationDoneCallback, true, 1, 1);
			}

			ClearOwner(view);

			//play animation and deactivate
			void AnimationDoneCallback()
			{
				if (view)
				{
					view.gameObject.SetActive(false);
					if (Instance.DestroyDynamicViews && view.IsDynamicallyCreated)
						Destroy(view.gameObject);
				}

				animationDoneCallback?.Invoke();
			}
		}

		/// <summary>Hides all children</summary>
		public static void HideAllChildren(BaseView view)
		{
			foreach (var child in view.OpenedChildren.ToArray())
				HideNoAnimation(child);
		}

		private static void ClearOwner(BaseView view)
		{
			//remove from opened children list
			view.Owner?.OpenedChildren.Remove(view);
		}

		private static void HideNoAnimation(BaseView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

			if (view.VisibleState == VisibleState.Hidden)
				return;

			foreach (var child in view.OpenedChildren.ToArray())
				HideNoAnimation(child);

			view.gameObject.SetActive(false);

			OnViewHidden(view);

			// FIXME Above
			if (Instance.DestroyDynamicViews && view.IsDynamicallyCreated)
				Destroy(view.gameObject);

			//remove from owner list
			view.Owner?.OpenedChildren.Remove(view);
		}

		private static IEnumerable<BaseView> GetConcurrentList(BaseView view)
		{
			var parent = view.RectTransform.parent;
			return parent.OfType<RectTransform>().Select(rt => rt.GetComponent<BaseView>()).Where(v => v != null);
		}

		#endregion

		#region Events

		public static event Action<BaseView> ViewShown;
		public static event Action<BaseView> ViewHidden;
		public static event Action<BaseView, VisibleState, VisibleState> ViewVisibleStateChanged;

		public static readonly List<BaseView> ShownViews = new List<BaseView>();

		private static void OnViewShown(BaseView view)
		{
			var prevState = view.VisibleState;

			(view as IBaseViewInternal).SetVisibleState(VisibleState.Visible);

			ViewShown?.Invoke(view);
			ViewVisibleStateChanged?.Invoke(view, prevState, view.VisibleState);
			if (!ShownViews.Contains(view))
				ShownViews.Add(view);
		}

		private static void OnViewHidden(BaseView view)
		{
			var prevState = view.VisibleState;

			(view as IBaseViewInternal).SetVisibleState(VisibleState.Hidden);

			ViewHidden?.Invoke(view);
			ViewVisibleStateChanged?.Invoke(view, prevState, view.VisibleState);
			ShownViews.Remove(view);
		}

		#endregion

		#region ShowCoroutine, ShowAsync

		/// <summary>Shows the view and wait while it will be hidden</summary>
		public static IEnumerator ShowCoroutine(BaseView view, BaseView owner, Action animationDoneCallback = null,
			bool isNoAnimation = false,
			bool isHideChildren = false)
		{
			Show(view, owner, animationDoneCallback, isNoAnimation, isHideChildren);
			while (view.VisibleState != VisibleState.Hidden)
				yield return null;
		}

		/// <summary>Shows the view and wait while it will be hidden</summary>
		public static async Task ShowAsync(BaseView view, BaseView owner, Action animationDoneCallback = null, bool isNoAnimation = false,
			bool isHideChildren = false)
		{
			Show(view, owner, animationDoneCallback, isNoAnimation, isHideChildren);

			await Extensions.WaitUntil(() => view.VisibleState != VisibleState.Hidden);
		}

		#endregion

		#region Gestures

		private void InitGestures()
		{
			//init gestures
			TryGetComponent(out Gestures);
			Gestures.onSwipeHoriz += (i) => ProcessGesture(i < 0 ? Gesture.SwipeLeft : Gesture.SwipeRight);
			Gestures.onSwipeVert += (i) => ProcessGesture(i < 0 ? Gesture.SwipeDown : Gesture.SwipeUp);
			Gestures.onSwipeFromHoriz += (i, p) =>
			{
				if (i > 0 && p < 0.1f * Screen.width)
				{
					ProcessGesture(Gesture.SwipeFromLeft);
				}
				else if (i > 0 && p < 0.9f * Screen.width)
				{
					ProcessGesture(Gesture.SwipeFromRight);
				}
			};
			Gestures.onSwipeFromVert += (i, p) =>
			{
				if (i > 0 && p < 0.1f * Screen.height)
				{
					ProcessGesture(Gesture.SwipeFromUp);
				}
				else if (i > 0 && p < 0.9f * Screen.height)
				{
					ProcessGesture(Gesture.SwipeFromDown);
				}
			};
			Gestures.onTap += (i) => ProcessGesture(Gesture.Tap);
			Gestures.onLongTap += (i) => ProcessGesture(Gesture.LongTap);
			Gestures.onDoubleTap += (i) => ProcessGesture(Gesture.DoubleTap);
			Gestures.onPan += (v) =>
			{
				if (Dragger.IsDragging) Dragger.OnDragging(v);
			};
			Gestures.onDragStart += (v) => Dragger.OnDragStart(v, Gestures.LastTouchedUI);
			Gestures.onDragEnd += Dragger.OnDragEnd;
		}

		private void ProcessGesture(Gesture gesture)
		{
			if (Dragger.IsDragging)
				return; //we are in drag mode => ignore gestures

			//get last touched UI
			var ui = Gestures.LastTouchedUI;
			if (ui == null)
			{
				DefaultGestureProcessing(gesture);
				return;
			}

			//find touched BaseView
			var view = ui.GetComponentsInParent<BaseView>().FirstOrDefault(v => v.VisibleState != VisibleState.Hidden);

			//try process gesture in touched view and it's owners
			var info = new GestureInfo(gesture, view);
			while (view != null)
			{
				//call method of view
				view.Gesture(info);
				if (info.IsHandled || view.SuppressAnyGesturesForOwners)
					break; //gesture is handled
				//go to owner
				view = view.Owner;
			}

			//gesture is not handled => try process Back
			DefaultGestureProcessing(gesture);
		}

		private void DefaultGestureProcessing(Gesture gesture)
		{
			if ((int)gesture >= 8)
			{
				PlayOneShotSound(SwipeSound, 0.6f);
			}
		}

		#endregion

		#region Sounds

		public static void PlayButtonSound(BaseView view)
		{
			if (Instance.ButtonSound)
				PlayOneShotSound(Instance.ButtonSound, 0.6f);
		}

		public static void PlayOneShotSound(AudioClip clip, float volume = 1f)
		{
			if (clip)
				Instance.AudioSource.PlayOneShot(clip, volume);
		}

		public static AudioClip GetShowSound() => Instance.ShowSound;
		public static AudioClip GetHideSound() => Instance.HideSound;

		#endregion

		#region Utils

		private static void AddOpenedChild(BaseView owner, BaseView child)
		{
			//hide prev owner 
			if (child.Owner != owner)
				ClearOwner(child);

			child.Owner = owner;

			if (owner == null)
				return;

			if (!owner.OpenedChildren.Contains(child))
				owner.OpenedChildren.Add(child);
		}

		public static IEnumerable<BaseView> GetViewsUnderMouse()
		{
			var uis = SimpleGestures.GetUIObjectsUnderPosition(Input.mousePosition).Select(r => r.gameObject);
			foreach (var ui in uis)
			{
				var view = ui.GetComponentsInParent<BaseView>().FirstOrDefault(v => v.VisibleState != VisibleState.Hidden);
				while (view != null)
				{
					yield return view;
					view = view.Owner;
				}
			}
		}

		#endregion

#if UNITY_EDITOR
		private void Reset()
		{
			name = "UI Manager";
			if (TryGetComponent(out Canvas _))
				name += " [Canvas]";
		}
#endif
	}

	public enum Gesture
	{
		None = 0,
		Tap,
		LongTap,
		DoubleTap,
		SwipeFromLeft = 4,
		SwipeFromRight,
		SwipeFromUp,
		SwipeFromDown,
		SwipeLeft = 8,
		SwipeRight,
		SwipeUp,
		SwipeDown,
	}

	public class GestureInfo
	{
		/// <summary>Touched BaseView</summary>
		public BaseView TouchedUI { get; }

		/// <summary>Gesture type</summary>
		public Gesture Gesture { get; }

		/// <summary>Set to True to prevent owner to process gesture</summary>
		public bool IsHandled { get; set; }

		public GestureInfo(Gesture gesture, BaseView touchedView)
		{
			Gesture = gesture;
			TouchedUI = touchedView;
		}
	}
}