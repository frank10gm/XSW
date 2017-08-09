using System;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
namespace StritWalk.iOS.CustomMaps.Cluster
{
	public class FBAnnotationCluster : MKAnnotation
	{
		private CLLocationCoordinate2D _coordinate;

		public FBAnnotationCluster(CLLocationCoordinate2D coordinate)
		{
			_coordinate = coordinate;
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
