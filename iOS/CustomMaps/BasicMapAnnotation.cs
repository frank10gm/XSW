using System;
using UIKit;
using System.Collections.Generic;
using MapKit;
using CoreLocation;
using CoreGraphics;

namespace StritWalk.iOS
{
	class BasicMapAnnotation : MKAnnotation
	{
		private string _title;
        private string _subtitle;
		private CLLocationCoordinate2D _coordinate;

        public BasicMapAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle)
		{
			_coordinate = coordinate;
			_title = title;
            _subtitle = subtitle;
		}

		public override CLLocationCoordinate2D Coordinate
		{
			get
			{
				return _coordinate;
			}
		}

		public override string Title
		{
			get
			{
				return _title;
			}
		}

        public override string Subtitle { get { return _subtitle; } }
	}
}
