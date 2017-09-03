using StritWalk;
using StritWalk.iOS;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace StritWalk.iOS
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);

            if (cell != null)
            {
				// Disable native cell selection color style - set as *Transparent*

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                //cell.SelectedBackgroundView
            }

            tv.RowHeight = UITableView.AutomaticDimension;
            tv.EstimatedRowHeight = 200;            

            return cell;
        }

        

    }
}
