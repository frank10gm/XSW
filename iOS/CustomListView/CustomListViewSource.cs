using System;
using System.Collections.Generic;
using System.Linq;
using StritWalk;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System;
using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Reflection;
using System.Collections.Specialized;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

using Foundation;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace StritWalk.iOS
{
    public class CustomListViewSource : UITableViewSource
    {
        // declare vars
        IList<Item> tableItems;
        CustomListView list;
        readonly NSString cellIdentifier = new NSString("TableCell");
        private readonly UITableViewSource source;
        private readonly Dictionary<int, double> cachedHeights = new Dictionary<int, double>();
        PropertyInfo specialProperty;        

        public IList<Item> ItemsSource
        {
            //get{ }
            set
            {
                tableItems = value.ToList();
            }
        }

        public CustomListViewSource(CustomListView view, UITableViewSource dataSource, PropertyInfo prop)
        {
            //tableItems = view.ItemsSource as ObservableRangeCollection<Item>;
            tableItems = view.ItemsSource as IList<Item>;
            list = view;
            source = dataSource;
            specialProperty = prop;
        }

        public void ClearRowHeightCache()
        {
            Console.WriteLine("@@@ clear cached");
            this.cachedHeights.Clear();
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
            //return tableItems.Count;
            return source.RowsInSection(tableview, section);
        }

        #region user interaction methods

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

        #endregion


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier) ;
            //var item = tableItems[indexPath.Row];


            //if (cell == null)
            //{
            //    cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
            //}

            //cell.TextLabel.Text = item.Creator;

            //return cell;

            //return GetCellInternal(tableView, indexPath);
            return source.GetCell(tableView, indexPath);
        }

        private UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
        {
            return source.GetCell(tableView, indexPath);
        }


        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var cellForPath = this.GetCellForPath(indexPath);

            if (this.list.RowHeight == -1 && cellForPath.Height == -1.0 && cellForPath is CustomViewCell)
            {

                //return UITableView.AutomaticDimension;

                var view = ((ViewCell)cellForPath).View;

                var sizeRequest = view.GetSizeRequest(tableView.Frame.Width, double.PositiveInfinity);

                cachedHeights[indexPath.Row] = sizeRequest.Request.Height;

                return (nfloat)sizeRequest.Request.Height;
            }

            var renderHeight = cellForPath.RenderHeight;

            if (renderHeight <= 0.0)
                return 44;

            return (nfloat)renderHeight;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            return this.cachedHeights.ContainsKey(indexPath.Row) ? (nfloat)this.cachedHeights[indexPath.Row] : 200;
        }

        private Cell GetCellForPath(NSIndexPath indexPath)
        {
            var templatedItemsList = (IReadOnlyList<Cell>)specialProperty.GetValue(this.list);

            if (this.list.IsGroupingEnabled)
                templatedItemsList = (IReadOnlyList<Cell>)((IList)templatedItemsList)[indexPath.Section];

            return templatedItemsList[indexPath.Row];
        }

    }
}
