using System;
using System.Collections.Generic;
using StritWalk.Views;
using Xamarin.Forms;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System.Diagnostics;

namespace StritWalk
{
    public partial class App : Application
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "https://www.hackweb.it/api";
        public static CustomTabbedPage tabbedPage;
        public static IDictionary<string, string> LoginParameters => null;
        static Robotics.Mobile.Core.Bluetooth.LE.IAdapter Adapter;
        public static INavigation AppNav
        {
            get; set;
        }

        public App()
        {
            InitializeComponent();
            Console.WriteLine("@@@@@@@ start app.");

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<CloudDataStore>();

            SetMainPage();
            //OneSignal.Current.SetLogLevel(LOG_LEVEL.DEBUG, LOG_LEVEL.DEBUG);
            OneSignal.Current.StartInit("31828451-4096-4355-8b1f-e54183b4a6c9")
                .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                .EndInit();
        }

        public static void SetMainPage()
        {
            if (!Settings.IsLoggedIn)
            {
                Current.MainPage = new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = (Color)Current.Resources["Sfondo2"],
                    BarTextColor = (Color)Current.Resources["Testo2"]
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
            OneSignal.Current.DeleteTags(new List<string>() { "UserId", "UserName" });
            OneSignal.Current.SetSubscription(false);
            //rimuovere id da server            

            Settings.UserId = "";
            Settings.AuthToken = "";
            Settings.Num_friends = 0;
            Settings.Num_likes = 0;
            Settings.Num_posts = 0;

            Current.MainPage = new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = (Color)Current.Resources["Sfondo2"],
                BarTextColor = (Color)Current.Resources["Testo2"]
            };
        }

        public static void GoToMainPage()
        {
            var itemsPage = new ItemsPage();

            //NavigationPage.SetHasNavigationBar(itemsPage, false);

            tabbedPage = new CustomTabbedPage
            {
                Children = {
                    new NavigationPage(itemsPage)
                    {
                        Icon = Platformer("tab_feed.png", null, null),
                        Title = Platformer(null, "Feed", "Feed")
                        //Title = "home"
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Icon = Platformer("tab_about.png", null, null),
                        Title = Platformer(null, "Map", "Map")
                        //Title = "map"
                    },
                    //new NavigationPage(new MonkeyPage())
                    //{
                    //    Icon = Platformer("tab_feed.png", null, null),
                    //    Title = Platformer(null, "Monkeys", "Monkeys")
                    //    //Title = "map"
                    //},
                    new NavigationPage(new MenuPage(Adapter))
                    {
                        Icon = Platformer("slideout.png", null, null),
                        Title = Platformer(null, "Profile", "Profile")
                        //Title = "map"
                    }
                }
            };

            if (Device.iOS == Device.RuntimePlatform)
            {
                tabbedPage.BarBackgroundColor = (Color)Current.Resources["Sfondo4"];
                tabbedPage.BarTextColor = (Color)Current.Resources["Testo2"];
            }
            else
            {
                tabbedPage.BarBackgroundColor = (Color)Current.Resources["Sfondo3"];
            }

            //NavigationPage.SetHasNavigationBar(tabbedPage, false);
            //Current.MainPage = new NavigationPage(tabbedPage);
            //AppNav = tabbedPage.Navigation;
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

        public static void SetAdapter(Robotics.Mobile.Core.Bluetooth.LE.IAdapter adapter)
        {
            Adapter = adapter;
        }

        protected override void OnResume()
        {
            MessagingCenter.Send(this, "OnResume", "with success");
        }
    }
}
