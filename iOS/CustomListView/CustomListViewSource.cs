using System;
using System.Collections.Generic;
using System.Linq;
using StritWalk;
using Foundation;
using UIKit;
namespace StritWalk.iOS
{
    public class CustomListViewSource : UITableViewSource
    {
        // declare vars
        ObservableRangeCollection<Item> tableItems;
        CustomListView listView;
        readonly NSString cellIdentifier = new NSString("TableCell");

        //      public IList<Item> Items
        //{
        //	//get{ }
        //	set
        //	{
        //		tableItems = value.ToList();
        //	}
        //}

        public CustomListViewSource(CustomListView view)
        {
            listView = view;
            tableItems = view.ItemsSource as ObservableRangeCollection<Item>;
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (tableItems != null)
                return tableItems.Count;
            else
                return 0;
        }

        //#region user interaction methods

        //public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    //listView.NotifyItemSelected(tableItems[indexPath.Row]);
        //    Console.WriteLine("Row " + indexPath.Row.ToString() + " selected");
        //    tableView.DeselectRow(indexPath, true);
        //}

        //public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    Console.WriteLine("Row " + indexPath.Row.ToString() + " deselected");
        //}

        //#endregion


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // request a recycled cell to save memory
            //CustomListViewCell cell = tableView.DequeueReusableCell(cellIdentifier) as CustomListViewCell;

            // if there are no cells to reuse, create a new one
            //if (cell == null)
            //{
            //    cell = new CustomListViewCell(cellIdentifier);
            //}

            //if (String.IsNullOrWhiteSpace(tableItems[indexPath.Row].ImageFilename))
            //{
            //	cell.UpdateCell(tableItems[indexPath.Row].Name
            //		, tableItems[indexPath.Row].Category
            //		, null);
            //}
            //else
            //{
            //	cell.UpdateCell(tableItems[indexPath.Row].Name
            //		, tableItems[indexPath.Row].Category
            //		, UIImage.FromFile("Images/" + tableItems[indexPath.Row].ImageFilename + ".jpg"));
            //}


            CustomListViewCell cell = tableView.DequeueReusableCell(cellIdentifier) as CustomListViewCell;
            //var item = tableItems[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            { 
                cell = new CustomListViewCell(cellIdentifier, cell); 
            }

            //cell.TextLabel.Text = item;

            return cell;

        }
    }
}
