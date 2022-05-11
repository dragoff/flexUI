using FlexUI;
using UnityEngine.UI;

namespace Sample
{
	public partial class Sample3 : BaseView
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
		public Sample3 Build()
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

		// Called after any button has been pressed.
		protected override void OnAnyButtonPressed(Button button)
		{
		}

		// Called after any gesture has been performed.
		public override void OnGestured(GestureInfo info)
		{
		}

		#region Drag&Drop

		
		public override BaseView Clone()
		{
			var clone = Instantiate(this);
			return clone;
		}
		
		// This method is calling when the view start dragging.
		public override void OnDragStart()
		{
		}

		// This method is calling when the view is successfully dropped into other view.
		public override void OnDropped(BaseView acceptor)
		{
		}

		// This method is calling when the view is not accepted and must be destroyed or returned back to starting spot.
		public override void OnDropCancelled()
		{
		}

		// This method is calling when other view is dragged over the view.
		public override bool CanDropIn(BaseView draggedView)
		{
			return false;
		}

		// This method is calling when other view try drop into the view.
		public override void DropIn(BaseView draggedView)
		{
		}

		// This method is calling when children is moved to other view.
		public override void DropOut(BaseView draggedView, BaseView acceptor)
		{
		}

		#endregion
	}
}