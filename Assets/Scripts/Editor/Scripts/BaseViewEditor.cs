using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlexUI.Editor
{
	[CustomEditor(typeof(BaseView), true)]
	public class BaseViewEditor : UnityEditor.Editor
	{
		private BaseView baseView;
		private Texture2D logo;
		private bool isBuilt;
		private bool isPlayMode;

		static bool isBuildBaseClass = false;
		static bool isAddBaseMethods = false;
		static bool isAddGestureMethods = false;
		static bool isAddDragDropMethods = false;

		private SerializedProperty isOverrideAnimation;

		private SerializedProperty animations;
		private SerializedProperty showAnimation;
		private SerializedProperty hideAnimation;

		private SerializedProperty sounds;
		private SerializedProperty isSoundEnabled;
		private SerializedProperty playButtonClickSound;
		private SerializedProperty ownerMode;
		private SerializedProperty backgroundSound;
		private SerializedProperty isOverrideSounds;
		private SerializedProperty showSound;
		private SerializedProperty hideSound;

		private SerializedProperty showAtStart;
		private SerializedProperty hideNeighborsOnShow;
		private SerializedProperty hideChildrenOnHide;
		private SerializedProperty isDynamicallyCreated;

		private SerializedProperty isDragMode;
		private SerializedProperty dragMode;
		private SerializedProperty isButtonsCallOnChanged;
		private SerializedProperty suppressAnyGesturesForOwners;

		protected void OnEnable()
		{
			baseView = target as BaseView;
			isBuilt = Directory.GetFiles(Application.dataPath, $"{target.GetType().Name}.cs", SearchOption.AllDirectories).Length > 1;
			isPlayMode = Application.isPlaying;

			animations = serializedObject.FindProperty(nameof(baseView.Animations));
			isOverrideAnimation = animations.FindPropertyRelative(nameof(baseView.Animations.IsOverrideAnimation));
			showAnimation = animations.FindPropertyRelative(nameof(baseView.Animations.ShowAnimation));
			hideAnimation = animations.FindPropertyRelative(nameof(baseView.Animations.HideAnimation));

			sounds = serializedObject.FindProperty(nameof(baseView.Sounds));
			isSoundEnabled = sounds.FindPropertyRelative(nameof(baseView.Sounds.IsSoundEnabled));
			playButtonClickSound = sounds.FindPropertyRelative(nameof(baseView.Sounds.PlayButtonClickSound));
			ownerMode = sounds.FindPropertyRelative(nameof(baseView.Sounds.OwnerMode));
			backgroundSound = sounds.FindPropertyRelative(nameof(baseView.Sounds.BackgroundSound));
			isOverrideSounds = sounds.FindPropertyRelative(nameof(baseView.Sounds.IsOverrideSounds));
			showSound = sounds.FindPropertyRelative(nameof(baseView.Sounds.ShowSound));
			hideSound = sounds.FindPropertyRelative(nameof(baseView.Sounds.HideSound));

			showAtStart = serializedObject.FindProperty(nameof(baseView.IsShowAtStart));
			hideNeighborsOnShow = serializedObject.FindProperty(nameof(baseView.IsHideNeighborsOnShow));
			hideChildrenOnHide = serializedObject.FindProperty(nameof(baseView.IsHideChildrenOnHide));
			isDynamicallyCreated = serializedObject.FindProperty(nameof(baseView.IsDynamicallyCreated));
			isDragMode = serializedObject.FindProperty(nameof(baseView.IsDragMode));
			dragMode = serializedObject.FindProperty(nameof(baseView.DragMode));
			isButtonsCallOnChanged = serializedObject.FindProperty(nameof(baseView.IsButtonsCallOnChanged));
			suppressAnyGesturesForOwners = serializedObject.FindProperty(nameof(baseView.SuppressAnyGesturesForOwners));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawHead();

			using (new EditorGUI.DisabledScope(true))
				EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

			if (isBuilt)
			{
				DrawBody();

				DrawPropertiesExcluding(serializedObject, "m_Script",
					nameof(baseView.Animations),
					nameof(baseView.Animations),
					nameof(baseView.Sounds),
					nameof(baseView.IsShowAtStart),
					nameof(baseView.IsHideNeighborsOnShow),
					nameof(baseView.IsHideChildrenOnHide),
					nameof(baseView.IsDynamicallyCreated),
					nameof(baseView.IsDragMode),
					nameof(baseView.DragMode),
					nameof(baseView.IsButtonsCallOnChanged),
					nameof(baseView.SuppressAnyGesturesForOwners)
				);
			}
			else
			{
				if (isBuildBaseClass)
				{
					isAddBaseMethods = FlexLayout.ToggleLabel(isAddBaseMethods, "Add base methods");
					isAddGestureMethods = FlexLayout.ToggleLabel(isAddGestureMethods, "Add gestures methods");
					isAddDragDropMethods = FlexLayout.ToggleLabel(isAddDragDropMethods, "Add Drag&Drop methods");
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawBody()
		{
			FlexLayout.ToggleLabel(showAtStart, "Show At Start", "Show the view at start of scene");
			FlexLayout.ToggleLabel(hideNeighborsOnShow, "Hide Neighbors On Show",
				"Hide opened neighbor views inside the parent when the view is showing");
			FlexLayout.ToggleLabel(hideChildrenOnHide, "Hide Children On Hide", "When it is closing - hide all children views");

			FlexLayout.Space();

			GUILayout.BeginHorizontal();
			{
				if (FlexLayout.ToggleLabel(isDragMode, "Drag mode", "Drag&Drop mode for the view"))
					FlexLayout.Property(dragMode);
			}
			GUILayout.EndHorizontal();

			FlexLayout.ToggleLabel(isButtonsCallOnChanged, "Buttons Call OnChanged event", "Button pressing causes OnChanged method");
			FlexLayout.ToggleLabel(suppressAnyGesturesForOwners, "Suppress Any Gestures For Owners", "Do not pass gestures to owners");
			FlexLayout.ToggleLabel(isDynamicallyCreated, "Dynamic View", "True if the view is manually created from prefab");

			if (FlexLayout.ToggleLabel(isOverrideAnimation, "Override Animation"))
			{
				GUILayout.BeginVertical(FlexLayout.Skin.box);
				{
					EditorGUILayout.PropertyField(showAnimation);
					EditorGUILayout.PropertyField(hideAnimation);
				}
				GUILayout.EndVertical();
			}

			if (FlexLayout.ToggleLabel(isSoundEnabled, "Use sounds"))
			{
				GUILayout.BeginVertical(FlexLayout.Skin.box);
				{
					EditorGUILayout.PropertyField(playButtonClickSound);
					EditorGUILayout.PropertyField(ownerMode);
					EditorGUILayout.PropertyField(backgroundSound);
					if (FlexLayout.ToggleLabel(isOverrideSounds, "Override Show/Hide Sounds"))
					{
						EditorGUILayout.PropertyField(showSound);
						EditorGUILayout.PropertyField(hideSound);
					}
				}
				GUILayout.EndVertical();
			}

			GUILayout.Label(new string('_', Screen.width));
		}

		private void DrawHead()
		{
			GUILayout.Label(GUIContent.none, FlexLayout.HEADER);

			GUILayout.BeginHorizontal();
			{
				if (!isPlayMode)
				{
					if (GUILayout.Button(
						new GUIContent($"{(isBuilt ? "Rebuild" : "Build")}", "Grab UI elements and build the script."),
						GUILayout.Width(70), GUILayout.Height(20)))
						RebuildAutoScript();

					if (isBuilt)
					{
						if (GUILayout.Button(
							new GUIContent("Assign", "Automatically find and assign UI elements to their fields if they are nulls."),
							GUILayout.Width(70), GUILayout.Height(20)))
							AssignControls();

						if (GUILayout.Button(
							new GUIContent("Reassign",
								"Automatically find and assign UI elements to their fields even if they are already assigned."),
							GUILayout.Width(70), GUILayout.Height(20)))
							AssignControls(true);

						GUILayout.FlexibleSpace();

						if (GUILayout.Button(
							new GUIContent("Remove", "Remove autogenerated script"),
							GUILayout.Width(70), GUILayout.Height(20)))
							RemoveAutoScript();
					}
					else
					{
						FlexLayout.Space();
						GUILayout.BeginVertical();
						FlexLayout.Space(4);
						isBuildBaseClass = FlexLayout.ToggleLabel(isBuildBaseClass, "Override this class with FlexUI template");
						GUILayout.EndVertical();
					}
				}
				else
				{
					if (GUILayout.Button(
						new GUIContent("Show", "Show the view"),
						GUILayout.Width(70), GUILayout.Height(20)))
						baseView.Show();
					if (GUILayout.Button(
						new GUIContent("Hide", "Hide the view "),
						GUILayout.Width(70), GUILayout.Height(20)))
						baseView.Hide();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void AssignControls(bool forced = false)
		{
			var obj = target as BaseView;
			if (obj != null)
			{
				obj.GrabComponents(forced);
				EditorUtility.SetDirty(obj);
			}
			else
				FlexLayout.Error($"{nameof(target)} is not a BaseView type or null.");
		}

		private void RebuildAutoScript()
		{
			if (!((Component)target).TryGetComponent(out RectTransform rt))
			{
				FlexLayout.Error($"{nameof(target)} is not a BaseView type or null.");
				return;
			}

			rt.name = target.GetType().Name;
			ScriptBuilder.CreateScript(rt, isBuildBaseClass, isAddBaseMethods, isAddGestureMethods, isAddDragDropMethods);
			isBuildBaseClass = isAddBaseMethods = isAddGestureMethods = isAddDragDropMethods = false;
		}

		private void RemoveAutoScript()
		{
			if (!((Component)target).TryGetComponent(out RectTransform rt))
			{
				FlexLayout.Error($"{nameof(target)} is not a BaseView type or null.");
				return;
			}

			rt.name = target.GetType().Name;
			ScriptBuilder.RemoveScript(rt);
		}
	}
}