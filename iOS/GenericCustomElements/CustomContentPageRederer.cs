using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ItemDetailPage), typeof(CustomContentPageRederer))]
namespace StritWalk.iOS
{
    [Preserve(AllMembers = true)]
    public class CustomContentPageRederer : PageRenderer
    {

        ItemDetailPage gino;

        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        private bool _pageWasShiftedUp;
        private double _activeViewBottom;
        private bool _isKeyboardShown;

        //public CustomContentPageRederer()
        //{

        //}

        //public override void ViewWillAppear(bool animated)
        //{
        //base.ViewWillAppear(animated);
        //this.HidesBottomBarWhenPushed = true;
        //ParentViewController.TabBarController.TabBar.Hidden = true;
        //ParentViewController.TabBarController.View.Subviews[1].Frame = new CGRect(TabBarController.View.Subviews[1].Frame.X, TabBarController.View.Subviews[1].Frame.Y, TabBarController.View.Subviews[1].Frame.Width, 0);

        //}

        //public override void ViewDidLoad()
        //{
        //    base.ViewDidLoad();
        //}

        //public static void Init()
        //{
        //	var now = DateTime.Now;
        //	Debug.WriteLine("Keyboard Overlap plugin initialized {0}", now);
        //}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var page = Element as ContentPage;

            if (page != null)
            {
                var contentScrollView = page.Content as ScrollView;

                if (contentScrollView != null)
                    return;

                RegisterForKeyboardNotifications();
            }
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            gino = e.NewElement as ItemDetailPage;
        }

        public override void ViewWillDisappear(bool animated)
        {
            gino.WillSparisci();
            base.ViewWillDisappear(animated);
            UnregisterForKeyboardNotifications();
        }

        //public override void ViewWillLayoutSubviews()
        //{
        //base.ViewWillLayoutSubviews();

        //nfloat tabSize = 44.0f;

        //UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;

        //if (UIInterfaceOrientation.LandscapeLeft == orientation || UIInterfaceOrientation.LandscapeRight == orientation)
        //{
        //	tabSize = 32.0f;
        //}

        //CGRect rect = this.View.Frame;
        //rect.Y = this.NavigationController != null ? tabSize : tabSize + 20;
        //this.View.Frame = rect;

        //if (TabBarController != null)
        //{
        //HidesBottomBarWhenPushed = true;
        //TabBarController.TabBar.Hidden = true;
        //CGRect tabFrame = this.TabBarController.TabBar.Frame;
        //tabFrame.Height = 0;
        //tabFrame.Y = this.NavigationController != null ? 64 : 20;
        //this.TabBarController.TabBar.Frame = tabFrame;
        //this.TabBarController.TabBar.BarTintColor = UIColor.Red;
        //}
        //}

        void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
        }

        void UnregisterForKeyboardNotifications()
        {
            _isKeyboardShown = false;
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
            if (!IsViewLoaded || _isKeyboardShown)
                return;

            _isKeyboardShown = true;
            var activeView = View.FindFirstResponder();

            if (activeView == null)
                return;

            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

            if (!isOverlapping)
                return;

            if (isOverlapping)
            {
                _activeViewBottom = activeView.GetViewRelativeBottom(View);
                ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
            }
        }

        private void OnKeyboardHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

            if (_pageWasShiftedUp)
            {
                ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
            }
        }

        private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;

            var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            var newH = pageFrame.Height + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            //Element.LayoutTo(new Rectangle(pageFrame.X, newY,
            //pageFrame.Width, pageFrame.Height));
            //dev10n
            Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, newH));

            _pageWasShiftedUp = true;
        }

        private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;

            var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            var newH = pageFrame.Height - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y,
                                           pageFrame.Width, newH));

            _pageWasShiftedUp = false;
        }

        private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
        {
            return (pageHeight - activeViewBottom) - keyboardHeight;
        }

    }
}
