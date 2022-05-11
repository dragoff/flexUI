using FlexUI.Editor;
using UnityEditor;
using UnityEngine;

namespace FlexUI.Dialogs.Editor
{
	[CustomEditor(typeof(UIManagerDialogs), true)]
	public class UIManagerDialogsEditor : UnityEditor.Editor
	{
		private SerializedProperty dialogWindowPrefab;
		private SerializedProperty dialogInputPrefab;
		private SerializedProperty overlayCanvasPrefab;

		protected void OnEnable()
		{
			dialogWindowPrefab = serializedObject.FindProperty("DialogWindowPrefab");
			dialogInputPrefab = serializedObject.FindProperty("DialogInputPrefab");
			overlayCanvasPrefab = serializedObject.FindProperty("OverlayCanvasPrefab");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label(GUIContent.none, FlexLayout.HEADER);

			serializedObject.Update();

			if (Application.isPlaying)
			{
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(
						new GUIContent("Show Message Dialog"), GUILayout.Height(20)))
						UIManagerDialogs.ShowMessageDialog("It's an example of using Error/Warning/Message dialogs.");

					if (GUILayout.Button(
						new GUIContent("Show Error Dialog"), GUILayout.Height(20)))
						UIManagerDialogs.ShowErrorDialog(
							"NullReferenceException: Object reference not set to an instance of an object. FlexUI.Dialogs.DialogWindow.Build (System.String message, System.String okText, System.String cancelText, System.Boolean closeByTap, System.String longMessage) (at Assets/Scripts/Predefined/Scripts/DialogWindow.cs:0)");

					if (GUILayout.Button(
						new GUIContent("Show Window Dialog"), GUILayout.Height(20)))
						UIManagerDialogs.ShowDialog(default, "It's an example of using Window dialogs.", cancelText: "Cancel");

					if (GUILayout.Button(
						new GUIContent("Show Input Dialog"), GUILayout.Height(20)))
						UIManagerDialogs.ShowDialogInput(default, "It's an example of using Input dialogs.");
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				EditorGUILayout.LabelField("Dialogs", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(overlayCanvasPrefab);
				EditorGUILayout.PropertyField(dialogWindowPrefab);
				EditorGUILayout.PropertyField(dialogInputPrefab);
			}
			GUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}