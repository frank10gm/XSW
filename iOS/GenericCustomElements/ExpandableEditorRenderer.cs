using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Linq;
using System.Drawing;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace StritWalk.iOS
{
    public class ExpandableEditorRenderer : EditorRenderer
    {
        private UILabel _placeholderLabel;
        ExpandableEditor element;
        double oneLine;
        bool toStart = true;
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        private Xamarin.Forms.Rectangle originalFrame;
        private CGRect originalKeyFrame;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            RegisterForKeyboardNotifications();
            element = (ExpandableEditor)Element;
            //CreatePlaceholderLabel((ExpandableEditor)Element, Control);
            this.Control.InputAccessoryView = null;
            Control.Ended += OnEnded;
            Control.Changed += OnChanged;
            Control.Started += OnFocused;
            Control.Text = element.Placeholder;
            Control.TextColor = UIColor.Gray;

            Control.ScrollEnabled = false;

            element.TextChanged += (sender, e1) =>
            {
                var numLines = Math.Round(Control.ContentSize.Height / Control.Font.LineHeight);
                var lines = 1;
                int count = e1.NewTextValue.Count(c => c == '\n');

                while (lines < ((numLines - oneLine) + 1))
                {
                    lines++;
                }

                if (e1.NewTextValue.LastIndexOf("\n", StringComparison.CurrentCulture) == e1.NewTextValue.Length - 1)
                {
                    if (count > 0)
                    {
                        if (e1.NewTextValue.Length < e1.OldTextValue.Length)
                        {
                            lines--;
                        }
                        else
                        {
                            lines++;
                        }
                    }
                    else
                    {
                        lines--;
                    }
                    Console.WriteLine("intervento secondario");
                }

                Console.WriteLine("TOTAL LINES " + lines + "\n");

                if (lines > 3)
                {
                    Control.ScrollEnabled = true;
                    element.ScrollReady = true;
                }
                else
                {
                    Control.ScrollEnabled = false;
                    element.ScrollReady = false;
                }

                if (Control.Text == element.Placeholder)
                {
                    Control.TextColor = UIColor.Gray;
                    element.Ready = false;
                }
                else
                {
                    element.Ready = true;
                }
            };
        }

        private void CreatePlaceholderLabel(ExpandableEditor element, UITextView parent)
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

            if (string.IsNullOrEmpty(Control.Text) || string.IsNullOrWhiteSpace(Control.Text) || Control.Text == element.Placeholder)
            {
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
            }
        }

        void OnChanged(object sender, EventArgs args)
        {
            //if (_placeholderLabel != null)
            //_placeholderLabel.Hidden = ((UITextView)sender).HasText;
        }

        void OnFocused(object sender, EventArgs args)
        {
            if (Control.Text == element.Placeholder)
            {
                Control.Text = "";
            }
            Control.TextColor = UIColor.Black;

            if (toStart)
                oneLine = Math.Round(Control.ContentSize.Height / Control.Font.LineHeight);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Control.Ended -= OnEnded;
                Control.Changed -= OnChanged;
                Control.Started -= OnFocused;

            }
            UnregisterForKeyboardNotifications();
        }

        void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
        }

        void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }
            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }

        protected virtual void OnKeyboardShow(NSNotification notification)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var editorFrame = Element.Bounds;
            originalKeyFrame = keyboardFrame;
            originalFrame = editorFrame;
            Element.LayoutTo(new Xamarin.Forms.Rectangle(editorFrame.X, (editorFrame.Y - keyboardFrame.Height), editorFrame.Width, editorFrame.Height));
        }

        protected virtual void OnKeyboardHide(NSNotification notification)
        {
            Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y, originalFrame.Width, originalFrame.Height));
        }

    }
}