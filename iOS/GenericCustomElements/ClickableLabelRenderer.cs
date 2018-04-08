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
    //class ClickableLabelRenderer : ViewRenderer<Label, UITextView>
    class ClickableLabelRenderer : LabelRenderer
    {

        ClickableLabel label;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            label = Element as ClickableLabel;

            //ridurre testo
            var testoBase = Control.Text;            
            string temp = testoBase;
            string hash;
            string ch = "#";            
            int count = testoBase.Length - testoBase.Replace(ch, "").Length;
            int s;
            int f;

            s = testoBase.IndexOf("#");
            if (s != -1)
            {
                temp = testoBase.Substring(s);
                f = temp.IndexOf(" ");
                if (f != -1)
                    temp = temp.Substring(0, f);
                else
                    hash = temp.Substring(0, temp.Length);                
            }

            Console.WriteLine("@@@@ count: " + count);

            UITapGestureRecognizer tapgesture = new UITapGestureRecognizer(TextTap);
            Control.UserInteractionEnabled = true;
            Control.AddGestureRecognizer(tapgesture);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        bool ShouldInteractWithUrl(UITextView arg1, NSUrl arg2, NSRange arg3)
        {
            return false;
        }

        void TextTap(UITapGestureRecognizer tap)
        {
            var testo = Control.AttributedText;
            var touchPoint = tap.LocationOfTouch(0, Control);

            var textStorage = new NSTextStorage();
            var layoutMgr = new NSLayoutManager();
            var textContainer = new NSTextContainer(Control.Frame.Size);

            textStorage.SetString(Control.AttributedText);
            layoutMgr.AddTextContainer(textContainer);
            textStorage.AddLayoutManager(layoutMgr);
            textContainer.LineFragmentPadding = 0;

            var startRange = new NSRange
            {
                Location = 0,
                Length = 5
            };
            var range = new NSRange
            {
                Location = Control.Text.IndexOf("rankie"),
                Length = "rankie".Length
            };

            var glyphRange = layoutMgr.GlyphRangeForCharacterRange(startRange);
            CGRect glyphRect = (layoutMgr.BoundingRectForGlyphRange(glyphRange, textContainer));

            if (glyphRect.Contains(touchPoint))
            {
                Console.WriteLine(string.Format("@@@@ click : {0}", "profilo utente"));
            }
            else
            {
                //verifica se ho cliccato un hashtag o altro
                for (int i = 0; i < 10; i++)
                {
                    string chkText = "Piscia";
                    range = new NSRange
                    {
                        Location = Control.Text.IndexOf(chkText),
                        Length = chkText.Length
                    };
                    glyphRange = layoutMgr.GlyphRangeForCharacterRange(range);
                    glyphRect = (layoutMgr.BoundingRectForGlyphRange(glyphRange, textContainer));
                    Console.WriteLine(string.Format("@@@@ click : {0}", Control.Text.Length));

                    if (glyphRect.Contains(touchPoint))
                    {
                        Console.WriteLine(string.Format("@@@@ click : {0}", "tag"));
                        break;
                    }
                }
            }


        }


    }
}


//onelementchanged

//var view = (ClickableLabel)Element;
//if (view == null) return;

//var firstAttributes = new UIStringAttributes
//{
//    ForegroundColor = UIColor.Blue,
//    Link = new NSUrl("www.google.it")
//};
//var attributedString = new NSMutableAttributedString("@panita e molto #teerosa");
//attributedString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, 5));

//UITextView textView = new UITextView(new CGRect(0, 0, view.Width, view.Height));
////textView.Text = view.FormattedText.ToString();   
//textView.AttributedText = attributedString;

//textView.Font = UIFont.SystemFontOfSize((float)view.FontSize);
//textView.Editable = false;
//textView.Bounces = false;
//textView.Selectable = false;
//textView.UserInteractionEnabled = true;
//textView.ShouldInteractWithUrl = ShouldInteractWithUrl;
//UITapGestureRecognizer tapgesture = new UITapGestureRecognizer(() =>
//{
//    TextTap();
//});
//textView.AddGestureRecognizer(tapgesture);

//// Setting the data detector types mask to capture all types of link-able data
////textView.DataDetectorTypes = UIDataDetectorType.All;
//textView.BackgroundColor = UIColor.Clear;

//// overriding Xamarin Forms Label and replace with our native control
//SetNativeControl(textView);



//onelementproprtychanged
//var view = (ClickableLabel)Element;
//if (view == null) return;

//UITextView textView;

//if (Control == null)
//{
//    textView = new UITextView();
//    SetNativeControl(textView);
//}
//else if (e.PropertyName == Label.TextProperty.PropertyName)
//{
//    if (Element != null && !string.IsNullOrWhiteSpace(Element.Text))
//    {
//        var attr = new NSAttributedStringDocumentAttributes();
//        var nsError = new NSError();
//        attr.DocumentType = NSDocumentType.HTML;

//        var myHtmlData = NSData.FromString(view.Text, NSStringEncoding.Unicode);
//        Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
//    }
//}