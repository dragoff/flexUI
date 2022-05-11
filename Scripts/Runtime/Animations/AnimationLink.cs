using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace FlexUI.Animations
{
	[CreateAssetMenu(fileName = "Animation", menuName = "FlexUI/Animation", order = 2)]
	[Serializable]
	public class AnimationLink : ScriptableObject, ISerializationCallbackReceiver
	{
		[NonSerialized]
		EaseAnimation easeAnimation;

		[SerializeField]
		[HideInInspector]
		byte[] raw;

		public EaseAnimation EaseAnimation
		{
			get
			{
				if (easeAnimation != null)
				{
					return easeAnimation;
				}

				if (raw == null)
				{
					easeAnimation = new EaseAnimation();
				}
				else
				{
					using (var ms = new MemoryStream(raw))
						try
						{
							easeAnimation = (EaseAnimation)new BinaryFormatter().Deserialize(ms);
						}
						catch (Exception ex)
						{
							Debug.LogException(ex);
							easeAnimation = new EaseAnimation();
						}
				}

				foreach (var sa in EaseAnimation.Sequence)
				foreach (var a in sa.Animations)
					a.OnAfterDeserialize();

				raw = null;
				return easeAnimation;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (easeAnimation == null)
			{
				return;
			}

			foreach (var sa in EaseAnimation.Sequence)
			foreach (var a in sa.Animations)
				a.OnBeforeSerialize();

			using (var ms = new MemoryStream())
			{
				new BinaryFormatter().Serialize(ms, EaseAnimation);
				raw = ms.ToArray();
			}
		}

		public static implicit operator EaseAnimation(AnimationLink link)
		{
			return link?.EaseAnimation;
		}
	}
}