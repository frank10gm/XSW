using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(CustomContentPage), typeof(CustomContentPageRederer))]
namespace StritWalk.iOS
{
    public class CustomContentPageRederer : PageRenderer
    {
        public CustomContentPageRederer()
        {
            this.HidesBottomBarWhenPushed = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //ParentViewController.TabBarController.TabBar.Hidden = true;
            //ParentViewController.TabBarController.View.Subviews[1].Frame = new CGRect(TabBarController.View.Subviews[1].Frame.X, TabBarController.View.Subviews[1].Frame.Y, TabBarController.View.Subviews[1].Frame.Width, 0);

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

			//nfloat tabSize = 44.0f;

			//UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;

			//if (UIInterfaceOrientation.LandscapeLeft == orientation || UIInterfaceOrientation.LandscapeRight == orientation)
			//{
			//	tabSize = 32.0f;
			//}

			//CGRect rect = this.View.Frame;
			//rect.Y = this.NavigationController != null ? tabSize : tabSize + 20;
			//this.View.Frame = rect;

			if (TabBarController != null)
			{
                //HidesBottomBarWhenPushed = true;
                //TabBarController.TabBar.Hidden = true;
				CGRect tabFrame = this.TabBarController.TabBar.Frame;
				tabFrame.Height = 0;
				//tabFrame.Y = this.NavigationController != null ? 64 : 20;
				//this.TabBarController.TabBar.Frame = tabFrame;
				//this.TabBarController.TabBar.BarTintColor = UIColor.Red;
			}
        }

    }
}
