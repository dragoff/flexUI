using System;
using FlexUI;
using UnityEngine;

namespace Sample
{
	public partial class SampleScript2 : BaseView
	{
		protected override void OnHidden()
		{
			Debug.Log($"{GetType().Name} is closed");
		}
	}
}