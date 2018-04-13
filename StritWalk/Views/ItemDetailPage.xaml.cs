using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            viewModel.listView = CommentsListView;

            CommentsListView.ItemTemplate = new DataTemplate(() =>
            {
                CustomViewCell cell = new CustomViewCell();

                var grid = new Grid();
                grid.Padding = 0;

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var postLabel = new Label { Margin = new Thickness(10, 5, 10, 5), TextColor = (Color)Application.Current.Resources["Testo2"] };
                postLabel.SetBinding(Label.FormattedTextProperty, "Result");
                grid.Children.Add(postLabel, 0, 0);

                cell.View = grid;                
                return cell;
            });

            page = Application.Current.MainPage as CustomTabbedPage;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();


            //caricamento dei commenti
            await viewModel.LoadComments();
            if(viewModel.CommentsItems.Count > 0)
            {
                var item = viewModel.CommentsItems[viewModel.CommentsItems.Count - 1];
                bool scrollanimation = false;
                if (Device.RuntimePlatform == Device.iOS)
                    scrollanimation = true;
                CommentsListView.ScrollTo(item, ScrollToPosition.End, scrollanimation);
            }            
        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();
        }

        public void WillSparisci()
        {
            Console.WriteLine("@@@@ disappearing ");
            this.UpdateChildrenLayout();
            //page.TabBarHidden = false;
        }

        public async void WillAppari()
        {
            Console.WriteLine("@@@@ appearing ");
            await Task.Delay(2000);
            this.ForceLayout();
            this.UpdateChildrenLayout();
            //page.TabBarHidden = true;
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

        void Handle_Completed(object sender, TextChangedEventArgs e)
        {
            viewModel.PostCommentCommand.Execute(e.NewTextValue);
        }
    }
}
