using UnityEngine;

namespace FlexUI.Dialogs
{
	public partial class DialogInput : FlexUI.BaseView
	{
		///<summary>Data</summary>
		public string message { get; private set; }
		public string okTxt { get; private set; }
		public string cancelTxt { get; private set; }
		public string placeholderText { get; private set; }
		public string Result { get; protected set; }

		protected override void OnAwake()
		{
			Subscribe(OkButton, () =>
			{
				Result = InputFieldTMP.text;
				Hide();
				LogResult();
			});
			Subscribe(CancelButton, () =>
			{
				Result = null;
				Hide();
				LogResult();
			});

			void LogResult() => Debug.Log($"<color=#ffff00ff>[FlexUI]</color> DialogInput's Result is {(string.IsNullOrEmpty(Result) ? "null or empty" : Result)}");
		}
		
		public DialogInput Build(string message, string okTxt, string cancelTxt, string placeholderText, string Result = null)
		{
			this.message = message;
			this.okTxt = okTxt;
			this.cancelTxt = cancelTxt;
			this.placeholderText = placeholderText;
			this.Result = Result;

			if (!string.IsNullOrEmpty(message))
				MessageText.SetText(message);

			if (!string.IsNullOrEmpty(okTxt))
				okText.SetText(okTxt);

			if (!string.IsNullOrEmpty(cancelTxt))
				cancelText.SetText(cancelTxt);

			if (!string.IsNullOrEmpty(placeholderText))
				InputFieldTMP.text = placeholderText;

			return this;
		}

		public override BaseView Clone()
		{
			var clone = (DialogInput)base.Clone();
			clone.message = message;
			clone.okTxt = okTxt;
			clone.cancelTxt = cancelTxt;
			clone.placeholderText = placeholderText;
			clone.Result = Result;
			return clone;
		}
	}
}