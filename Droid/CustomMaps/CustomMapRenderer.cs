using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using System.Collections.Generic;
using Com.Google.Maps.Android.Clustering;
using System;

using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Com.Google.Maps.Android.Clustering.View;
using Android.Content;
using Android.Views;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace StritWalk.Droid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Com.Google.Maps.Android.Clustering;

    public class CustomMapRenderer : MapRenderer, ClusterManager.IOnClusterClickListener, ClusterManager.IOnClusterInfoWindowClickListener
    {
        ClusterManager _clusterManager;
        bool _isDrawn;
        bool _mapReady;
        CustomMap _formsMap;
        public static ICluster clickedCluster;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                _formsMap = (CustomMap)e.NewElement;
                ((MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion") && !_isDrawn)
            {
                OnGMapReady();
                _isDrawn = true;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                _isDrawn = false;
            }
        }

        public void OnGMapReady()
        {
            if (_mapReady) return;

            _clusterManager = new ClusterManager(Context, NativeMap);
            _clusterManager.Renderer = new CustomClusterRenderer(Context, NativeMap, _clusterManager);
            //NativeMap.SetOnCameraChangeListener(_clusterManager);

            _clusterManager.SetOnClusterClickListener(this);
            _clusterManager.SetOnClusterInfoWindowClickListener(this);
            NativeMap.SetInfoWindowAdapter(_clusterManager.MarkerManager);
            _clusterManager.ClusterMarkerCollection.SetOnInfoWindowAdapter(new CustomClusterAdapter(Context));

            //_clusterManager.SetOnClusterItemClickListener(this);
            NativeMap.SetOnCameraIdleListener(_clusterManager);
            NativeMap.SetOnMarkerClickListener(_clusterManager);

            //inserimento dei pin nel cluster
            var items = new List<ClusterItem>();

            foreach (var p in _formsMap.PinList)
            {
                var item = new ClusterItem(p.Position.Latitude, p.Position.Longitude);
                item.Title = p.Label;
                item.Snippet = p.Address;
                items.Add(item);
            }

            _clusterManager.AddItems(items);            
            

            //this.AddClusterItems();

            _mapReady = true;

        }

        private void AddClusterItems()
        {
            double lat = 47.59978;
            double lng = -122.3346;

            var items = new List<ClusterItem>();

            // Create a log. spiral of markers to test clustering
            for (int i = 0; i < 20; ++i)
            {
                var t = i * System.Math.PI * 0.33f;
                var r = 0.005 * System.Math.Exp(0.1 * t);
                var x = r * System.Math.Cos(t);
                var y = r * System.Math.Sin(t);
                var item = new ClusterItem(lat + x, lng + y);
                items.Add(item);
            }
            _clusterManager.AddItems(items);
        }

        public bool OnClusterClick(ICluster p0)
        {
            clickedCluster = p0;
            return false;
        }

        public void OnClusterInfoWindowClick(ICluster p0)
        {
            
        }

        public class CustomClusterRenderer : DefaultClusterRenderer
        {
            public CustomClusterRenderer(Context p0, GoogleMap p1, ClusterManager p2) : base(p0, p1, p2)
            {
                
            }

            protected override void OnBeforeClusterItemRendered(Java.Lang.Object p0, MarkerOptions p1)
            {
                base.OnBeforeClusterItemRendered(p0, p1);
            }

            protected override void OnClusterItemRendered(Java.Lang.Object p0, Marker p1)
            {
                base.OnClusterItemRendered(p0, p1);
            }
        }


        class CustomClusterAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
        {
            private View myContentsView;
            //private ICluster clickedCluster;

            public CustomClusterAdapter(Context context)
            {
                //LayoutInflater inflater = LayoutInflater.From(context);
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                //var layout = inflater.Inflate(Resource.Layout.amu_info_window, null) as LinearLayout;
                myContentsView = inflater.Inflate(Resource.Layout.amu_info_window, null);
                //clickedCluster = cluster;
                Console.WriteLine("@@@ custom cluster");
            }

            View GoogleMap.IInfoWindowAdapter.GetInfoContents(Marker marker) => null;

            Android.Views.View GoogleMap.IInfoWindowAdapter.GetInfoWindow(Marker marker)
            {
                Console.WriteLine("@@@ dentro il getinfo");
                TextView tvTitle = ((TextView)myContentsView.FindViewById(Resource.Id.amu_text));
                //TextView tvSnippet = ((TextView)myContentsView.FindViewById(Resource.Id.text2));
                ////tvSnippet.Visibility = ViewStates.Gone;
                //tvTitle.SetText("ok", TextView.BufferType.Normal);
                tvTitle.Text = " more posts";                

                //if (clickedCluster != null)
                //{
                //    tvTitle.Text = " more posts"; //clickedCluster.Items.Count.ToString() +
                //}

                return myContentsView;
            }
        }

    }
}