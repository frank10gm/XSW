using System;
using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace StritWalk.iOS
{
    public class CustomListViewRenderer : ListViewRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            //Control.SeparatorInset = UIEdgeInsets.Zero;
            //Control.LayoutMargins = UIEdgeInsets.Zero;
            //Control.CellLayoutMarginsFollowReadableWidth = false;

            //Control.TableFooterView = new UIView();

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {

            }
        }

    }
}
