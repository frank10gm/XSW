using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using StritWalk;
using StritWalk.Droid;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.Droid
{
	public class ExpandableEditorRenderer : EditorRenderer
	{
		ExpandableEditor element;
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> ev)
		{
			base.OnElementChanged(ev);

			if (Element == null)
				return;

			element = (ExpandableEditor)Element;

			Control.Hint = element.Placeholder;
			Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
			Control.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				Control.Hint = element.Placeholder;
			};
		}
	}
}
