using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.Views;
using StritWalk.iOS;
using CoreGraphics;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using CoreBluetooth;
using CoreFoundation;
using AVFoundation;
using MultipeerConnectivity;

[assembly: ExportRenderer(typeof(MonkeyPage), typeof(MonkeyPageRenderer))]

namespace StritWalk.iOS
{
    public class MonkeyPageRenderer : PageRenderer
    {


        public MonkeyPageRenderer()
        {

        }

        protected override void OnElementChanged(VisualElementChangedEventArgs el)
        {
            base.OnElementChanged(el);

            if (el.OldElement != null || Element == null)
            {
                return;
            }

        }

    }
}
