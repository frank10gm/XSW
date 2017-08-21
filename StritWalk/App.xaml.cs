using System.Collections.Generic;

using Xamarin.Forms;


namespace StritWalk
{
    public partial class App : Application
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "http://www.hackweb.it/api";

        public static IDictionary<string, string> LoginParameters => null;

        public App()
        {
            InitializeComponent();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<CloudDataStore>();

            SetMainPage();
        }

        public static void SetMainPage()
        {
            if (!Settings.IsLoggedIn)
            {
                Current.MainPage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = (Color)Current.Resources["Primary"],
                    BarTextColor = Color.White
                };
            }
            else
            {
                GoToMainPage();
            }
            //GoToMainPage();
        }

        public static void LogOut()
        {
			Settings.UserId = "";
			Settings.AuthToken = "";
			Current.MainPage = new NavigationPage(new LoginPage())
			{
				BarBackgroundColor = (Color)Current.Resources["Primary"],
				BarTextColor = Color.White
			};
        }

        public static void GoToMainPage()
        {

			TabbedPage tabbedPage = new TabbedPage
			{
				Children = {
					new NavigationPage(new ItemsPage())
					{
						Icon = Device.OnPlatform("tab_about.png", null, null),
						Title = Device.OnPlatform(null, "Home", "Home")
                        //Title = "home"
                    },
					new NavigationPage(new AboutPage())
					{
						Icon = Device.OnPlatform("tab_about.png", null, null),
						Title = Device.OnPlatform(null, "Map", "Map")
                        //Title = "map"
                    },
                    new NavigationPage(new ItemsPage())
					{
						Icon = Device.OnPlatform("tab_about.png", null, null),
						Title = Device.OnPlatform(null, "News", "News")
                        //Title = "map"
                    },
					new NavigationPage(new ItemsPage())
					{
						Icon = Device.OnPlatform("slideout.png", null, null),
						Title = Device.OnPlatform(null, "Menu", "Menu")
                        //Title = "map"
                    }
				}
			};

            Current.MainPage = tabbedPage;

            //tabbedPage.CurrentPage = tabbedPage.Children[1];

        }
    }
}
