using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.Connectivity;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
//using Xamarin.Forms.GoogleMaps;
//using Xamarin.Forms.GoogleMaps.Bindings;
//using TK.CustomMap;

namespace StritWalk
{
    public partial class AboutPage : ContentPage
    {
        CustomMap map;
        IIBeaconService locationTracker;
        Position position;
        Button button;
        bool start;
        bool first_starter;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        AbsoluteLayout layout;

        public AboutPage()
        {
            InitializeComponent();
            layout = new AbsoluteLayout() { };
            Content = layout;

            if (CrossConnectivity.Current.IsConnected && locationTracker == null)
            {
                getMap();
            }
        }

        void positionClicked(object sender, EventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(50)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (start && CrossConnectivity.Current.IsConnected && locationTracker != null)
            {
                locationTracker.StartTracking();
                map.IsShowingUser = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (locationTracker != null)
            {
                locationTracker.PauseTracking();
                map.IsShowingUser = false;
            }
        }

        void OnLocationTracker(object sender, GeographicLocation args)
        {
            Debug.WriteLine("locating...");

            position = new Position(args.Latitude, args.Longitude);
            Settings.lat = position.Latitude.ToString().Replace(",", "."); ;
            Settings.lng = position.Longitude.ToString().Replace(",", ".");

            if (!first_starter)
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(50)));

            first_starter = true;
        }

        async void getMap()
        {
            var items = await DataStore.GetMapItemsAsync(true);

            map = new CustomMap
            {
                MapType = MapType.Street,
                IsShowingUser = false
            };

            foreach (var p in items)
            {
                var pin = new Pin
                {
                    Type = PinType.Place,
                    //Position = new Position(Convert.ToDouble(p.Lat.Replace(".", ",")), Convert.ToDouble(p.Lng.Replace(".", ","))),
                    Position = new Position(double.Parse(p.Lat, System.Globalization.CultureInfo.InvariantCulture), double.Parse(p.Lng, System.Globalization.CultureInfo.InvariantCulture)),
                    Label = p.Name + " (" + p.Creator + ")",
                    Address = p.Added + "ago"
                };
                //map.Pins.Add(pin);
                map.PinList.Add(pin);
            }

            AbsoluteLayout.SetLayoutBounds(map, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(map, AbsoluteLayoutFlags.All);
            layout.Children.Add(map);

            button = new Button
            {
                //Text = "Click Me!",
                //Font = Font.SystemFontOfSize(NamedSize.Large),
                //BorderWidth = 1,
                //HorizontalOptions = LayoutOptions.Center,
                //VerticalOptions = LayoutOptions.CenterAndExpand
                Image = "profile_generic.png",
                Margin = new Thickness(0, 0, 0, 0)
            };
            button.Clicked += positionClicked;

            AbsoluteLayout.SetLayoutBounds(button, new Rectangle(.95, .95, 30, 30));
            AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);

            //AbsoluteLayout.SetLayoutBounds(map, new Rectangle(0, 0, 1, 1));
            //AbsoluteLayout.SetLayoutFlags(map, AbsoluteLayoutFlags.All);
            //layout.Children.Add(map);

            if (Device.Android != Device.RuntimePlatform)
                layout.Children.Add(button);

            position = new Position(double.Parse(Settings.lat, System.Globalization.CultureInfo.InvariantCulture), double.Parse(Settings.lng, System.Globalization.CultureInfo.InvariantCulture));

            locationTracker = DependencyService.Get<IIBeaconService>();
            locationTracker.LocationChanged += OnLocationTracker;
            locationTracker.SingleTracking();

            //if (!first_starter)
                //locationTracker.StartTracking();

            //map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(2)));

            start = true;
        }

    }
}
