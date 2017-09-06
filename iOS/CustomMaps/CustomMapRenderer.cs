using CoreLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MapKit;
using StritWalk;
using StritWalk.iOS;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using StritWalk.iOS.CustomMaps.Cluster;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace StritWalk.iOS
{
	public class CustomMapRenderer : MapRenderer
	{
        //cluster
		private const string kClusterAnnotationId = "REUSABLE_CLUSTER_ANNOTATION_ID";
		private const string kPinAnnotationId = "REUSABLE_PIN_ANNOTATION_ID";
        private const int kTagClusterLabel = 1;
        FBClusteringManager clusteringManager;

        //other
		UIView customPinView;
		//List<CustomPin> customPins;
		MKPolygonRenderer polygonRenderer;
        MKMapView nativeMap;

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				nativeMap = Control as MKMapView;
				if (nativeMap != null)
				{
					nativeMap.RemoveOverlays(nativeMap.Overlays);
					nativeMap.OverlayRenderer = null;
					polygonRenderer = null;
                    nativeMap.ShowsCompass = true;
                    nativeMap.UserTrackingMode = MKUserTrackingMode.FollowWithHeading;
				}
			}

			if (e.NewElement != null)
			{
				var formsMap = (CustomMap)e.NewElement;
				nativeMap = Control as MKMapView;

				nativeMap.OverlayRenderer = GetOverlayRenderer;

				CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[formsMap.ShapeCoordinates.Count];

                System.Diagnostics.Debug.WriteLine("shape coords : " + formsMap.ShapeCoordinates.Count);

				int index = 0;
				foreach (var position in formsMap.ShapeCoordinates)
				{
					coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
					index++;
				}

				var blockOverlay = MKPolygon.FromCoordinates(coords);
				nativeMap.AddOverlay(blockOverlay);

                //dev10n
				List<IMKAnnotation> listAnnotations = new List<IMKAnnotation>();
				
                Random rnd = new Random();
				for (int i = 1; i <= 500; i++)
				{
					double lat = 51.7 + rnd.NextDouble();
					double lon = 4.3 + rnd.NextDouble();
					//var annotation = new BasicMapAnnotation(new CLLocationCoordinate2D(lat, lon), "Marker " + i, "unca");
                    //var annotation = new MKAnnotation()
					//listAnnotations.Add(annotation);
				}

                foreach (var p in formsMap.PinList){
                    var annotation = new MKPointAnnotation
                    {
                        Coordinate = new CLLocationCoordinate2D(p.Position.Latitude, p.Position.Longitude),
                        Title = p.Label,
                        Subtitle = p.Address
                    };
                    listAnnotations.Add(annotation);
                }

				//CLLocationCoordinate2D center = new CLLocationCoordinate2D(52.2, 4.8);
				//MKCoordinateRegion region = MKCoordinateRegion.FromDistance(center, 200000, 200000);
				//nativeMap.SetRegion(region, false);

				clusteringManager = new FBClusteringManager(listAnnotations);

				nativeMap.RegionChanged += OnClusterChange;
				nativeMap.GetViewForAnnotation += GetClusterView;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;

			}


			//if (e.OldElement != null)
			//{
			//	var nativeMap = Control as MKMapView;
			//	if (nativeMap != null)
			//	{
			//		nativeMap.RemoveAnnotations(nativeMap.Annotations);
			//		//nativeMap.GetViewForAnnotation = null;
			//		nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
			//		//nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
			//		//nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
			//	}
			//}

			//if (e.NewElement != null)
			//{
			//	var formsMap = (CustomMap)e.NewElement;
			//	var nativeMap = Control as MKMapView;
			//	customPins = formsMap.CustomPins;

			//	//nativeMap.GetViewForAnnotation = GetViewForAnnotation;
			//	nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
			//	//nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
			//	//nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
			//}
		}

		MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
		{
			if (polygonRenderer == null && !Equals(overlayWrapper, null))
			{
				var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
				polygonRenderer = new MKPolygonRenderer(overlay as MKPolygon)
				{
					FillColor = UIColor.Red,
					StrokeColor = UIColor.Blue,
					Alpha = 0.4f,
					LineWidth = 9
				};
			}
			return polygonRenderer;
		}

		//MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		//{
		//	MKAnnotationView annotationView = null;

		//	if (annotation is MKUserLocation)
		//		return null;

		//	var anno = annotation as MKPointAnnotation;
		//	var customPin = GetCustomPin(anno);
		//	if (customPin == null)
		//	{
		//		throw new Exception("Custom pin not found");
		//	}

		//	annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
		//	if (annotationView == null)
		//	{
		//		annotationView = new CustomMKAnnotationView(annotation, customPin.Id)
		//		{
		//			Image = UIImage.FromFile("pin.png"),
		//			CalloutOffset = new CGPoint(0, 0),
		//			LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png")),
		//			RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure)
		//		};
		//		((CustomMKAnnotationView)annotationView).Id = customPin.Id;
		//		((CustomMKAnnotationView)annotationView).Url = customPin.Url;
		//	}
		//	annotationView.CanShowCallout = true;
		//	return annotationView;
		//}

		void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
		{
			var customView = e.View as CustomMKAnnotationView;
			if (!string.IsNullOrWhiteSpace(customView.Url))
			{
				UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
			}
		}

		void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
		{
            //System.Diagnostics.Debug.WriteLine("gino");
			//var customView = e.View as CustomMKAnnotationView;
			//customPinView = new UIView();

			//if (customView.Id == "Xamarin")
			//{
			//	customPinView.Frame = new CGRect(0, 0, 200, 84);
			//	var image = new UIImageView(new CGRect(0, 0, 200, 84));
			//	image.Image = UIImage.FromFile("xamarin.png");
			//	customPinView.AddSubview(image);
			//	customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
			//	e.View.AddSubview(customPinView);
			//}
		}

		void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
		{
			if (!e.View.Selected)
			{
				customPinView.RemoveFromSuperview();
				customPinView.Dispose();
				customPinView = null;
			}
		}

        void OnClusterChange(object sender, MKMapViewChangeEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("regionchanged");
            double scale = nativeMap.Bounds.Size.Width / nativeMap.VisibleMapRect.Size.Width;
            List<IMKAnnotation> annotationsToDisplay = clusteringManager.ClusteredAnnotationsWithinMapRect(nativeMap.VisibleMapRect, scale);
            clusteringManager.DisplayAnnotations(annotationsToDisplay, nativeMap);
        }

        MKAnnotationView GetClusterView(MKMapView mapView, IMKAnnotation annotation)
        {
            //System.Diagnostics.Debug.WriteLine("getview");
            MKAnnotationView anView;

            if (annotation is FBAnnotationCluster)
            {
                //System.Diagnostics.Debug.WriteLine("cluview?ornot");
                FBAnnotationCluster annotationcluster = (FBAnnotationCluster)annotation;
                anView = (MKAnnotationView)mapView.DequeueReusableAnnotation(kClusterAnnotationId);

                UILabel label = null;
                if (anView == null)
                {
                    // nicely format the cluster icon and display the number of items in it
                    anView = new MKAnnotationView(annotation, kClusterAnnotationId);
                    anView.Image = UIImage.FromBundle("cluster");
                    //anView.Image = UIImage.FromFile("cluster.png");
                    label = new UILabel(new CGRect(0, 0, anView.Image.Size.Width, anView.Image.Size.Height));
                    label.Tag = kTagClusterLabel;
                    label.TextAlignment = UITextAlignment.Center;
                    label.TextColor = UIColor.White;
                    anView.AddSubview(label);

                    anView.CanShowCallout = true;
                }
                else
                {
                    label = (UILabel)anView.ViewWithTag(kTagClusterLabel);
                }

                label.Text = annotationcluster.Annotations.Count.ToString();
                return anView;
            }
            return null;
        }


        double GetZoom()
        {
            var angleCamera = nativeMap.Camera.Heading;
            if(angleCamera > 270)
            {
                angleCamera = 360 - angleCamera;
            }
            else if(angleCamera > 90)
            {
                angleCamera = Math.Abs(angleCamera - 180);
            }
            var angleRad = Math.PI * angleCamera / 180;
            double width = (double)nativeMap.Frame.Size.Width;
            double height = (double)nativeMap.Frame.Size.Height;
            double heightOffset = 20;   
            var spanStraight = width * nativeMap.Region.Span.LongitudeDelta / (width * Math.Cos(angleRad) + (height - heightOffset) * Math.Sin(angleRad));
            return Math.Log(360 * ((width / 256) / spanStraight)) + 1;
            //return Math.Log(360 * ((width / 256) / nativeMap.Region.Span.LongitudeDelta));
        }

		//CustomPin GetCustomPin(MKPointAnnotation annotation)
		//{
		//	var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
		//	return customPins.FirstOrDefault(pin => pin.Pin.Position == position);
		//}


	}
}