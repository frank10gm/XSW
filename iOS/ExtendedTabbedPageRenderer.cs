using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using StritWalk.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]

namespace StritWalk.iOS
{
    public class ExtendedTabbedPageRenderer : TabbedRenderer
    {
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

            // Set Text Font for unselected tab states
            //UITextAttributes normalTextAttributes = new UITextAttributes();
            //normalTextAttributes.Font = UIFont.FromName("HelveticaNeue", 15.0F); // unselected

            //UITabBarItem.Appearance.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
            //UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0,-16);
            TabBarItem.ImageInsets = new UIEdgeInsets(50, 50, 50, 50);
            TabBar.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            TabBar.BarTintColor = UIColor.FromRGB(255, 255, 255);
            TabBar.TintColor = UIColor.FromRGB(72, 133, 237);
		}

		public override UIViewController SelectedViewController
		{
			get
			{
				//UITextAttributes selectedTextAttributes = new UITextAttributes();
				//selectedTextAttributes.Font = UIFont.FromName("HelveticaNeue", 15.0F); // SELECTED
				//if (base.SelectedViewController != null)
				//{
				//	base.SelectedViewController.TabBarItem.SetTitleTextAttributes(selectedTextAttributes, UIControlState.Normal);
				//}
				return base.SelectedViewController;
			}
			set
			{
				base.SelectedViewController = value;

				foreach (UIViewController viewController in base.ViewControllers)
				{
					//UITextAttributes normalTextAttributes = new UITextAttributes();
					//normalTextAttributes.Font = UIFont.FromName("HelveticaNeue", 15.0F); // unselected

					//viewController.TabBarItem.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
				}
			}
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			//const float newTabBarHeight = 40f;
            //TabBar.Frame = new System.Drawing.RectangleF((float)TabBar.Frame.X, (float)TabBar.Frame.Y + ((float)TabBar.Frame.Height - newTabBarHeight), (float)TabBar.Frame.Width, newTabBarHeight);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (TabBar.Items == null) return;
			foreach (var uiTabBarItem in TabBar.Items)
			{
				//uiTabBarItem.Image = uiTabBarItem.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
				//uiTabBarItem.SetTitleTextAttributes(new UITextAttributes() {TextColor = UIColor.Black},
				//    UIControlState.Normal);
                uiTabBarItem.ImageInsets = new UIEdgeInsets(5, 0, -5, 0);
			}
		}
    }
}
