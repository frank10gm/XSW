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
        bool keyOn;
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        Xamarin.Forms.Rectangle originalFrame;
        Xamarin.Forms.Rectangle originalWithKeyFrame;
        Xamarin.Forms.Rectangle originalPageFrame;
        Xamarin.Forms.Rectangle originalListFrame;
        bool firstassignment;
        nfloat scrolly;
        CGRect originalKeyFrame;
        int numlines = 1;
        int framelines = 0;
        AppDelegate ad;
        CGSize requestSize;

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
                UnregisterForKeyboardNotifications();
                element.TextChanged -= Element_TextChanged;
            }

            if (e.NewElement != null)
            {
                element = (ExpandableEditor)Element;
                ad.KeyAppeared += KeyRaise;
                RegisterForKeyboardNotifications();
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

            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y, originalFrame.Width, originalFrame.Height));
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
                UnregisterForKeyboardNotifications();
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
            if (!firstassignment)
            {
                originalKeyFrame = UIKeyboard.FrameEndFromNotification(notification);
                originalFrame = Element.Bounds;
                firstassignment = true;
            }
            var currentFrame = Element.Bounds;
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, originalKeyFrame.Height + (nfloat)currentFrame.Height - (nfloat)originalFrame.Height, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;
            Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y - originalKeyFrame.Height - currentFrame.Height + originalFrame.Height, originalFrame.Width, currentFrame.Height));

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }

            keyOn = true;
        }

        protected virtual void OnKeyboardHide(NSNotification notification)
        {
            keyOn = false;
            var currentFrame = Element.Bounds;
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;
            Console.WriteLine(currentFrame.Height + " " + originalFrame.Height);
            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, (nfloat)currentFrame.Height - (nfloat)originalFrame.Height, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;
            Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalFrame.Y - currentFrame.Height + originalFrame.Height, originalFrame.Width, currentFrame.Height));
            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        void AgumentView2()
        {
            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            //aumento riga
            //originalWithKeyFrame = Element.Bounds;
            framelines++;
            numlines++;
            var newy = ((originalFrame.Y) - originalKeyFrame.Height) - (originalFrame.Height - 0) * framelines;
            var newh = (originalFrame.Height * framelines) + (originalFrame.Height - 0);
            originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalFrame.X, newy, originalFrame.Width, newh);
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalWithKeyFrame.Y - (originalFrame.Height - 16), originalWithKeyFrame.Width, originalWithKeyFrame.Height + (originalFrame.Height - 16)));
            Element.LayoutTo(originalWithKeyFrame);

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, listcontrol.ContentInset.Bottom + ((nfloat)originalFrame.Height - 0), 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        void DegumentView2()
        {
            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            framelines--;
            numlines--;
            var newy = ((originalFrame.Y) - originalKeyFrame.Height) - (originalFrame.Height - 0) * framelines;
            var newh = (originalFrame.Height * framelines) + (originalFrame.Height - 0);
            originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalFrame.X, newy, originalFrame.Width, newh);
            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, listcontrol.ContentInset.Bottom - ((nfloat)originalFrame.Height - 0), 0);

            if (framelines < 1)
            {
                newy = originalFrame.Y - originalKeyFrame.Height;
                originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalFrame.X, newy, originalFrame.Width, originalFrame.Height);
                contentinsets = new UIEdgeInsets(0, 0, originalKeyFrame.Height, 0);
                framelines = 0;
                numlines = 1;
            }

            Element.LayoutTo(originalWithKeyFrame);

            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        void AgumentView3()
        {
            //definizione listview
            var list = Control.Superview.Superview.Subviews[0] as CustomListViewRenderer;
            var listview = list.Element;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            //aumento riga
            //originalWithKeyFrame = Element.Bounds;
            framelines++;
            numlines++;
            var newy = ((originalFrame.Y) - originalKeyFrame.Height) - (originalFrame.Height - 0) * framelines;
            newy = originalFrame.Y - originalKeyFrame.Height + originalFrame.Height - requestSize.Height;
            var newh = (originalFrame.Height * framelines) + (originalFrame.Height - 0);
            newh = requestSize.Height;
            originalWithKeyFrame = new Xamarin.Forms.Rectangle(originalFrame.X, newy, originalFrame.Width, newh);
            //Element.LayoutTo(new Xamarin.Forms.Rectangle(originalFrame.X, originalWithKeyFrame.Y - (originalFrame.Height - 16), originalWithKeyFrame.Width, originalWithKeyFrame.Height + (originalFrame.Height - 16)));

            if (requestSize.Height < 100)
            {
                Element.LayoutTo(originalWithKeyFrame);
                UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, listcontrol.ContentInset.Bottom + ((nfloat)requestSize.Height - 0), 0);
                contentinsets = new UIEdgeInsets(0, 0, originalKeyFrame.Height + ((nfloat)requestSize.Height - (nfloat)originalFrame.Height), 0);
                listcontrol.ContentInset = contentinsets;
                listcontrol.ScrollIndicatorInsets = contentinsets;
				Control.ScrollEnabled = false;
				element.ScrollReady = false;
            }
            else
            {
                newh = Element.Bounds.Height - 16;
                newy = Element.Bounds.Y - 16;
                originalWithKeyFrame = new Xamarin.Forms.Rectangle(Element.Bounds.X, Element.Bounds.Y, Element.Bounds.Width, newh);
                //Element.LayoutTo(originalWithKeyFrame);
				Control.ScrollEnabled = true;
				element.ScrollReady = true;
            }

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }
        }

        void Element_TextChanged(object sender, TextChangedEventArgs e1)
        {
            requestSize = Control.SizeThatFits(new CGSize(Element.Bounds.Width, 99999));

            if (keyOn)
                AgumentView3();

            //Control.SizeToFit();
            //Control.LayoutIfNeeded();

            //definizione delle righe di testo
            //var numLinesHeight = Math.Round(Control.ContentSize.Height / Control.Font.LineHeight);
            //var lines = 1;
            //int count = e1.NewTextValue.Count(c => c == '\n');

            //while (lines < ((numLines - oneLine) + 1))
            //{
            //    lines++;
            //    AgumentView();
            //}

            //if (e1.NewTextValue.LastIndexOf("\n", StringComparison.CurrentCulture) == e1.NewTextValue.Length - 1)
            //{
            //    if (keyOn)
            //    {
            //        if (numlines > 2)
            //        {

            //        }
            //        else
            //        {
            //            //AgumentView2();
            //        }
            //    }

            //    //if (e1.NewTextValue.Length < e1.OldTextValue.Length)
            //    //{
            //    //    numlines--;
            //    //}
            //    //else
            //    //{
            //    //    numlines++;
            //    //    AgumentView();
            //    //}
            //}
            //else
            //{
            //    if (keyOn)
            //    {
            //        //DegumentView2();
            //    }
            //    //DegumentView2();
            //}

            //if (numlines > 2)
            //{
            //    Control.ScrollEnabled = true;
            //    element.ScrollReady = true;
            //}
            //else
            //{
            //    Control.ScrollEnabled = false;
            //    element.ScrollReady = false;
            //}

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

    }
}