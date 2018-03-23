using System;
using Android.Content;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]

namespace StritWalk.Droid
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

			if (e.OldElement != null)
			{
			
			}

			if (e.NewElement != null)
			{
                Control.VerticalScrollBarEnabled = false;
			}
        }
    }
}
