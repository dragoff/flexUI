using UnityEditor;
using UnityEngine;

namespace FlexUI.Editor
{
	public static class HierarchyContext
	{
		[MenuItem("GameObject/UI/FlexUI Manager", false, 5000)]
		public static void CreateUIManager()
		{
			var manager = Resources.Load<UIManager>("Prefabs/FlexUI Manager Dialogs")
			              ?? Resources.Load<UIManager>("Prefabs/FlexUI Manager");
			manager = Object.Instantiate(manager);
			manager.name = "FlexUI Manager";
		}
	}
}