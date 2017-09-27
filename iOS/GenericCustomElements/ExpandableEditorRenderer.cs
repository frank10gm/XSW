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
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private Xamarin.Forms.Rectangle originalPageFrame;
        private Xamarin.Forms.Rectangle originalListFrame;
        private nfloat scrolly;
        private CGRect originalKeyFrame;
        int numlines = 1;
        int framelines = 1;
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
            //definizione delle righe di testo
            var numLines = Math.Round(Control.ContentSize.Height / Control.Font.LineHeight);
            var lines = 1;
            int count = e1.NewTextValue.Count(c => c == '\n');

            //while (lines < ((numLines - oneLine) + 1))
            //{
            //    lines++;
            //    AgumentView();
            //}

            if (e1.NewTextValue.LastIndexOf("\n", StringComparison.CurrentCulture) == e1.NewTextValue.Length - 1)
            {
                numlines++;
                AgumentView2();
                //if (e1.NewTextValue.Length < e1.OldTextValue.Length)
                //{
                //    numlines--;
                //}
                //else
                //{
                //    numlines++;
                //    AgumentView();
                //}
            }

            Console.WriteLine("TOTAL LINES " + numlines + "");

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

            //placeholder
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

        void AgumentView2()
        {
            //definizione pagina commenti
            ItemDetailPage page = Element.Parent.Parent as ItemDetailPage;
            originalPageFrame = page.Bounds;

            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            //Console.WriteLine("orignalframe:" + originalFrame.Height + " originalwithkey:" + originalWithKeyFrame.Height + " insetbottom:" + listcontrol.ContentInset.Bottom);

            //aumento riga
            //originalWithKeyFrame = Element.Bounds;
            Console.WriteLine(Element.Bounds.Y - (originalFrame.Height - 16));
            var newy = ((originalFrame.Y) - originalKeyFrame.Height) - (originalFrame.Height - 16) * framelines;
            var newh = (originalFrame.Height * framelines) + (originalFrame.Height - 16);
            Console.WriteLine(newy);
            originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalFrame.X, newy, originalFrame.Width, newh);
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalWithKeyFrame.Y - (originalFrame.Height - 16), originalWithKeyFrame.Width, originalWithKeyFrame.Height + (originalFrame.Height - 16)));
            Element.LayoutTo(originalWithKeyFrame);
            framelines++;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, listcontrol.ContentInset.Bottom + ((nfloat)originalFrame.Height - 16), 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        async void AgumentView()
        {
            //aggiunta riga
            //definizione pagina commenti
            ItemDetailPage page = Element.Parent.Parent as ItemDetailPage;
            originalPageFrame = page.Bounds;
            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listv = list.Element;
            var source = list.Control.Source as CustomListViewSource;
            IList<CommentsItem> items = source.list.ItemsSource as IList<CommentsItem>;
            var el = items[items.Count - 1];
            originalListFrame = list.Element.Bounds;
            scrolly = list.Control.ContentOffset.Y;
            //Console.WriteLine(newy + " ");
            await listv.LayoutTo(new Xamarin.Forms.Rectangle(originalListFrame.X, originalListFrame.Y, originalListFrame.Width, originalListFrame.Height - originalFrame.Height), 0, Easing.Linear);
            //CGRect lframe = Control.Frame;
            //lframe.Size.Height = originalListFrame.Height - originalFrame.Height;
            //Control.Frame = new CGRect(originalListFrame.X, originalListFrame.Y, originalListFrame.Width, 0);
            await Task.Delay(1);
            //var newy = list.Control.ContentSize.Height + list.Control.ContentInset.Bottom - list.Control.Bounds.Size.Height;			
            var newy = list.Control.ContentSize.Height + list.Control.ContentInset.Bottom - list.Control.Bounds.Size.Height;
            list.Control.SetContentOffset(new CGPoint(0, newy), false);
            //listv.ScrollTo(el, ScrollToPosition.End, true);
            //await Task.Delay(10);
            //ridefinizione editor
            originalWithKeyFrame = Element.Bounds;
            await Element.LayoutTo(new Xamarin.Forms.Rectangle(originalWithKeyFrame.X, originalWithKeyFrame.Y - originalFrame.Height, originalWithKeyFrame.Width, originalWithKeyFrame.Height + originalFrame.Height), 0, Easing.Linear);
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