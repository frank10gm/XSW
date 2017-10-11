using Android.Content;
using Android.Views;
using StritWalk.Droid;
using StritWalk;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;
using System;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]

namespace StritWalk.Droid
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {

		protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		{
            var cell = base.GetCellCore(item, convertView, parent, context);
			var listView = parent as Android.Widget.ListView;
            var xcell = item as CustomViewCell;
            var cellview = cell as ViewGroup;

			if (listView != null)
			{
				// Disable native cell selection color style - set as *Transparent*
				listView.SetSelector(Android.Resource.Color.Transparent);
				listView.CacheColorHint = Android.Graphics.Color.Transparent;
			}

			try
			{                
                var child1 = parent.GetChildAt(0) as ViewGroup;
                var child2 = child1.GetChildAt(0) as ViewGroup;
                var child3 = child2.GetChildAt(3) as ViewGroup;
                //var comments = child3.GetChildAt(0) as FormsTextView;
                Debug.WriteLine("xxx " + child2);
                //comments.Clickable = true;
                //comments.Click += (sender, e) => {
                //    xcell.InvokeTap();
                //    Debug.WriteLine("xxx tapped");
                //};			
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return cell;
		}
    }
}
