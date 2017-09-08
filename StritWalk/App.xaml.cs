using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace StritWalk
{
    public partial class App : Application
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "http://www.hackweb.it/api";
        public static TabbedPage tabbedPage;
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
            
            tabbedPage = new TabbedPage
            {
                Children = {
                    new NavigationPage(new ItemsPage())
                    {                        
                        Icon = Platformer("tab_about.png", null, null),
                        Title = Platformer(null, "Home", "Home")
                        //Title = "home"
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Icon = Platformer("tab_about.png", null, null),
                        Title = Platformer(null, "Map", "Map")
                        //Title = "map"
                    },
                    new NavigationPage(new MenuPage())
                    {
                        Icon = Platformer("tab_about.png", null, null),
                        Title = Platformer(null, "News", "News")
                        //Title = "map"
                    },
                    new NavigationPage(new MenuPage())
                    {
                        Icon = Platformer("slideout.png", null, null),
                        Title = Platformer(null, "Menu", "Menu")
                        //Title = "map"
                    }
                }
            };

            if (Device.iOS == Device.RuntimePlatform)
            {
                //tabbedPage.BarBackgroundColor = Color.FromHex("#ffffff");
                //tabbedPage.BackgroundColor = Color.FromHex("#2b98f0");
                //tabbedPage.BarTextColor = Color.FromHex("#2b98f0");
            }      

            Current.MainPage = tabbedPage;

            tabbedPage.CurrentPageChanged += TabbedPage_CurrentPageChanged;

            //tabbedPage.CurrentPage = tabbedPage.Children[1];

        }

        private static void TabbedPage_CurrentPageChanged(object sender, System.EventArgs e)
        {
            if ((tabbedPage.CurrentPage as NavigationPage).CurrentPage.Title == "Map")
                ((tabbedPage.CurrentPage as NavigationPage).CurrentPage as AboutPage).Starter();
        }

        public static string Platformer(string ios, string android, string win)
        {
            string stringa = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    stringa = ios;
                    break;
                case Device.Android:
                    stringa = android;
                    break;
                case Device.WinPhone:
                    stringa = win;
                    break;
            }

            return stringa;
        }
    }
}
