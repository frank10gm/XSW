﻿using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;
//using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(ItemDetailPage), typeof(CustomContentPageRederer))]
namespace StritWalk.iOS
{
    [Preserve(AllMembers = true)]
    public class CustomContentPageRederer : PageRenderer
    {
        ItemDetailPage thispage;
        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        bool _pageWasShiftedUp;
        double _activeViewBottom;
        bool _isKeyboardShown;
        Rectangle originalFrame;
        Rectangle originalEditorFrame;
        CGRect originalKeyboardFrame;
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

            var page = Element as ContentPage;
            thispage.WillAppari();

            //if (page != null)
            //{
            //var contentScrollView = page.Content as ScrollView;
            //if (contentScrollView != null)
            //return;
            //RegisterForKeyboardNotifications();
            //}
            //RegisterForKeyboardNotifications();
        }

        public override void ViewDidAppear(bool animated)
        {
            thispage.WillAppari();
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            thispage.WillSparisci();
            //base.ViewWillDisappear(animated);
            //UnregisterForKeyboardNotifications();
        }

        public override void ViewDidDisappear(bool animated)
        {
            thispage.WillSparisci();
            base.ViewDidDisappear(animated);
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

            ad.KeyOn = false;
            _isKeyboardShown = true;

            //sending event
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            originalKeyboardFrame = keyboardFrame;
            ad.KeyFrame = keyboardFrame;
            ad.TriggerKey();

            var activeView = View.FindFirstResponder();

            if (activeView == null)
                return;

            var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

            if (!isOverlapping)
                return;

            if (isOverlapping)
            {
                _activeViewBottom = activeView.GetViewRelativeBottom(View);
                ShiftPageUp2(keyboardFrame.Height, _activeViewBottom);
            }


        }

        void OnKeyboardHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            ad.KeyOn = false;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

            if (_pageWasShiftedUp)
            {
                ShiftPageDown2(keyboardFrame.Height, _activeViewBottom);
            }
        }

        void ShiftPageUp2(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;
            originalFrame = pageFrame;

            var list = View.Subviews[0].Subviews[0] as CustomListViewRenderer;
            var listview = list.Element as CustomListView;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, keyboardHeight, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;

            IList<CommentsItem> items = listsource.list.ItemsSource as IList<CommentsItem>;
            if (items.Count > 0)
            {
                var el = items[items.Count - 1];
                listview.ScrollTo(el, ScrollToPosition.End, true);
            }

            _pageWasShiftedUp = true;

            var editor = View.Subviews[0].Subviews[1] as ExpandableEditorRenderer;
            originalEditorFrame = editor.Element.Bounds;
            editor.Element.LayoutTo(new Rectangle(originalEditorFrame.X, originalEditorFrame.Y - keyboardHeight, originalEditorFrame.Width, originalEditorFrame.Height));
        }

        void ShiftPageDown2(nfloat keyboardHeight, double activeViewBottom)
        {
            var list = View.Subviews[0].Subviews[0] as CustomListViewRenderer;
            var listview = list.Element as CustomListView;
            var listsource = list.Control.Source as CustomListViewSource;
            var listcontrol = list.Control;
            var editor = View.Subviews[0].Subviews[1] as ExpandableEditorRenderer;

            UIEdgeInsets contentinsets = new UIEdgeInsets(0, 0, (nfloat)editor.Element.Height - 16, 0);
            listcontrol.ContentInset = contentinsets;
            listcontrol.ScrollIndicatorInsets = contentinsets;
            editor.Element.LayoutTo(new Rectangle(originalEditorFrame.X, originalEditorFrame.Y - editor.Element.Height + originalEditorFrame.Height - 16, originalEditorFrame.Width, editor.Element.Height - 16));

            _pageWasShiftedUp = false;
        }

        async void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;
            originalFrame = pageFrame;
            var newH = pageFrame.Height + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            await Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, newH));
            _pageWasShiftedUp = true;

            await Task.Delay(10);

            var gino2 = View.Subviews[0].Subviews[0] as CustomListViewRenderer;
            var xel = gino2.Element as CustomListView;
            var list = gino2.Control.Source as CustomListViewSource;
            IList<CommentsItem> items = list.list.ItemsSource as IList<CommentsItem>;
            var el = items[items.Count - 1];
            xel.ScrollTo(el, ScrollToPosition.End, true);

            await Task.Delay(5000);
            //ad.KeyOn = true;
        }

        private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
        {
            //var pageFrame = Element.Bounds;

            //var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);
            //var newH = pageFrame.Height - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            //Element.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y,
            //                               pageFrame.Width, newH));                     

            Element.LayoutTo(originalFrame);
            _pageWasShiftedUp = false;
        }

        private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
        {
            return (pageHeight - activeViewBottom) - keyboardHeight;
        }

    }
}
