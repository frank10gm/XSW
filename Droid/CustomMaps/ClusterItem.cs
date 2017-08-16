using System;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering;
using StritWalk;
using StritWalk.Droid;

namespace StritWalk.Droid
{
	public class ClusterItem : Java.Lang.Object, IClusterItem
	{
		public ClusterItem()
		{
		}
		public LatLng Position { get; set; }

        public String Snippet { get; set; }

        public String Title { get; set; }

        public ClusterItem(double lat, double lng)
		{
			Position = new LatLng(lat, lng);
		}

    }
}