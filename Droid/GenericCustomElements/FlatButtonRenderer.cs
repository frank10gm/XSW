using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FlatButton), typeof(FlatButtonRenderer))]
namespace StritWalk.Droid
{
    class FlatButtonRenderer : ButtonRenderer
    {
        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
        }
    }
}