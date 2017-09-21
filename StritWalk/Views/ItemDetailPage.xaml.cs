using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace StritWalk
{
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;
        public CustomTabbedPage page;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            CommentsListView.ItemTemplate = new DataTemplate(() =>
            {
                CustomViewCell cell = new CustomViewCell();

                var grid = new Grid();
                grid.Padding = 0;

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var postLabel = new Label { Margin = new Thickness(20, 10, 20, 20) };
                postLabel.SetBinding(Label.TextProperty, "Comment");
                grid.Children.Add(postLabel, 0, 0);

                cell.View = grid;
                //cell.Height = 234;                

                return cell;
            });

            page = Application.Current.MainPage as CustomTabbedPage;

            //        CommentEditor.TextChanged += (sender, e) =>
            //        {
            //if (CommentEditor.lineHeight != 0)
            //{
            //	double newHeight;
            //	int count = CommentEditor.Text.Count(c => c == '\n');
            //	if (count == 0)
            //	{
            //		newHeight = CommentEditor.lineHeight;
            //	}
            //	else
            //	{
            //		newHeight = ((CommentEditor.lineHeight) * (count + 1));
            //                    Console.WriteLine("### " + CommentEditor.Height);
            //	}
            //	if (CommentEditor.Height != newHeight)
            //	{
            //		CommentEditor.HeightRequest = newHeight;
            //	}
            //}
            //};

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void WillSparisci()
        {
            page.TabBarHidden = false;
        }

        void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {

            //if (CommentEditor.IsFocused)
            //CommentEditor.Unfocus();            		
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            CommentsListView.SelectedItem = null;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            CommentsListView.SelectedItem = null;
        }

    }
}
