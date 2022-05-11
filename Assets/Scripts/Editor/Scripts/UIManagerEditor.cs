using System;
using FlexUI;
using UnityEditor;
using UnityEngine;

namespace FlexUI.Editor
{
	[CustomEditor(typeof(UIManager), true)]
	public class UIManagerEditor : UnityEditor.Editor
	{
		private UIManager ui;

		private SerializedProperty replayShowAnimation;
		private SerializedProperty destroyDynamicViews;
		private SerializedProperty showAnimation;
		private SerializedProperty hideAnimation;

		private SerializedProperty audioSource;
		private SerializedProperty showSound;
		private SerializedProperty hideSound;
		private SerializedProperty buttonSound;
		private SerializedProperty swipeSound;

		private SerializedProperty defaultVolume;
		private SerializedProperty fadeVolume;
		private SerializedProperty volumeChangeSpeed;

		protected void OnEnable()
		{
			ui = target as UIManager;
			replayShowAnimation = serializedObject.FindProperty("ReplayShowAnimation");
			destroyDynamicViews = serializedObject.FindProperty("DestroyDynamicViews");

			showAnimation = serializedObject.FindProperty("ShowAnimation");
			hideAnimation = serializedObject.FindProperty("HideAnimation");

			audioSource = serializedObject.FindProperty("AudioSource");
			showSound = serializedObject.FindProperty("ShowSound");
			hideSound = serializedObject.FindProperty("HideSound");
			buttonSound = serializedObject.FindProperty("ButtonSound");
			swipeSound = serializedObject.FindProperty("SwipeSound");

			defaultVolume = serializedObject.FindProperty("DefaultVolume");
			fadeVolume = serializedObject.FindProperty("FadeVolume");
			volumeChangeSpeed = serializedObject.FindProperty("VolumeChangeSpeed");
			
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawHead();
			DrawGeneral();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawHead()
		{
			GUILayout.Label(GUIContent.none, FlexLayout.HEADER);

			using (new EditorGUI.DisabledScope(true))
				EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
		}

		private void DrawGeneral()
		{
			FlexLayout.ToggleLabel(replayShowAnimation, "Replay Show Animation", "Play ShowAnimation when view already shown");
			FlexLayout.ToggleLabel(destroyDynamicViews, "Destroy Dynamic Views",
				"Dynamically created Views will be destroyed after closing");

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(showAnimation);
				EditorGUILayout.PropertyField(hideAnimation);
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(audioSource);
				GUILayout.Space(4);
				EditorGUILayout.PropertyField(buttonSound);
				EditorGUILayout.PropertyField(swipeSound);
				GUILayout.Space(4);
				EditorGUILayout.PropertyField(showSound);
				EditorGUILayout.PropertyField(hideSound);
				GUILayout.Space(4);
				if (!Application.isPlaying)
					DrawSoundOptions();
				else
				{
					EditorGUI.BeginChangeCheck();
					DrawSoundOptions();
					if (EditorGUI.EndChangeCheck())
					{
						ui.SoundController.DefaultVolume = defaultVolume.floatValue;
						ui.SoundController.FadeVolume = fadeVolume.floatValue;
						ui.SoundController.VolumeChangeSpeed = volumeChangeSpeed.floatValue;
					}
				}
			}
			GUILayout.EndVertical();

			void DrawSoundOptions()
			{
				EditorGUILayout.PropertyField(defaultVolume);
				EditorGUILayout.PropertyField(fadeVolume);
				EditorGUILayout.PropertyField(volumeChangeSpeed);
			}
		}
	}
}