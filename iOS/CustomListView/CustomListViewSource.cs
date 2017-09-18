using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System.Reflection;

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
        UITableView table;
        bool _keyOn = false;
		NSObject _keyboardShowObserver;
		NSObject _keyboardHideObserver;

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
            //Console.WriteLine("@@@ clear cached");
            this.cachedHeights.Clear();
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {
			//return tableItems.Count;
			table = tableview;
			
			//if (_keyboardShowObserver == null)
			//	_keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
			//if (_keyboardHideObserver == null)
				//_keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
            
            return source.RowsInSection(tableview, section);
        }

        #region user interaction methods

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cellForPath = GetCellForPath(indexPath);
            //listView.NotifyItemSelected(tableItems[indexPath.Row]);
            tableView.DeselectRow(indexPath, true);

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() => { tableView.EndEditing(true); });
            tableView.AddGestureRecognizer(gesture);
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {

        }

        #endregion


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            //var cell = this.GetCellForPath(indexPath);
            return source.GetCell(tableView, indexPath);
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            cachedHeights[indexPath.Row] = cell.Frame.Size.Height;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableView.AutomaticDimension;

            var cellForPath = this.GetCellForPath(indexPath);

            if (this.list.RowHeight == -1 && cellForPath.Height == -1.0 && cellForPath is CustomViewCell)
            {

                //var view = ((CustomViewCell)cellForPath).View;

                ////var sizeRequest = view.GetSizeRequest(tableView.Frame.Width, double.PositiveInfinity);
                //var sizeRequest = view.Measure(tableView.Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
                ////Console.WriteLine("@@@ " + sizeRequest);

                //cachedHeights[indexPath.Row] = sizeRequest.Request.Height;



                //return (nfloat)sizeRequest.Request.Height;
            }

            var renderHeight = cellForPath.RenderHeight;

            if (renderHeight <= 0.0)
                return 44;

            return (nfloat)renderHeight;
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            //if(cachedHeights.ContainsKey(indexPath.Row))
            //{
            //    Console.WriteLine("@@@ " + (nfloat)this.cachedHeights[indexPath.Row]);
            //}            
            return this.cachedHeights.ContainsKey(indexPath.Row) ? (nfloat)this.cachedHeights[indexPath.Row] : UITableView.AutomaticDimension;
        }

        private Cell GetCellForPath(NSIndexPath indexPath)
        {
            var templatedItemsList = (IReadOnlyList<Cell>)specialProperty.GetValue(this.list);

            //if (this.list.IsGroupingEnabled)
            //    templatedItemsList = (IReadOnlyList<Cell>)((IList)templatedItemsList)[indexPath.Section];

            return templatedItemsList[indexPath.Row];
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (table != null){
                table.EndEditing(true);
            }
        }

		protected virtual void OnKeyboardShow(NSNotification notification)
		{
            _keyOn = true;
		}

		protected virtual void OnKeyboardHide(NSNotification notification)
		{
            _keyOn = false;         
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
			//Console.WriteLine("### dispose " + _keyOn);
			//_keyOn = false;
			//if (_keyboardShowObserver != null)
			//{
			//	NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
			//	_keyboardShowObserver.Dispose();
			//	_keyboardShowObserver = null;
			//}

			//if (_keyboardHideObserver != null)
			//{
			//	NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
			//	_keyboardHideObserver.Dispose();
			//	_keyboardHideObserver = null;
			//}
        }

    }
}
