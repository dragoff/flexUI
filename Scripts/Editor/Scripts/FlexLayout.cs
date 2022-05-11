using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlexUI.Editor
{
	public static class FlexLayout
	{
		public static readonly GUISkin Skin = Resources.Load<GUISkin>("FlexSkin");

		public static readonly Color BACK_COLOR = new Color(0.8f, 0.8f, 0.86f, 1f);
		public static readonly Color ACTIVE_COLOR = new Color(0.94f, 0.73f, 0.15f, 1f);

		public static readonly Texture2D ICON_BS = Skin.customStyles.First(x => x.name.Equals("IconBaseView")).normal.background;
		public static readonly Texture2D ICON_UIM = Skin.customStyles.First(x => x.name.Equals("IconManager")).normal.background;

		public static readonly GUIStyle LABEL = Skin.customStyles.First(x => x.name.Equals("Label"));

		public static readonly GUIStyle HEADER = Skin.customStyles.First(x => x.name.Equals("Header"));

		public static readonly GUIStyle TOGGLE_TRUE = Skin.customStyles.First(x => x.name.Equals("ToggleEnabled"));
		public static readonly GUIStyle TOGGLE_FALSE = Skin.customStyles.First(x => x.name.Equals("ToggleDisabled"));

		public static void Property(SerializedProperty p, string text = "", string toolTip = "", bool includeChilds = false)
		{
			EditorGUILayout.PropertyField(p, new GUIContent(text, toolTip), includeChilds);
			Space(2);
		}

		public static void Space(float space = 10) => GUILayout.Space(space);
		public static void Error(string text) => EditorUtility.DisplayDialog("Error", text, "OK");
		public static void Message(string text) => EditorUtility.DisplayDialog("Info", text, "OK");

		public static void Label(string text, string tooltip = "", params GUILayoutOption[] options)
		{
			GUIStyle style = LABEL;
			GUILayout.Label(new GUIContent(text, tooltip), style, options);
		}

		public static bool ToggleLabel(SerializedProperty p, string text, string tooltip = "", float height = 12)
		{
			bool result = p.boolValue;
			var color = result ? ACTIVE_COLOR : BACK_COLOR;
			var toggleStyle = result ? TOGGLE_TRUE : TOGGLE_FALSE;

			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			{
				GUILayout.BeginHorizontal(GUILayout.Height(height), GUILayout.ExpandWidth(true));
				{
					result = Toggle(p.boolValue, toggleStyle, color, height);
					Label(text, tooltip);
					Space();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			p.boolValue = result;
			return result;
		}

		public static bool ToggleLabel(bool p, string text, string tooltip = "", float height = 12)
		{
			var color = p ? ACTIVE_COLOR : BACK_COLOR;
			var toggleStyle = p ? TOGGLE_TRUE : TOGGLE_FALSE;

			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			{
				GUILayout.BeginHorizontal(GUILayout.Height(height), GUILayout.ExpandWidth(true));
				{
					p = Toggle(p, toggleStyle, color, height);
					Label(text, tooltip);
					Space();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			return p;
		}
		
		private static bool Toggle(bool value, GUIStyle toggleStyle, Color color, float height)
		{
			Color initialColor = GUI.color;
			GUI.color = color;
			EditorGUI.BeginChangeCheck();
			value = GUILayout.Toggle(value, GUIContent.none, toggleStyle);
			if (EditorGUI.EndChangeCheck())
			{
				GUIUtility.keyboardControl = 0;
			}

			GUI.color = initialColor;
			return value;
		}

		// Implementation taken from https://forum.unity.com/threads/free-hierarchy-window-icons-on-objects-via-interfaces.436548/
		[InitializeOnLoad]
		class HierarchyIcons
		{
			static HierarchyIcons()
			{
				EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
			}

			static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
			{
				var rect = new Rect(selectionRect) { x = 32, width = 20 };
				var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

				if (!go)
					return;

				if (go.GetComponent<FlexUI.BaseView>())
					GUI.Label(rect, ICON_BS);

				if (go.GetComponent<FlexUI.UIManager>())
					GUI.Label(rect, ICON_UIM);
			}
		}
	}
}