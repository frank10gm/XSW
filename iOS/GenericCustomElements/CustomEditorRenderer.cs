using System;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace StritWalk.iOS
{
    public class CustomEditorRenderer : EditorRenderer
    {
		private UILabel _placeholderLabel;
        CustomEditor element;

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> ev)
		{
			base.OnElementChanged(ev);

			if (Element == null)
				return;

            if(ev.OldElement != null)
            {
				Control.Ended -= OnEnded;
				Control.Changed -= OnChanged;
				Control.Started -= OnFocused;
                element.TextChanged -= Element_TextChanged;
            }

            element = (CustomEditor)Element;

            //CreatePlaceholderLabel((CustomEditor)Element, Control);

            this.Control.InputAccessoryView = null;

            Control.Ended += OnEnded;
            Control.Changed += OnChanged;
            Control.Started += OnFocused;
            Control.Text = element.Placeholder;
            Control.TextColor = UIColor.Gray;
            element.TextChanged += Element_TextChanged;

		}

        private void CreatePlaceholderLabel(CustomEditor element, UITextView parent)
		{
			_placeholderLabel = new UILabel
			{
				Text = element.Placeholder,
				TextColor = element.PlaceholderColor.ToUIColor(),
				BackgroundColor = UIColor.Clear,
				//Font = UIFont.FromName(element.FontFamily, (nfloat)element.FontSize)
			};
            _placeholderLabel.Font.WithSize((nfloat)element.FontSize);
			_placeholderLabel.SizeToFit();

			parent.AddSubview(_placeholderLabel);

			parent.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
			parent.AddConstraints(
				_placeholderLabel.AtLeftOf(parent, 7),
				_placeholderLabel.WithSameCenterY(parent)
			);
			parent.LayoutIfNeeded();

			_placeholderLabel.Hidden = parent.HasText;
		}

		private void OnEnded(object sender, EventArgs args)
		{
			//if (!((UITextView)sender).HasText && _placeholderLabel != null)
				//_placeholderLabel.Hidden = false;

            if(string.IsNullOrEmpty(Control.Text) || string.IsNullOrWhiteSpace(Control.Text) || Control.Text == element.Placeholder)
            {
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
            }
		}

		private void OnChanged(object sender, EventArgs args)
		{
			//if (_placeholderLabel != null)
				//_placeholderLabel.Hidden = ((UITextView)sender).HasText;
		}

		private void OnFocused(object sender, EventArgs args)
		{
            if(Control.Text == element.Placeholder)
            {
                Control.Text = "";
            }
            Control.TextColor = UIColor.Black;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.Ended -= OnEnded;
				Control.Changed -= OnChanged;
                Control.Started -= OnFocused;			
				element.TextChanged -= Element_TextChanged;
				//_placeholderLabel?.Dispose();
				//_placeholderLabel = null;
			}
			base.Dispose(disposing);
		}

        void Element_TextChanged(object sender, TextChangedEventArgs e)
        {
			if (Control.Text == element.Placeholder)
			{
				Control.TextColor = UIColor.Gray;
				element.Ready = false;
			}
			else
			{
				element.Ready = true;
			}
        }
    }
}
