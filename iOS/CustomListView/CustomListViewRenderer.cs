using System;
using StritWalk;
using StritWalk.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Reflection;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace StritWalk.iOS
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        private CustomListViewSource dataSource;
        private static readonly PropertyInfo ListViewTemplatedItemsPropertyInfo;
        INotifyCollectionChanged templatedItemsList;

        static CustomListViewRenderer()
        {
            ListViewTemplatedItemsPropertyInfo = typeof(ListView).GetProperty("TemplatedItems", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
        }

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
                templatedItemsList = (INotifyCollectionChanged)ListViewTemplatedItemsPropertyInfo.GetValue(e.NewElement);
                templatedItemsList.CollectionChanged -= this.OnCollectionChanged;
            }

            if (e.NewElement != null)
            {                
                Control.ShowsVerticalScrollIndicator = true;                
                templatedItemsList = (INotifyCollectionChanged)ListViewTemplatedItemsPropertyInfo.GetValue(e.NewElement);                
                templatedItemsList.CollectionChanged += this.OnCollectionChanged;
                Control.Source = dataSource = new CustomListViewSource(Element as CustomListView, Control.Source, ListViewTemplatedItemsPropertyInfo);
                UpdateRowHeight();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Control.Source = dataSource = new CustomListViewSource(Element as CustomListView, Control.Source, ListViewTemplatedItemsPropertyInfo);
            ClearRowHeightCache();
        }

        private void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;
            Control.EstimatedRowHeight = 200;
            Control.RowHeight = rowHeight <= 0 ? 44 : rowHeight;
        }

        private void ClearRowHeightCache()
        {
            if (dataSource != null)
                dataSource.ClearRowHeightCache();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                ClearRowHeightCache();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Element != null)
            {
                templatedItemsList = (INotifyCollectionChanged)ListViewTemplatedItemsPropertyInfo.GetValue(Element);

                templatedItemsList.CollectionChanged -= OnCollectionChanged;
            }
        }
       

    }

}
