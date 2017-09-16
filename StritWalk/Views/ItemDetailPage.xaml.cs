using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

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

                var postLabel = new Label { Margin = new Thickness(20, 10, 20, 20)};
                postLabel.SetBinding(Label.TextProperty, "Comment");
				grid.Children.Add(postLabel, 0, 0);				

				cell.View = grid;
				//cell.Height = 234;                

				return cell;
            });

        }
    }
}
