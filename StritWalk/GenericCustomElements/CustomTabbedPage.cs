using System;
using Xamarin.Forms;

namespace StritWalk
{
    public class CustomTabbedPage : TabbedPage
    {

		public static readonly BindableProperty TabBarHiddenProperty =
		BindableProperty.Create("TabBarHidden", typeof(bool), typeof(CustomTabbedPage), false);

		public bool TabBarHidden
		{
			get { return (bool)GetValue(TabBarHiddenProperty); }
			set { SetValue(TabBarHiddenProperty, value); }
		}

		public CustomTabbedPage()
        {
        }
    }
}
