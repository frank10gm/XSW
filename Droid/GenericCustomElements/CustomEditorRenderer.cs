using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using StritWalk;
using StritWalk.Droid;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace StritWalk.Droid
{
    public class CustomEditorRenderer : EditorRenderer
    {
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (Element == null)
				return;

            var element = (CustomEditor)Element;

			Control.Hint = element.Placeholder;
			Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
		}
    }
}
