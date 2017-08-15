using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using System.Collections.Generic;
using Xamarin.Forms.Maps;


[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace StritWalk.Droid
{
	public class CustomMapRenderer : MapRenderer
	{
		//GoogleMap map;
		//List<Position> shapeCoordinates;

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe
			}

			if (e.NewElement != null)
			{
				var formsMap = (CustomMap)e.NewElement;

				((MapView)Control).GetMapAsync(this);
			}
		}

	}
}