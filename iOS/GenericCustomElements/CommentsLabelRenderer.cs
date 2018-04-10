using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreGraphics;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(CommentsLabel), typeof(CommentsLabelRenderer))]
namespace StritWalk.iOS
{
    public class CommentsLabelRenderer : LabelRenderer
    {
        CommentsLabel _label;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            _label = e.NewElement as CommentsLabel;

            UITapGestureRecognizer tapgesture = new UITapGestureRecognizer(TextTap);
            Control.UserInteractionEnabled = true;
            Control.AddGestureRecognizer(tapgesture);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        void TextTap(UITapGestureRecognizer tap)
        {
            _label?.InvokeClicked(_label.ItemSelected);
        }
    }
}
