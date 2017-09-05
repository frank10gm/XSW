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


[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace StritWalk.Droid
{
    public class CustomMapRenderer : MapRenderer, ClusterManager.IOnClusterClickListener
    {
        ClusterManager _clusterManager;
        bool _isDrawn;
        bool _mapReady;
        CustomMap _formsMap;

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

            _clusterManager.SetOnClusterClickListener(this);
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
                var t = i * Math.PI * 0.33f;
                var r = 0.005 * Math.Exp(0.1 * t);
                var x = r * Math.Cos(t);
                var y = r * Math.Sin(t);
                var item = new ClusterItem(lat + x, lng + y);
                items.Add(item);
            }
            _clusterManager.AddItems(items);
        }

        public bool OnClusterClick(ICluster p0)
        {
            Console.WriteLine("@@@ stai male");
        }
    }
}