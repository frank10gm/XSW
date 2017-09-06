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

namespace StritWalk.Droid
{
    class CustomClusterAdapter2 : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private View myContentsView;
        private ICluster clickedCluster;

        public CustomClusterAdapter2()
        {
            LayoutInflater inflater = LayoutInflater.From(Application.Context);
            myContentsView = inflater.Inflate(Resource.Layout.amu_info_window, null);
            //clickedCluster = cluster;
        }

        public IntPtr Handle;

        public void Dispose()
        {
            
        }

        public View GetInfoContents(Marker marker)
        {
            return null;
        }
        
        public View GetInfoWindow(Marker marker)
        {
            Console.WriteLine("@@@ dentro il getinfo");
            TextView tvTitle = ((TextView)myContentsView.FindViewById(Resource.Id.text));
            TextView tvSnippet = ((TextView)myContentsView.FindViewById(Resource.Id.text2));
            tvSnippet.Visibility = ViewStates.Gone;

            tvTitle.Text = " more posts";

            if (clickedCluster != null)
            {
                tvTitle.Text =  " more posts"; //clickedCluster.Items.Count.ToString() +
            }

            return myContentsView;
        }
    }
}