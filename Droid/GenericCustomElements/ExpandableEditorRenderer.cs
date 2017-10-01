using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using StritWalk;
using StritWalk.Droid;
using System.Reflection;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.Droid
{
    public class ExpandableEditorRenderer : EditorRenderer
    {
        ExpandableEditor element;

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            if (e.OldElement != null)
            {
                Control.KeyPress -= Control_KeyPress;
            }

            if (e.NewElement != null)
            {
                element = (ExpandableEditor)Element;
                Control.Hint = element.Placeholder;
                Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
                Control.SetMaxLines(4);
                Control.SetImeActionLabel("Send", Android.Views.InputMethods.ImeAction.Send);
                Control.KeyPress += Control_KeyPress;
                Control.EditorAction += Control_EditorAction;
                element.TextChanged += Element_TextChanged;

                Control.TextChanged += (object sender, Android.Text.TextChangedEventArgs e1) =>
                {
                    if (e1.AfterCount > e1.BeforeCount)
                    {
                        var t = e1.Text.ToString();
                        if (t.Contains("\n"))
                        {
                            element?.InvokeCompleted(Control.Text.ToString());
                            Control.Text = "";
                        }
                    }
                    Control.Hint = element.Placeholder;
                    //Element.LayoutTo(new Rectangle(Element.X, Element.Y - Element.Height, Element.Width, Element.Height * 2));
                };

                //Control.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                //Control.InputType = Android.Text.InputTypes.TextFlagImeMultiLine;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        void Control_KeyPress(object sender, KeyEventArgs e)
        {

        }

        void Control_EditorAction(object sender, Android.Widget.TextView.EditorActionEventArgs e)
        {

        }

        void Element_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
