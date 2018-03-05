using StritWalk;
using StritWalk.iOS;
using System;
using System.Diagnostics;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;

[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewCellRenderer))]
namespace StritWalk.iOS
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {

        //CustomListViewCell cell;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            var xcell = item as CustomViewCell;

            //var customViewCell = (CustomViewCell)item;
            //cell = reusableCell as CustomListViewCell;

            //if (cell == null)
            //{
            //    cell = new CustomListViewCell(item.GetType().FullName, customViewCell);
            //}
            //else
            //{
            //    //cell.NativeCell.PropertyChanged -= OnNativeCellPropertyChanged;
            //}

            // //nativeCell.PropertyChanged += OnNativeCellPropertyChanged;

            // //cell.UpdateCell(nativeCell);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            try
            {
                UILabel comments = cell.Subviews[0].Subviews[0].Subviews[3].Subviews[0] as UILabel;
                comments.UserInteractionEnabled = true;
                UITapGestureRecognizer tapgesture = new UITapGestureRecognizer(() =>
                {
                    xcell.InvokeTap();
                });
                comments.AddGestureRecognizer(tapgesture);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);   
            }

            return cell;
        }

    }
}
