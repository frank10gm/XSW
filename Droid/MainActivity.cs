using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using Xamarin.Forms.Platform.Android;
using Android.Util;


namespace StritWalk.Droid
{
    [Activity(Label = "StritWalk", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
        ScreenOrientation = ScreenOrientation.Portrait)]
    //WindowSoftInputMode = SoftInput.AdjustResize
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            //WindowSoftInputMode = SoftInput.AdjustResize            

            base.OnCreate(bundle);

            //Remove the status bar underlay in API 21+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.DecorView.SystemUiVisibility = 0;
                var statusBarHeightInfo = typeof(FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (statusBarHeightInfo != null)
                    statusBarHeightInfo.SetValue(this, 0);
                //Window.SetStatusBarColor(new Android.Graphics.Color(18, 52, 86, 255));
                //Window.SetStatusBarColor(new Android.Graphics.Color(25, 118, 210));
                Window.SetStatusBarColor(new Android.Graphics.Color(37, 41, 46));
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.FormsMaps.Init(this, bundle);
            //global::Xamarin.FormsGoogleMaps.Init(this, bundle);
            Toolkit.Init(this, bundle);

            //mokey robotics ble
            var a = new Robotics.Mobile.Core.Bluetooth.LE.Adapter();
            App.SetAdapter(a);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

    }
}
