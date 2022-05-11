using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FlexUI
{
	public static class Extensions
	{
		public static async Task WaitUntil(Func<bool> predicate, CancellationToken? cancellationToken = null)
		{
			bool IsCanceled() => cancellationToken is { IsCancellationRequested: true };

			while (!IsCanceled() && !predicate())
			{
				await Task.Yield();
			}
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var element in enumerable)
				action(element);
		}
		
		public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
		{
			camera ??= Camera.main;
			return canvas.ViewportToCanvasPosition(camera!.WorldToViewportPoint(worldPosition));
		}

		public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition) => canvas.ViewportToCanvasPosition(new Vector3(screenPosition.x / Screen.width, screenPosition.y / Screen.height, 0));

		public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
		{
			var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
			canvas.TryGetComponent(out RectTransform canvasRect);
			var scale = canvasRect.sizeDelta;
			return Vector3.Scale(centerBasedViewPortPosition, scale);
		}
	}
}