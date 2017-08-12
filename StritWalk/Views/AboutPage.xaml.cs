using System;
using System.Collections.Generic;
using Plugin.Connectivity;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
//using TK.CustomMap;

namespace StritWalk
{
    public partial class AboutPage : ContentPage
    {

        CustomMap map;
        ILocationTracker locationTracker;
        Position position;
        Button button;
        bool start = true;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        AbsoluteLayout layout;

        public AboutPage()
        {
            InitializeComponent();

            //map = new CustomMap
            //{
            //    MapType = MapType.Street,
            //    IsShowingUser = true
            //};

            if (CrossConnectivity.Current.IsConnected)
                getPins();

            //map.ShapeCoordinates.Add(new Position(37.797513, -122.402058));
            //map.ShapeCoordinates.Add(new Position(37.798433, -122.402256));
            //map.ShapeCoordinates.Add(new Position(37.798582, -122.401071));
            //map.ShapeCoordinates.Add(new Position(37.797658, -122.400888));

            //map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.79752, -122.40183), Distance.FromMiles(0.1)));

            //             (
            //MapSpan.FromCenterAndRadius(
            //                 new Position(44, 12.6), Distance.FromKilometers(2)))
            //{
            //	IsShowingUser = true,
            //	//HeightRequest = 100,
            //	//WidthRequest = 960,
            //	//VerticalOptions = LayoutOptions.FillAndExpand
            //};
            layout = new AbsoluteLayout { };

            var blueBox = new BoxView
            {
                Color = Color.Blue,
                //VerticalOptions = LayoutOptions.FillAndExpand,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //HeightRequest = 75
            };
            AbsoluteLayout.SetLayoutBounds(blueBox, new Rectangle(.9, .9, 10, 10));
            AbsoluteLayout.SetLayoutFlags(blueBox, AbsoluteLayoutFlags.PositionProportional);

            Content = layout;
        }

        void positionClicked(object sender, EventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(2)));
        }

        void OnLocationTracker(object sender, GeographicLocation args)
        {
            position = new Position(args.Latitude, args.Longitude);

            //prima posizione all'apertura della pagina della mappa
            if (start)
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(2)));
                start = false;
                //getPins();
            }

            //myLocationButton.IsVisible = Device.OS != TargetPlatform.Android;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    break;
                default:
                    break;
            }

            if (!start)
            {
                if (CrossConnectivity.Current.IsConnected && locationTracker != null)
                    locationTracker.StartTracking();
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                    getPins();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    break;
                default:
                    break;
            }

            if (CrossConnectivity.Current.IsConnected && locationTracker != null)
                locationTracker.PauseTracking();
        }

        async void getPins()
        {
            var items = await DataStore.GetMapItemsAsync(true);

            map = new CustomMap
            {
                MapType = MapType.Street,
                IsShowingUser = true
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
                map.Pins.Add(pin);
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
                Image = "xamarin_logo.png",
                Margin = new Thickness(0, 0, 0, 0)
            };
            button.Clicked += positionClicked;

            AbsoluteLayout.SetLayoutBounds(button, new Rectangle(.9, .98, 100, 30));
            AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);

            //AbsoluteLayout.SetLayoutBounds(map, new Rectangle(0, 0, 1, 1));
            //AbsoluteLayout.SetLayoutFlags(map, AbsoluteLayoutFlags.All);
            //layout.Children.Add(map);

            if (Device.Android != Device.RuntimePlatform)
                layout.Children.Add(button);

            //location gestione
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    break;
                default:
                    break;
            }

            locationTracker = DependencyService.Get<ILocationTracker>();
            locationTracker.LocationChanged += OnLocationTracker;
            locationTracker.StartTracking();
        }

    }
}
