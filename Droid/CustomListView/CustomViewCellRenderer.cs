using Android.Content;
using Android.Views;
using StritWalk.Droid;
using StritWalk;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;
using System;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]

namespace StritWalk.Droid
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {

		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
			var cell = base.GetCellCore(item, convertView, parent, context);
			var listView = parent as Android.Widget.ListView;

			if (listView != null)
			{
				// Disable native cell selection color style - set as *Transparent*
				listView.SetSelector(Android.Resource.Color.Transparent);
				listView.CacheColorHint = Android.Graphics.Color.Transparent;
			}

			return cell;
		}
    }
}
