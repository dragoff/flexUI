using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace FlexUI.Dialogs
{
	public class UIManagerDialogs : Singleton<UIManagerDialogs>
	{
		#region Dialog Windows

		[SerializeField] private DialogWindow DialogWindowPrefab = default;
		[SerializeField] private DialogInput DialogInputPrefab = default;
		[SerializeField] private Canvas OverlayCanvasPrefab = default;
		
		private Canvas canvas = null;

		/// <summary>
		/// Shows formatted error dialog.
		/// </summary>
		public static void ShowErrorDialog(string message, int messageLenghtLimit = 200,
			string messageLimitText = "<color #ff0000ff>Error!</color>\nCopy & send the stacktrace.")
		{
			if (message.Length > messageLenghtLimit)
				ShowDialog(default, messageLimitText, longMessage: message);
			else
				ShowDialog(default, message);
		}

		public static void ShowErrorDialog(string message) => ShowErrorDialog(message, 200);

		/// <summary>
		/// Shows formatted warning dialog.
		/// </summary>
		public static void ShowWarningDialog(string message, int messageLenghtLimit = 200,
			string messageLimitText = "<color #ffff00ff>Warning!</color>")
		{
			if (message.Length > messageLenghtLimit)
				ShowDialog(default, messageLimitText, longMessage: message);
			else
				ShowDialog(default, message);
		}

		public static void ShowWarningDialog(string message) => ShowWarningDialog(message, 200);

		/// <summary>
		/// Shows formatted message dialog.
		/// </summary>
		public static void ShowMessageDialog(string message, int messageLenghtLimit = 200, string messageLimitText = "Message!")
		{
			if (message.Length > messageLenghtLimit)
				ShowDialog(default, messageLimitText, longMessage: message);
			else
				ShowDialog(default, message);
		}

		public static void ShowMessageDialog(string message) => ShowMessageDialog(message, 200);

		public static DialogWindow ShowDialog(BaseView owner, string message, string okText = "Ok", string cancelText = null,
			bool isHideByTap = false, Action<DialogResult> hiddenCallback = null, string longMessage = null)
		{
			Instance.canvas ??= Instantiate(Instance.OverlayCanvasPrefab);
			var view = Instantiate(Instance.DialogWindowPrefab, Instance.canvas.transform);
			if (hiddenCallback != null)
				view.Hidden += () => hiddenCallback(view.DialogResult);

			view.IsDynamicallyCreated = true;
			view.Build(message, okText, cancelText, isHideByTap, longMessage);
			view.Show(owner);
			return view;
		}

		public static async Task<DialogResult> ShowDialogAsync(BaseView owner, string message, string okText = "Ok",
			string cancelText = null, bool isHideByTap = false, string longMessage = null)
		{
			Instance.canvas ??= Instantiate(Instance.OverlayCanvasPrefab);
			var view = Instantiate(Instance.DialogWindowPrefab, Instance.canvas.transform);
			view.GrabComponents();
			view.Initialize();
			view.gameObject.SetActive(false);
			view.Build(message, okText, cancelText, isHideByTap, longMessage);
			await view.ShowAsync(owner);
			Destroy(Instance.canvas.gameObject, 1);
			return view.DialogResult;
		}

		public static IEnumerator ShowDialogCoroutine(BaseView owner = default, string message = null, string okText = "Ok",
			string cancelText = null, bool isHideByTap = false, Action<DialogResult> hiddenCallback = null, string longMessage = null)
		{
			var view = Instantiate(Instance.DialogWindowPrefab);
			view.Build(message, okText, cancelText, isHideByTap, longMessage);
			yield return view.ShowCoroutine(owner);
			hiddenCallback?.Invoke(view.DialogResult);
		}

		public static DialogInput ShowDialogInput(BaseView owner = default, string message = null, string placeHolderText = "",
			string okText = "OK", string cancelText = "Cancel", Action<string> hiddenCallback = null)
		{
			Instance.canvas ??= Instantiate(Instance.OverlayCanvasPrefab);
			var view = Instantiate(Instance.DialogInputPrefab, Instance.canvas.transform);
			if (hiddenCallback != null)
				view.Hidden += () => hiddenCallback(view.Result);

			view.IsDynamicallyCreated = true;
			view.Build(message, okText, cancelText, placeHolderText);
			view.Show(owner);
			return view;
		}

		public static async Task<string> ShowDialogInputAsync(BaseView owner = null, string message = null, string placeHolderText = "",
			string okText = "OK", string cancelText = "Cancel")
		{
			Instance.canvas ??= Instantiate(Instance.OverlayCanvasPrefab);
			var view = Instantiate(Instance.DialogInputPrefab, Instance.canvas.transform);

			view.GrabComponents();
			view.Initialize();
			view.gameObject.SetActive(false);
			view.Build(message, okText, cancelText, placeHolderText);

			await view.ShowAsync(owner);

			Destroy(Instance.canvas.gameObject, 1);

			return view.Result;
		}

		public static IEnumerator ShowDialogInputCoroutine(BaseView owner, string message, string text, string placeHolderText = "",
			string okText = "OK", string cancelText = "Cancel", Action<string> hiddenCallback = null)
		{
			Instance.canvas ??= Instantiate(Instance.OverlayCanvasPrefab);
			var view = Instantiate(Instance.DialogInputPrefab, Instance.canvas.transform);
			view.Build(message, okText, cancelText, placeHolderText);

			yield return view.ShowCoroutine(owner);

			Destroy(Instance.canvas.gameObject, 1);
			hiddenCallback?.Invoke(view.Result);
		}

		#endregion
	}
}