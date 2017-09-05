using System;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
namespace StritWalk.iOS.CustomMaps.Cluster
{
	public class FBAnnotationCluster : MKAnnotation
	{
		private CLLocationCoordinate2D _coordinate;
		string title, subtitle;
		public override string Title { get { return title; } }
		public override string Subtitle { get { return subtitle; } }

		public FBAnnotationCluster(CLLocationCoordinate2D coordinate, string _title)
		{
			_coordinate = coordinate;
            title = _title;
            subtitle = "click here to view them all";
		}

		public override CoreLocation.CLLocationCoordinate2D Coordinate
		{
			get
			{
				return _coordinate;
			}
		}

		public List<IMKAnnotation> Annotations;
	}
}
