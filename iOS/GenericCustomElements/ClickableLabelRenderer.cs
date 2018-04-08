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

[assembly: ExportRenderer(typeof(ClickableLabel), typeof(ClickableLabelRenderer))]
namespace StritWalk.iOS
{
    class ClickableLabelRenderer : ViewRenderer<Label, UITextView>
    {   
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (ClickableLabel)Element;
            if (view == null) return;

            var firstAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.Blue,                                
                Link = new NSUrl("www.google.it")
            };
            var attributedString = new NSMutableAttributedString(" <a href= http://www.google.com>Visit Google</a> ");
            attributedString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, 5));

            UITextView textView = new UITextView(new CGRect(0, 0, view.Width, view.Height));
            //textView.Text = view.FormattedText.ToString();   
            textView.AttributedText = attributedString;
            
                    
            //Console.WriteLine(string.Format("@@@@ text : {0}", view.Text));
            textView.Font = UIFont.SystemFontOfSize((float)view.FontSize);
            textView.Editable = false;
            textView.Bounces = false;
            textView.Selectable = true;
            textView.UserInteractionEnabled = true;            
            textView.ShouldInteractWithUrl = ShouldInteractWithUrl;

            // Setting the data detector types mask to capture all types of link-able data
            textView.DataDetectorTypes = UIDataDetectorType.All;
            textView.BackgroundColor = UIColor.Clear;

            // overriding Xamarin Forms Label and replace with our native control
            SetNativeControl(textView);
        }

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    var view = (ClickableLabel)Element;
        //    if (view == null) return;

        //    UITextView textView;

        //    if (Control == null)
        //    {
        //        textView = new UITextView();
        //        SetNativeControl(textView);
        //        textView.ShouldInteractWithUrl = ShouldInteractWithUrl;
        //    }
        //    else if (e.PropertyName == Label.TextProperty.PropertyName)
        //    {
        //        if (Element != null && !string.IsNullOrWhiteSpace(Element.Text))
        //        {
        //            var attr = new NSAttributedStringDocumentAttributes();
        //            var nsError = new NSError();
        //            attr.DocumentType = NSDocumentType.HTML;

        //            var myHtmlData = NSData.FromString(view.Text, NSStringEncoding.Unicode);
        //            Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
        //        }
        //    }
        //}

        bool ShouldInteractWithUrl(UITextView arg1, NSUrl arg2, NSRange arg3)
        {
            Console.WriteLine("@@@ pannita tesoroosa");
            return true;
        }
    }
}