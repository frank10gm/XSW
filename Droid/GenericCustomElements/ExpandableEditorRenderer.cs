using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using StritWalk;
using StritWalk.Droid;
using System.Reflection;
using Android.Graphics;
using Android.Views;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.Droid
{
    public class ExpandableEditorRenderer : EditorRenderer
    {
        ExpandableEditor element;
        int originalheight;
        int originallineheight;
        Rectangle originalframe;
        Rectangle originalwkframe;
        bool startedkey;
        bool startedkey2;
        double originalflineheight;

        ItemDetailPage page;
        ViewGroup pagecontrol;
        CustomListViewRenderer list;
        CustomListView listview;
        Android.Widget.ListView listcontrol;

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
                Control.ImeOptions = Android.Views.InputMethods.ImeAction.Send;

                Control.KeyPress += Control_KeyPress;
                Control.EditorAction += Control_EditorAction;
                Control.TextChanged += Control_TextChanged;
                element.TextChanged += Element_TextChanged;
                element.MeasureInvalidated += Element_MeasureInvalidated;

                Control.InputType = Android.Text.InputTypes.TextFlagImeMultiLine | Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationNormal | Android.Text.InputTypes.NumberVariationNormal;
                Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
                Control.SetHorizontallyScrolling(false);
            }
        }

        private void Element_MeasureInvalidated(object sender, EventArgs e)
        {
            //if (Control.LineCount >= 1 && Control.LineCount <= 4)
            //{
            //    Control.SetHeight(originalheight + ((Control.LineCount - 1) * originallineheight));
            //}
        }

        private void Control_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {


            if (!startedkey2 && Math.Abs(element.Y) > 0)
            {
                originalwkframe = new Rectangle(originalframe.X, originalframe.Y - (originalframe.Y - element.Y), originalframe.Width, originalframe.Height);
                originalflineheight = (originalframe.Height * originallineheight) / originalheight;
                startedkey2 = true;

                page = Element.Parent.Parent as ItemDetailPage;
                pagecontrol = Control.Parent.Parent as ViewGroup;
                list = pagecontrol.GetChildAt(0) as CustomListViewRenderer;
                listview = list.Element as CustomListView;
                listcontrol = list.Control as Android.Widget.ListView;
            }

            if (Control.LineCount >= 1 && Control.LineCount <= 4)
            {
                var newh = originalheight + (originallineheight * (Control.LineCount - 1));
                var newfy = originalwkframe.Y - (originalflineheight * (Control.LineCount - 1));
                var newfh = originalframe.Height + (originalflineheight * (Control.LineCount - 1));
                Control.SetHeight(newh);
                element.Layout(new Rectangle(element.X, newfy, element.Width, newfh));
                //inset della lista di seguito  
                listcontrol.SetPadding(0, 0, 0, newh - originalheight);
            }

            if (e.AfterCount > e.BeforeCount)
            {
                var t = e.Text.ToString();
                if (t.Contains("\n"))
                {
                    element?.InvokeCompleted(Control.Text.ToString());
                    Control.Text = "";
                }
            }
            Control.Hint = element.Placeholder;

            //Element.LayoutTo(new Rectangle(Element.X, Element.Y - Element.Height, Element.Width, Element.Height * 2));                                                                                          
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
            element?.InvokeCompleted(Control.Text.ToString());
            Control.Text = "";
        }

        void Element_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            //controllo altezza
            if (!startedkey)
            {
                originalframe = element.Bounds;
                originallineheight = Control.LineHeight;
                originalheight = Control.Height;
                startedkey = true;
            }
        }

    }
}
