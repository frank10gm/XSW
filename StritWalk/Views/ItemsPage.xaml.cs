using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.MediaManager;
//using System.Windows.Input;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using System.Diagnostics;
using Com.OneSignal;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace StritWalk
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        //IList<Item> items;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        public Command LoadMoreCommand { get; }

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            viewModel.Navigation = Navigation;
            viewModel.PostEditor = PostEditor;
            LoadMoreCommand = new Command(async () => await LoadMoreItems(null));

            var dataTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                grid.Padding = 0;
                grid.RowSpacing = 0;
                grid.ColumnSpacing = 0;
                grid.Margin = 0;

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                //var userLabel = new Label { Margin = new Thickness(20, 10, 10, 5) };
                //userLabel.SetBinding(Label.FormattedTextProperty, "Username");
                //grid.Children.Add(userLabel, 0, 0);
                //Grid.SetColumnSpan(userLabel, 4);

                //var imageProfile = new Image { Source = "cluster.png", Aspect = as };
                //grid.Children.Add(imageProfile, 0, 0);

                var postLabel = new Label { Margin = new Thickness(20, 10, 20, 10) };
                postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                grid.Children.Add(postLabel, 0, 0);
                Grid.SetColumnSpan(postLabel, 3);

                var separatorLine = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(0, 0, 0, 0) };
                grid.Children.Add(separatorLine, 0, 1);
                Grid.SetColumnSpan(separatorLine, 3);

                // numeri like play ecc
                var numbersLabel = new Label { Margin = new Thickness(20, 10, 20, 10), TextColor = Color.Gray, FontSize = 12 };
                numbersLabel.SetBinding(Label.TextProperty, "NumberOfLikes");
                grid.Children.Add(numbersLabel, 0, 2);
                Grid.SetColumnSpan(numbersLabel, 3);

                var separatorLine3 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(20, 0, 20, 0) };
                grid.Children.Add(separatorLine3, 0, 3);
                Grid.SetColumnSpan(separatorLine3, 3);

                //play button
                var playButton = new Button() { TextColor = Color.FromHex("#293e49"), FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(10, 0, 20, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                playButton.Text = "Play";
                //playButton.SetBinding(Button.TextProperty, "Play");
                playButton.Command = viewModel.IPlayThis;
                playButton.SetBinding(Button.CommandParameterProperty, ".");
                grid.Children.Add(playButton, 2, 4);
                //Grid.SetColumnSpan(playButton, 2);

                var likeButton = new Button() { TextColor = Color.Gray, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(20, 0, 10, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                likeButton.SetBinding(Button.TextColorProperty, "Liked_me");
                likeButton.SetBinding(Button.TextProperty, "Likes");
                likeButton.Command = viewModel.ILikeThis;
                likeButton.SetBinding(Button.CommandParameterProperty, ".");
                grid.Children.Add(likeButton, 0, 4);
                //Grid.SetColumnSpan(likeButton, 2);

                var commentsButton = new Button() { TextColor = Color.Gray, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(10, 0, 10, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                commentsButton.SetBinding(Button.TextProperty, "Comments_count");
                commentsButton.Command = viewModel.ICommentThis;
                commentsButton.SetBinding(Button.CommandParameterProperty, ".");
                grid.Children.Add(commentsButton, 1, 4);
                //Grid.SetColumnSpan(commentsButton, 2);                

                //var otherButton = new Button { Text = "Actions" };                
                //grid.Children.Add(otherButton, 4, 2);        

                var separatorLine2 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(0, 0, 0, 0) };
                grid.Children.Add(separatorLine2, 0, 5);
                Grid.SetColumnSpan(separatorLine2, 3);

                var commentsLabel = new Label { Margin = new Thickness(20, 0, 20, 0), Text = "", TextColor = Color.Black, FontSize = 13 };
                commentsLabel.SetBinding(Label.FormattedTextProperty, "ViewComments");
                //commentsLabel.SetBinding(IsVisibleProperty, "VisibleComments"); // non visualizzare la barra dei commenti quando non ci sono
                grid.Children.Add(commentsLabel, 0, 6);
                Grid.SetColumnSpan(commentsLabel, 3);
                //var tapGestureRecognizer = new TapGestureRecognizer();
                //tapGestureRecognizer.Command = viewModel.ICommentThis;
                //tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
                //commentsLabel.GestureRecognizers.Add(tapGestureRecognizer);

                var whiteSeparator = new BoxView { BackgroundColor = Color.FromHex("#efefef"), HeightRequest = 10 };
                grid.Children.Add(whiteSeparator, 0, 7);
                Grid.SetColumnSpan(whiteSeparator, 3);

                //var tapGestureRecognizer = new TapGestureRecognizer();
                //tapGestureRecognizer.Tapped += (s, e) => {

                //};
                //grid.GestureRecognizers.Add(tapGestureRecognizer);

                CustomViewCell cell = new CustomViewCell();

                cell.View = grid;
                cell.Tapges += (sender, e) =>
                {
                    viewModel.ICommentThis.Execute(sender);
                };

                return cell;
            });

            ItemsListView.ItemTemplate = dataTemplate;

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
            ItemsListView.SelectedItem = null;
        }

        private async void OnFocused(object sender, EventArgs args)
        {
            if (viewModel.NewPostDescription == PostEditor.Placeholder)
            {
                viewModel.NewPostDescription = string.Empty;
            }

            await Task.Delay(250);
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
            ItemsListView.SelectedItem = null;
        }

        void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {

            if (PostEditor.IsFocused && viewModel.IsPosting)
                PostEditor.Unfocus();

            if (viewModel.Items[viewModel.Items.Count - 1] == args.Item && !Settings.listEnd)
            {
                if (!viewModel.IsBusy)
                    Task.Run(() => LoadMoreItems(args.Item));
            }
            else
            {
                viewModel.EndText = "The End";
            }

        }

        async Task LoadMoreItems(object x)
        {
            try
            {
                viewModel.start += 20;
                viewModel.IsBusy = false;
                var items = await DataStore.GetItemsAsync(true, viewModel.start);
                viewModel.Items.AddRange(items);
                //foreach (var item in items)
                //{
                //    viewModel.Items.Add(item);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine("# EXCEPTION # \n there was a fuckin exception: " + ex);
            }
            finally
            {
                viewModel.IsBusy = false;
            }
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage(viewModel.Items));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }


        void Handle_Tapped(object sender, System.EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
