﻿using StritWalk;
using StritWalk.iOS;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace StritWalk.iOS
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {

        //CustomListViewCell cell;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {

            //var customCell = (CustomViewCell)item;

            //cell = reusableCell as CustomListViewCell;

            //if (cell == null)
            //{
            //             cell = new CustomListViewCell(item.GetType().FullName, customCell);
            //}
            //else
            //{
            //             cell.CustomViewCell.PropertyChanged -= OnNativeCellPropertyChanged;
            //}

            //nativeCell.PropertyChanged += OnNativeCellPropertyChanged;

            var cell = base.GetCell(item, reusableCell, tv);
            if (cell != null)
            {
                // Disable native cell selection color style - set as *Transparent*
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.SelectedBackgroundView = null;
            }
            return cell;
        }


    }
}