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
        private Xamarin.Forms.Rectangle originalWithKeyFrame;
        private CGRect originalKeyFrame;
        AppDelegate ad;

        public ExpandableEditorRenderer()
        {
            ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            if (e.OldElement != null)
            {
                ad.KeyAppeared -= KeyRaise;
                //UnregisterForKeyboardNotifications();
                element.TextChanged -= Element_TextChanged;
            }

            if (e.NewElement != null)
            {
                element = (ExpandableEditor)Element;
                ad.KeyAppeared += KeyRaise;
                //RegisterForKeyboardNotifications();
                //CreatePlaceholderLabel((ExpandableEditor)Element, Control);
                Control.InputAccessoryView = null;
                Control.Ended += OnEnded;
                Control.Changed += OnChanged;
                Control.Started += OnFocused;
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
                Control.ScrollEnabled = false;

                element.TextChanged += Element_TextChanged;
            }
        }

        void KeyRaise(object sender, EventArgs args)
        {
            originalKeyFrame = (CGRect)sender;
            originalFrame = Element.Bounds;
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, (originalFrame.Y - originalKeyFrame.Height), originalFrame.Width, originalFrame.Height));
        }

        void Element_TextChanged(object sender, TextChangedEventArgs e1)
        {
            originalWithKeyFrame = Element.Bounds;
            Element.LayoutTo(new Xamarin.Forms.Rectangle(originalWithKeyFrame.X, originalWithKeyFrame.Y - originalFrame.Height, originalWithKeyFrame.Width, originalWithKeyFrame.Height + originalFrame.Height));

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
                //Control.ScrollEnabled = false;
                //element.ScrollReady = false;
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
        }

        void CreatePlaceholderLabel(ExpandableEditor element, UITextView parent)
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

        void OnEnded(object sender, EventArgs args)
        {
            //if (!((UITextView)sender).HasText && _placeholderLabel != null)
            //_placeholderLabel.Hidden = false;

            if (string.IsNullOrEmpty(Control.Text) || string.IsNullOrWhiteSpace(Control.Text) || Control.Text == element.Placeholder)
            {
                Control.Text = element.Placeholder;
                Control.TextColor = UIColor.Gray;
            }

            Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y, originalFrame.Width, originalFrame.Height));
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
                ad.KeyAppeared -= KeyRaise;
                element.TextChanged -= Element_TextChanged;
                //UnregisterForKeyboardNotifications();
            }
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