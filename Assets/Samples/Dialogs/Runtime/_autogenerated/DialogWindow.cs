/////////////////////////////////////////
// === THIS IS AUTOGENERATED CODE ===  //
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlexUI;

namespace FlexUI.Dialogs
{
    public partial class DialogWindow : FlexUI.BaseView //Autogenerated
    {
        public static DialogWindow Instance { get; private set; }
        // Controls
        #pragma warning disable 0414
        [Header("Controls (auto capture)")]
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI MessageText = default;
        [AutoGenerated, SerializeField] protected TMPro.TMP_InputField InputField = default;
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Button CopyToClipButton = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI CopyToClipTxt = default;
        [AutoGenerated, SerializeField] protected UnityEngine.RectTransform TextArea = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI Placeholder = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI InputFieldText = default;
        [AutoGenerated, SerializeField] protected UnityEngine.RectTransform BottomPanel = default;
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Button OkButton = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI OkText = default;
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Button CancelButton = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI CancelText = default;
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Button IgnoreButton = default;
        [AutoGenerated, SerializeField] protected TMPro.TextMeshProUGUI Text = default;
        #pragma warning restore 0414
        
        protected override void AutoInit()
        {
            Instance = this;
        }
        
        protected override void AutoSubscribe()
        {
            SubscribeOnChanged(MessageText);
            SubscribeOnChanged(InputField);
            SubscribeOnChanged(CopyToClipButton);
            SubscribeOnChanged(CopyToClipTxt);
            SubscribeOnChanged(TextArea);
            SubscribeOnChanged(Placeholder);
            SubscribeOnChanged(InputFieldText);
            SubscribeOnChanged(BottomPanel);
            SubscribeOnChanged(OkButton);
            SubscribeOnChanged(OkText);
            SubscribeOnChanged(CancelButton);
            SubscribeOnChanged(CancelText);
            SubscribeOnChanged(IgnoreButton);
            SubscribeOnChanged(Text);
        }
        #region Static Methods
        public static void ShowInstance() => Instance.Show();
        public static void HideInstance() => Instance.Hide();
        #endregion
    }
}