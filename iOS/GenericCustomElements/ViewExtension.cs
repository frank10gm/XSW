using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace StritWalk.iOS
{
	public static class ViewExtensions
	{
        
		public static UIView FindFirstResponder(this UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = subView.FindFirstResponder();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}
	
		public static double GetViewRelativeBottom(this UIView view, UIView rootView)
		{
			var viewRelativeCoordinates = rootView.ConvertPointFromView(view.Frame.Location, view);
			var activeViewRoundedY = Math.Round(viewRelativeCoordinates.Y, 2);

			return activeViewRoundedY + view.Frame.Height;
		}

		public static bool IsKeyboardOverlapping(this UIView activeView, UIView rootView, CGRect keyboardFrame)
		{
			var activeViewBottom = activeView.GetViewRelativeBottom(rootView);
			var pageHeight = rootView.Frame.Height;
			var keyboardHeight = keyboardFrame.Height;

			var isOverlapping = activeViewBottom >= (pageHeight - keyboardHeight);

			return isOverlapping;
		}
	}
}