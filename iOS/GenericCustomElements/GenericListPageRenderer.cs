using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using CoreGraphics;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ItemsPage), typeof(GenericListPageRenderer))]
namespace StritWalk.iOS
{
    public class GenericListPageRenderer : PageRenderer
    {
        public GenericListPageRenderer()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            //controllo della tastiera
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() => { View.EndEditing(true); });
            View.AddGestureRecognizer(gesture);
        }
    }
}
