using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;
//using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ItemDetailPage), typeof(CustomContentPageRederer))]
namespace StritWalk.iOS
{
    [Preserve(AllMembers = true)]
    public class CustomContentPageRederer : PageRenderer
    {
        ItemDetailPage thispage;
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        bool _isKeyboardShown;
        AppDelegate ad;

        public CustomContentPageRederer()
        {
            ad = (AppDelegate)UIApplication.SharedApplication.Delegate;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            thispage = e.NewElement as ItemDetailPage;

            //controllo della tastiera
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() => { View.EndEditing(true); });
            View.AddGestureRecognizer(gesture);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            RegisterForKeyboardNotifications();
        }

        public override void ViewWillDisappear(bool animated)
        {
            thispage.WillSparisci();
            base.ViewWillDisappear(animated);
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
            ad.KeyOn = true;

            //sending event
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            ad.KeyFrame = keyboardFrame;
            ad.TriggerKey();
        }

        private void OnKeyboardHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            ad.KeyOn = false;
        }

    }
}
