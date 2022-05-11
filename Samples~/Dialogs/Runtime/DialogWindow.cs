using UnityEngine;
using UnityEngine.UI;

namespace FlexUI.Dialogs
{
	public partial class DialogWindow : BaseView
	{
		public DialogResult DialogResult;
		public bool Created;

		///<summary>Data</summary>
		public string message { get; private set; }

		public string okText { get; private set; }
		public string cancelText { get; private set; }
		public bool closeByTap { get; private set; }
		public string longMessage { get; private set; }

		protected override void OnAwake()
		{
			Subscribe(OkButton, () =>
			{
				DialogResult = DialogResult.Ok;
				Hide();
				LogResult();
			});
			Subscribe(CancelButton, () =>
			{
				DialogResult = DialogResult.Cancel;
				Hide();
				LogResult();
			});
			Subscribe(IgnoreButton, () =>
			{
				DialogResult = DialogResult.Ignore;
				Hide();
				LogResult();
			});
			Subscribe(CopyToClipButton, () =>
			{
				var t = new TextEditor { text = InputField.text };
				t.SelectAll();
				t.Copy();
				CopyToClipTxt.SetText("Copied!");
			});

			Created = true;
			void LogResult() => Debug.Log($"<color=#ffff00ff>[FlexUI]</color> DialogWindow's Result is {DialogResult.ToString()}");
		}

		public DialogWindow Build(string message, string okText, string cancelText, bool closeByTap, string longMessage = null)
		{
			this.message = message;
			this.okText = okText;
			this.cancelText = cancelText;
			this.closeByTap = closeByTap;
			this.longMessage = longMessage;

			MessageText.SetText(message);
			OkText.SetText(okText);
			CancelText.SetText(cancelText);

			SetActive(OkButton, okText != null);
			SetActive(CancelButton, cancelText != null);
			SetActive(IgnoreButton, cancelText != null);
			InputField.gameObject.SetActive(!string.IsNullOrEmpty(longMessage));
			InputField.text = longMessage ?? string.Empty;

			LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

			DialogResult = DialogResult.None;

			return this;
		}

		public override BaseView Clone()
		{
			var clone = (DialogWindow)base.Clone();
			clone.message = message;
			clone.okText = okText;
			clone.cancelText = cancelText;
			clone.closeByTap = closeByTap;
			clone.longMessage = longMessage;
			return clone;
		}

		public override void OnGestured(GestureInfo info)
		{
			if (info.Gesture != FlexUI.Gesture.Tap || !closeByTap)
				return;

			Debug.Log($"<color=#ffff00ff>[FlexUI]</color> DialogWindow is closed by gesture");
			DialogResult = DialogResult.Cancel;
			info.IsHandled = true;
			Hide();
		}
	}

	public enum DialogResult
	{
		None,
		Ok,
		Cancel,
		Ignore
	}
}