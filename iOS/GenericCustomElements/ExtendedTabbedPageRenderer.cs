using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using StritWalk;
using StritWalk.iOS;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(CustomTabbedPage), typeof(ExtendedTabbedPageRenderer))]

namespace StritWalk.iOS
{
    public class ExtendedTabbedPageRenderer : TabbedRenderer
    {

        public CoreGraphics.CGRect baseFrame;

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
            base.OnElementChanged(e);
            baseFrame = View.Subviews[1].Frame;

            //View.Subviews[1].Frame = new CoreGraphics.CGRect(View.Subviews[1].Frame.X, View.Subviews[1].Frame.Y, View.Subviews[1].Frame.Width, 49);

            // Set Text Font for unselected tab states
            //UITextAttributes normalTextAttributes = new UITextAttributes();
            //normalTextAttributes.Font = UIFont.FromName("HelveticaNeue", 15.0F); // unselected

            //UITabBarItem.Appearance.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
            //UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0,-16);
            TabBarItem.ImageInsets = new UIEdgeInsets(50, 50, 50, 50);
            //TabBar.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            //TabBar.BarTintColor = UIColor.FromRGB(255, 255, 255);
            TabBar.TintColor = UIColor.FromRGB(43, 152, 240);

			if (e.NewElement == null) return;

            var custom = e.NewElement as CustomTabbedPage;

            //controllo della tastiera
            //UITapGestureRecognizer gesture = new UITapGestureRecognizer(KeyDismiss);
            //View.Subviews[0].AddGestureRecognizer(gesture);
            //View.AddGestureRecognizer(gesture);

			this.Tabbed.PropertyChanging += async (sender, eventArgs) => {                
				if (eventArgs.PropertyName == "TabBarHidden")
				{
                    bool tabBarHidden = !custom.TabBarHidden;
					TabBar.Hidden = tabBarHidden;

                    //Console.WriteLine("@@@ hidden? " + tabBarHidden);
                    //await Task.Delay(1000);
                    //Console.WriteLine("@@@ hidden? " + tabBarHidden);

                    // //The solution to the space left behind the invisible tab bar
                    //if (TabBar.Hidden)
                    //    View.Subviews[1].Frame = new CoreGraphics.CGRect(View.Subviews[1].Frame.X, View.Subviews[1].Frame.Y, View.Subviews[1].Frame.Width, 0);
                    //else
                        //View.Subviews[1].Frame = baseFrame;
                    
				}
			};
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

        void KeyDismiss()
        {
            View.Subviews[0].EndEditing(true);
        }
    }
}
