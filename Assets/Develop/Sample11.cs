using FlexUI;
using UnityEngine.UI;

namespace SSS
{
    public partial class Sample11 : FlexUI.BaseView
    {
        
        // Called after unity Awake executed on this.
        protected override void OnAwake()
        {
            Subscribe(Button, () => { });
            
        }
        
        // Called after unity Start executed on this.
        protected override void OnStart()
        {
        }
        
        // This method is called by owner class to build the view. Building assumes passing data to View and copying data to UI controls.
        public Sample11 Build()
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
    }
}