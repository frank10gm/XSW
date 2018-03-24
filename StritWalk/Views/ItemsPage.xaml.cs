using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;


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

                var postLabel = new Label { Margin = new Thickness(20, 0, 20, 0)};
                postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                var separatorLine = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(0, 0, 0, 0) };
                var numbersLabel = new Label { Margin = new Thickness(20, 0, 20, 0), TextColor = Color.Gray, FontSize = 12 };
                numbersLabel.SetBinding(Label.TextProperty, "NumberOfLikes");
                var separatorLine3 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(20, 0, 20, 0) };
                var playButton = new Button() { TextColor = Color.FromHex("#293e49"), FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                playButton.Text = "Play";
                playButton.SetBinding(IsEnabledProperty, "AudioExist");
                playButton.Command = viewModel.IPlayThis;
                playButton.SetBinding(Button.CommandParameterProperty, ".");
                var likeButton = new Button() { Text = "Like", FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                likeButton.SetBinding(Button.TextColorProperty, "Liked_me");
                //likeButton.SetBinding(Button.TextProperty, "Likes");
                likeButton.Command = viewModel.ILikeThis;
                likeButton.SetBinding(Button.CommandParameterProperty, ".");
                var commentsButton = new Button() { Text = "Comment", TextColor = Color.Gray, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                //commentsButton.SetBinding(Button.TextProperty, "Comments_count");
                commentsButton.Command = viewModel.ICommentThis;
                commentsButton.SetBinding(Button.CommandParameterProperty, ".");
                var separatorLine2 = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromHex("#efefef"), Margin = new Thickness(0, 0, 0, 0) };
                var commentsLabel = new Label { Margin = new Thickness(20, 0, 20, 0), Text = "", TextColor = Color.Gray, FontSize = 13 };
                commentsLabel.SetBinding(Label.FormattedTextProperty, "ViewComments");
                //var tapGestureRecognizer = new TapGestureRecognizer();
                //tapGestureRecognizer.Command = viewModel.ICommentThis;
                //tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
                //commentsLabel.GestureRecognizers.Add(tapGestureRecognizer);
                var whiteSeparator = new BoxView { BackgroundColor = Color.FromHex("#efefef"), HeightRequest = 10, Margin = new Thickness(0, 0, 0, 0) };


                //relative layout
                var layout = new RelativeLayout();

                layout.Children.Add(postLabel,
                    Constraint.Constant(0),
                    Constraint.Constant(10),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    null);

                layout.Children.Add(separatorLine,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(postLabel, (Parent, sibling) =>
                    {
                        return sibling.Y + sibling.Height + 10;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    Constraint.Constant(1));

                layout.Children.Add(numbersLabel,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(separatorLine, (Parent, sibling) =>
                    {
                        return sibling.Y + 11;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    Constraint.Constant(28));

                layout.Children.Add(separatorLine3,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(numbersLabel, (Parent, sibling) =>
                    {
                        return sibling.Y + sibling.Height;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    Constraint.Constant(1));

                layout.Children.Add(playButton,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(separatorLine3, (Parent, sibling) =>
                    {
                        return sibling.Y - 5;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width / 3;
                    }),
                    Constraint.Constant(48));

                layout.Children.Add(likeButton,
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width / 3;
                    }),
                    Constraint.RelativeToView(separatorLine3, (Parent, sibling) =>
                    {
                        return sibling.Y - 5;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width / 3;
                    }),
                    Constraint.Constant(48));

                layout.Children.Add(commentsButton,
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width / 3) * 2;
                    }),
                    Constraint.RelativeToView(separatorLine3, (Parent, sibling) =>
                    {
                        return sibling.Y - 5;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width / 3);
                    }),
                    Constraint.Constant(48));

                layout.Children.Add(separatorLine2,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(playButton, (Parent, sibling) =>
                    {
                        return sibling.Y + sibling.Height - 5;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    Constraint.Constant(1));

                layout.Children.Add(commentsLabel,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(separatorLine2, (Parent, sibling) =>
                    {
                        return sibling.Y + 1;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    null);

                layout.Children.Add(whiteSeparator,
                    Constraint.Constant(0),
                    Constraint.RelativeToView(commentsLabel, (Parent, sibling) =>
                    {
                        return sibling.Y + sibling.Height;
                    }),
                    Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }),
                    Constraint.Constant(10));


                //var grid = new Grid();
                //grid.Padding = 0;
                //grid.RowSpacing = 0;
                //grid.ColumnSpacing = 0;
                //grid.Margin = 0;

                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                //grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

                //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                //grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                ////var userLabel = new Label { Margin = new Thickness(20, 10, 10, 5) };
                ////userLabel.SetBinding(Label.FormattedTextProperty, "Username");
                ////grid.Children.Add(userLabel, 0, 0);
                ////Grid.SetColumnSpan(userLabel, 4);

                ////var imageProfile = new Image { Source = "cluster.png", Aspect = as };
                ////grid.Children.Add(imageProfile, 0, 0);


                //grid.Children.Add(postLabel, 0, 0);
                //Grid.SetColumnSpan(postLabel, 3);

                //grid.Children.Add(separatorLine, 0, 1);
                //Grid.SetColumnSpan(separatorLine, 3);

                //grid.Children.Add(numbersLabel, 0, 2);
                //Grid.SetColumnSpan(numbersLabel, 3);

                //grid.Children.Add(separatorLine3, 0, 3);
                //Grid.SetColumnSpan(separatorLine3, 3);                

                //grid.Children.Add(playButton, 2, 4);                       

                //grid.Children.Add(likeButton, 0, 4);                

                //grid.Children.Add(commentsButton, 1, 4);                

                ////var otherButton = new Button { Text = "Actions" };
                ////grid.Children.Add(otherButton, 4, 2);

                //grid.Children.Add(separatorLine2, 0, 5);
                //Grid.SetColumnSpan(separatorLine2, 3);

                ////commentsLabel.SetBinding(IsVisibleProperty, "VisibleComments"); // non visualizzare la barra dei commenti quando non ci sono
                //grid.Children.Add(commentsLabel, 0, 6);
                //Grid.SetColumnSpan(commentsLabel, 3);

                //grid.Children.Add(whiteSeparator, 0, 7);
                //Grid.SetColumnSpan(whiteSeparator, 3);                


                ////absolute layout
                //var abslayout = new AbsoluteLayout() { MinimumHeightRequest = 100 };

                //AbsoluteLayout.SetLayoutBounds(postLabel, new Rectangle(0f, 0f, 1, AbsoluteLayout.AutoSize));
                //AbsoluteLayout.SetLayoutFlags(postLabel, AbsoluteLayoutFlags.WidthProportional);
                //abslayout.Children.Add(postLabel);

                //AbsoluteLayout.SetLayoutBounds(separatorLine, new Rectangle(0f, 1, 1, 1));
                //AbsoluteLayout.SetLayoutFlags(separatorLine, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
                //abslayout.Children.Add(separatorLine);

                //AbsoluteLayout.SetLayoutBounds(numbersLabel, new Rectangle(0f, 1, 1, 100));
                //AbsoluteLayout.SetLayoutFlags(numbersLabel, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
                //abslayout.Children.Add(numbersLabel);


                //cell.Tapges += (sender, e) =>
                //{
                //    viewModel.ICommentThis.Execute(sender);
                //};


                ////var tapGestureRecognizer = new TapGestureRecognizer();
                ////tapGestureRecognizer.Tapped += (s, e) => {
                ////};
                ////grid.GestureRecognizers.Add(tapGestureRecognizer);

                CustomViewCell cell = new CustomViewCell();



                //scelta della vista
                //cell.View = grid;
                cell.View = layout;
                //cell.View.BackgroundColor = Color.Black;


                return cell;
            });

            ItemsListView.ItemTemplate = dataTemplate;
            ItemsListView.ItemSelectedCustomEvent += async (object o, SelectedItemChangedEventArgs e) =>
            {
                Item item = e.SelectedItem as Item;
                await Navigation.PushAsync(new PostPage(viewModel.Items, item));
                ItemsListView.SelectedItem = null;
            };

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

        private async void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            Console.WriteLine("@@@@@ itemtapped");
            Item item = args.Item as Item;            
            await Navigation.PushAsync(new PostPage(viewModel.Items, item));
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
