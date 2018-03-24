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
        CustomEditor element;

        public CustomEditorRenderer(CustomEditor element)
        {
            this.element = element;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> ev)
		{
			base.OnElementChanged(ev);

			if (Element == null)
				return;

            element = (CustomEditor)Element;

            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
			Control.Hint = element.Placeholder;
			Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
            Control.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                Control.Hint = element.Placeholder;
            };
		}
    }
}
