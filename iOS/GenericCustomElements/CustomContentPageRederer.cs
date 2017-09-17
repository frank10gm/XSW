using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(ItemDetailPage), typeof(CustomContentPageRederer))]
namespace StritWalk.iOS
{
    public class CustomContentPageRederer : PageRenderer
    {
        public CustomContentPageRederer()
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ParentViewController.TabBarController.TabBar.Hidden = true;

            ParentViewController.TabBarController.View.Subviews[1].Frame = new CGRect(TabBarController.View.Subviews[1].Frame.X, TabBarController.View.Subviews[1].Frame.Y, TabBarController.View.Subviews[1].Frame.Width, 0);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }

        public override bool HidesBottomBarWhenPushed
        {
            get
            {
                return true;
            }
        }
    }
}
