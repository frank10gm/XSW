using System;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]

namespace StritWalk.Droid
{
    public class CustomListViewRenderer : ListViewRenderer
    {
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
