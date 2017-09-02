using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.MediaManager;
using System.Windows.Input;

namespace StritWalk
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        IList<Item> items;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        public ICommand LoadMoreCommand { get; }

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            viewModel.Navigation = Navigation;        
            viewModel.PostEditor = PostEditor;            
            LoadMoreCommand = new Command(async () => await LoadMoreItems());

            ItemsListView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                grid.Padding = 0;
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

                var userLabel = new Label { Margin = new Thickness(20, 10, 10, 5) };
                userLabel.SetBinding(Label.FormattedTextProperty, "Username");
                grid.Children.Add(userLabel, 0, 0);
                Grid.SetColumnSpan(userLabel, 4);

                var postLabel = new Label { Margin = new Thickness(20, 0, 20, 20) };
                postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                grid.Children.Add(postLabel, 0, 1);
                Grid.SetColumnSpan(postLabel, 5);

                var likeButton = new Button() { TextColor = Color.Black, FontSize = 12 };
                likeButton.SetBinding(Button.TextProperty, "Likes");
                grid.Children.Add(likeButton, 1, 2);
                //Grid.SetColumnSpan(likeButton, 2);

                var commentsButton = new Button() { TextColor = Color.Black, FontSize = 12 };
                commentsButton.SetBinding(Button.TextProperty, "Comments_count");
                grid.Children.Add(commentsButton, 3, 2);
                //Grid.SetColumnSpan(commentsButton, 2);

                //var otherButton = new Button { Text = "Actions" };                
                //grid.Children.Add(otherButton, 4, 2);                

                var commentsLabel = new Label { Margin = new Thickness(15, 10, 10, 10), Text = "View all comments", TextColor = Color.Gray, FontSize = 9, IsVisible = false };
                commentsLabel.SetBinding(Label.IsVisibleProperty, "ViewComments");
                grid.Children.Add(commentsLabel, 0, 3);
                Grid.SetColumnSpan(commentsLabel, 5);

                var whiteSeparator = new BoxView { BackgroundColor = Color.White, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 10 };
                grid.Children.Add(whiteSeparator, 0, 4);
                Grid.SetColumnSpan(whiteSeparator, 5);

                return new CustomViewCell { View = grid };


                //var layout = new AbsoluteLayout();

                //var userLabel = new Label { Margin = new Thickness(20, 10, 10, 5) };
                //userLabel.SetBinding(Label.FormattedTextProperty, "Username");
                //AbsoluteLayout.SetLayoutBounds(userLabel, new Rectangle(0,0,1,.2));
                //AbsoluteLayout.SetLayoutFlags(userLabel, AbsoluteLayoutFlags.All);            
                //layout.Children.Add(userLabel);

                //var postLabel = new Label { Margin = new Thickness(20, 0, 20, 20) };
                //postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                //AbsoluteLayout.SetLayoutBounds(postLabel, new Rectangle(0, .4, 1, .8));
                //AbsoluteLayout.SetLayoutFlags(postLabel, AbsoluteLayoutFlags.All);
                //layout.Children.Add(postLabel);

                //return new CustomViewCell { View = layout };
            });

        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as Item;
            //if (item == null)
            //return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //await CrossMediaManager.Current.Play("http://www.hackweb.it/api/uploads/music/" + item.Audio);            

            ItemsListView.SelectedItem = null;
        }

        private void OnCellTapped(object sender, EventArgs args)
        {

        }

        private void OnFocused(object sender, EventArgs args)
        {
            viewModel.IsPosting = true;
        }

        private void OnCheckTest(object sender, EventArgs args)
        {
            if (String.IsNullOrEmpty(PostEditor.Text) || String.IsNullOrWhiteSpace(PostEditor.Text))
            {
                string text = "Do you want to post something?";
                PostEditor.Placeholder = text;
                viewModel.IsPosting = false;
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            //ItemsListView.SelectedItem = null;
        }

        void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {
            if (viewModel.Items[viewModel.Items.Count - 1] == args.Item && !Settings.listEnd)
            {
                //LoadMoreCommand.Execute(null);
                Task.Run(LoadMoreItems);
            }
            else
            {
                viewModel.EndText = "The End";
            }
        }

        async Task LoadMoreItems()
        {
            try
            {
                viewModel.start += 20;
                items = await DataStore.GetItemsAsync(true, viewModel.start);
            }
            finally
            {
                viewModel.Items.AddRange(items);
                //for (var i = 0; i < items.Count; i++)
                //{
                //    viewModel.Items.Add(items[i]);                    
                //}
            }
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
