using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlexUI
{
	/// <summary>Provides view's background sounds</summary>
	public class SoundController
	{
		public float DefaultVolume { get; set; }
		public float FadeVolume { get; set; }
		public float VolumeChangeSpeed { get; set; }

		private Dictionary<AudioClip, ViewSoundInfo> viewSounds = new Dictionary<AudioClip, ViewSoundInfo>();
		private MonoBehaviour owner;

		class ViewSoundInfo
		{
			public AudioSource AudioSource;
			public int ViewsCounter;
			public int FadeViewsCounter;
			public int MuteViewsCounter;
			public float InitVolume;
			public Coroutine Coroutine;
		}

		public SoundController(MonoBehaviour owner, float defaultVolume, float fadeVolume, float volumeChangeSpeed)
		{
			this.owner = owner;
			this.DefaultVolume = defaultVolume;
			this.DefaultVolume = defaultVolume;
			this.FadeVolume = fadeVolume;
			this.VolumeChangeSpeed = volumeChangeSpeed;
		}

		public void ViewHidden(BaseView view)
		{
			if (!view.Sounds.IsSoundEnabled)
				return;

			UIManager.PlayOneShotSound(view.Sounds.IsOverrideSounds
					? view.Sounds.HideSound
					: UIManager.GetHideSound()
				, DefaultVolume);

			if (view.Sounds.BackgroundSound != null)
				StopBackground(view.Sounds.BackgroundSound);

			if (view.Sounds.OwnerMode == OwnerSoundMode.Normal)
				return;

			var owner = view.Owner;
			while (owner != null)
			{
				if (owner.Sounds.BackgroundSound != null)
					switch (view.Sounds.OwnerMode)
					{
						case OwnerSoundMode.Fade:
							StopFade(owner.Sounds.BackgroundSound);
							break;
						case OwnerSoundMode.Mute:
							StopMute(owner.Sounds.BackgroundSound);
							break;
					}

				owner = owner.Owner;
			}
		}

		public void OnViewShown(BaseView view)
		{
			if (!view.Sounds.IsSoundEnabled)
				return;

			UIManager.PlayOneShotSound(view.Sounds.IsOverrideSounds
					? view.Sounds.ShowSound
					: UIManager.GetShowSound()
				, DefaultVolume);

			if (view.Sounds.BackgroundSound != null)
				PlayBackground(view.Sounds.BackgroundSound);

			if (view.Sounds.OwnerMode == OwnerSoundMode.Normal)
				return;

			var viewOwner = view.Owner;
			while (viewOwner != null)
			{
				if (viewOwner.Sounds.BackgroundSound != null)
					switch (view.Sounds.OwnerMode)
					{
						case OwnerSoundMode.Fade:
							Fade(viewOwner.Sounds.BackgroundSound);
							break;
						case OwnerSoundMode.Mute:
							Mute(viewOwner.Sounds.BackgroundSound);
							break;
					}

				viewOwner = viewOwner.Owner;
			}
		}

		private void Mute(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.MuteViewsCounter++;
				StartVolumeChange(info, 0, false);
			}
		}

		private void StopMute(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.MuteViewsCounter--;
				if (info.MuteViewsCounter <= 0)
					StartVolumeChange(info, info.InitVolume, false);
			}
		}

		private void Fade(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.FadeViewsCounter++;
				StartVolumeChange(info, info.InitVolume * FadeVolume, false);
			}
		}

		private void StopFade(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.FadeViewsCounter--;
				if (info.FadeViewsCounter <= 0)
					StartVolumeChange(info, info.InitVolume, false);
			}
		}

		private void PlayBackground(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.ViewsCounter++;
				return;
			}

			var audio = new GameObject(backgroundSound.ToString()).AddComponent<AudioSource>();
			audio.transform.SetParent(UIManager.Transform);
			audio.volume = DefaultVolume;
			audio.loop = true;
			audio.clip = backgroundSound;
			audio.time = UnityEngine.Random.Range(0, audio.clip.length / 2);
			audio.Play();

			info = new ViewSoundInfo { AudioSource = audio, InitVolume = audio.volume };
			info.ViewsCounter++;
			viewSounds[backgroundSound] = info;

			audio.volume = 0;
			StartVolumeChange(info, info.InitVolume, false);
		}

		private void StopBackground(AudioClip backgroundSound)
		{
			if (viewSounds.TryGetValue(backgroundSound, out var info))
			{
				info.ViewsCounter--;
				if (info.ViewsCounter <= 0)
				{
					StartVolumeChange(info, 0, true);
				}
			}
		}

		private void StartVolumeChange(ViewSoundInfo info, float targetVolume, bool destroyAtEnd)
		{
			if (info.Coroutine != null)
				owner.StopCoroutine(info.Coroutine);

			info.Coroutine = owner.StartCoroutine(VolumeChangeCoroutine());

			IEnumerator VolumeChangeCoroutine()
			{
				if (info.Coroutine != null)
					owner.StopCoroutine(info.Coroutine);

				while (info.AudioSource && Math.Abs(info.AudioSource.volume - targetVolume) > 0.005f)
				{
					info.AudioSource.volume =
						Mathf.MoveTowards(info.AudioSource.volume, targetVolume, Time.unscaledDeltaTime * VolumeChangeSpeed);
					yield return null;
				}

				if (destroyAtEnd)
				{
					if (info.AudioSource)
					{
						if (viewSounds.TryGetValue(info.AudioSource.clip, out var otherInfo))
						{
							if (info == otherInfo)
								viewSounds.Remove(info.AudioSource.clip);
						}

						info.AudioSource.Stop();
						Object.Destroy(info.AudioSource.gameObject);
					}
				}
			}
		}
	}
}