using System;
using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace StritWalk.iOS
{
    public class CustomListViewRenderer : ListViewRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                //Control.Source = new NativeiOSListViewSource(e.NewElement as NativeListView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //if (e.PropertyName == NativeListView.ItemsProperty.PropertyName)
            //{
            //    Control.Source = new NativeiOSListViewSource(Element as NativeListView);
            //}
        }

 
    }
}
