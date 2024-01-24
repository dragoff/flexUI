using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlexUI;

namespace Sample
{
    public class Sample4 : FlexUI.BaseView
    {
        
        // Called after unity Awake executed on this.
        protected override void OnAwake()
        {
        }
        
        // Called after unity Start executed on this.
        protected override void OnStart()
        {
        }
        
        // This method is called by owner class to build the view. Building assumes passing data to View and copying data to UI controls.
        public Sample4 Build()
        {
            return this;
        }
        
        // Called after view has been Shown.
        protected override void OnShown()
        {
        }
        
        // Called after the view has been hidden.
        protected override void OnHidden()
        {
        }
        
        // Called after view has been Shown or Hidden.
        protected override void OnVisibleStateChanged(VisibleState currentState)
        {
        }
        
        // OnChanged is automatically called when value of controls was changed by user or buttons were pressed.
        protected override void OnChanged()
        {
        }
        
        // Called after any button has been pressed.
        protected override void OnAnyButtonPressed(Button button)
        {
        }
        
        // Called after any gesture has been performed.
        public override void OnGestured(GestureInfo info)
        {
        }
#region Drag&Drop
        
        // This method is called when this view is cloned
        public override BaseView Clone()
        {
            var clone = Instantiate(this);
            return clone;
        }
        
        // This method is called when the view start dragging.
        public override void OnDragStart()
        {
        }
        
        // This method is called when the view is successfully dropped into other view.
        public override void OnDropped(BaseView acceptor)
        {
        }
        
        // This method is called when the view is not accepted and must be destroyed or returned back to starting spot.
        public override void OnDropCancelled()
        {
        }
        
        // This method is called when other view is dragged over the view.
        public override bool CanDropIn(BaseView draggedView)
        {
            return false;
        }
        
        // This method is called when other view try drop into the view.
        public override void DropIn(BaseView draggedView)
        {
        }
        
        // This method is called when children is moved to other view.
        public override void DropOut(BaseView draggedView, BaseView acceptor)
        {
        }
#endregion
#region Autogenerated
        //////////////////////////////////////////////////////////////////////////////////
        // === THIS IS AUTOGENERATED CODE ===  //
        // === DO NOT CHANGE MANUALLY ===  //
#pragma warning disable 0414
#if UNITY_EDITOR
        /// <summary>
        /// Used as mark for Editor.
        /// </summary>
        [HideInInspector] public bool IsBuilt = true;
#endif
        public static Sample4 Instance { get; private set; }
        
        // Controls
        [Header("Controls (auto capture)")]
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Button Button = default;
        [AutoGenerated, SerializeField] protected UnityEngine.UI.Text Text = default;
        protected override void AutoInit()
        {
            Instance = this;
        }
        
        protected override void AutoSubscribe()
        {
            SubscribeOnChanged(Button);
            SubscribeOnChanged(Text);
        }
        public static void ShowInstance() => Instance.Show();
        public static void HideInstance() => Instance.Hide();
#pragma warning restore 0414
        // === End of AUTOGENERATED CODE ===  //
        //////////////////////////////////////////////////////////////////////////////////
#endregion
    }
}