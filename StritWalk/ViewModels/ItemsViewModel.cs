using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace StritWalk
{
    public class ItemsViewModel : BaseViewModel
    {
        public int start;
        string result = string.Empty;
        //public ObservableRangeCollection<Item> Items { get; set; }
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command PostCommand { get; }
        public CustomEditor PostEditor { get; set; }
        public Command ILikeThis { get; }
        public User me;
        public Command ICommentThis { get; }

        string newPostDescription = string.Empty;
        public string NewPostDescription
        {
            get { return newPostDescription; }
            set { SetProperty(ref newPostDescription, value); }
        }

        string endText = string.Empty;
        public string EndText
        {
            get { return endText; }
            set { SetProperty(ref endText, value); }
        }

        Color postPlaceholder = Color.Black;
        public Color PostPlaceholder
        {
            get { return postPlaceholder; }
            set { SetProperty(ref postPlaceholder, value); }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                var dataTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid();
                    grid.Padding = 0;

                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


                    var postLabel = new Label { Margin = new Thickness(20, 10, 20, 20) };
                    postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                    grid.Children.Add(postLabel, 0, 0);
                    Grid.SetColumnSpan(postLabel, 2);

                    var likeButton = new Button() { FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(10, 0, 10, 0) };
                    likeButton.SetBinding(Button.TextColorProperty, "Liked_meText");
                    likeButton.SetBinding(Button.TextProperty, "LikesText");
                    likeButton.Command = ILikeThis;
                    likeButton.SetBinding(Button.CommandParameterProperty, ".");
                    grid.Children.Add(likeButton, 0, 1);

                    var commentsButton = new Button() { TextColor = Color.Black, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(10, 0, 10, 0) };
                    commentsButton.SetBinding(Button.TextProperty, "Comments_countText");
                    commentsButton.Command = ICommentThis;
                    commentsButton.SetBinding(Button.CommandParameterProperty, ".");
                    grid.Children.Add(commentsButton, 1, 1);

                    var commentsLabel = new Label { Margin = new Thickness(15, 10, 10, 10), Text = "", TextColor = Color.Gray, FontSize = 9 };
                    commentsLabel.SetBinding(Label.FormattedTextProperty, "ViewComments");
                    commentsLabel.SetBinding(Label.IsVisibleProperty, "VisibleComments");
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) =>
                    {

                    };
                    //commentsLabel.GestureRecognizers.Add(tapGestureRecognizer);
                    grid.Children.Add(commentsLabel, 0, 2);
                    Grid.SetColumnSpan(commentsLabel, 2);

                    var whiteSeparator = new BoxView { BackgroundColor = Color.White, HeightRequest = 10 };
                    grid.Children.Add(whiteSeparator, 0, 3);
                    Grid.SetColumnSpan(whiteSeparator, 2);

                    CustomViewCell cell = new CustomViewCell();
                    cell.View = grid;
                    return cell;
                });
                return dataTemplate;
            }
        }

        bool isNotEnd = false;
        public bool IsNotEnd { get { return isNotEnd; } set { SetProperty(ref isNotEnd, value); } }

        public ItemsViewModel()
        {
            Title = "Seahorse";
            //Items = new ObservableRangeCollection<Item>();
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            PostCommand = new Command(async (par1) => await PostTask((object)par1));
            ILikeThis = new Command(async (par1) => await ILikeThisTask((object)par1));
            ICommentThis = new Command(async (par1) => await ICommentThisTask((object)par1));
            me = new User();
            //Task.Run(() => GetMyUser());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            MessagingCenter.Subscribe<CloudDataStore, bool>(this, "NotEnd", (sender, arg) =>
            {
                IsNotEnd = arg;
            });

        }

        void insertItem(Item item)
        {
            try
            {
                Items.Insert(0, item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("### EX ### " + ex);
            }

        }

        async Task PostTask(object par1)
        {
            try
            {
                // Post method
                IsLoading = true;
                result = await TryPostAsync();
            }
            finally
            {
                IsLoading = false;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var newitem = new Item { Id = result, Nuovo = true, Creator = Settings.UserId, Description = newPostDescription, Likes = "0", Comments_count = "0", Distanza = "0", Liked_me = "0" };
                    //Items.Insert(0, newitem);
                    insertItem(newitem);

                    Settings.Num_posts += 1;
                    me.Num_posts += 1;
                    PostsN = new FormattedString
                    {
                        Spans =
                        {
                            new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                            new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                        }
                    };

                    string text = "Posted. Do you want to post something else?";
                    PostEditor.Placeholder = text;

                    if (Device.iOS == Device.RuntimePlatform)
                    {
                        PostEditor.Text = text;
                    }
                    else
                    {
                        PostEditor.Text = "";
                    }
                }
                else
                {
                    string text = "Do you want to post something?";
                    PostEditor.Placeholder = text;
                }
                IsPosting = false;
                PostEditor.Unfocus();
            }
        }

        public async Task<string> TryPostAsync()
        {
            return await DataStore.Post("", "", "", "", "", newPostDescription);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await Task.Run(() => GetMyUser());
            EndText = "";
            start = 0;
            IsNotEnd = false;
            Settings.listEnd = false;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                //Items.ReplaceRange(items);			
                foreach (var item in items)
                {
                    Items.Add(item);
                }

                IsNotEnd = true;
                EndText = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //IsNotEnd = false;
                IsBusy = false;
            }
        }

        async Task ILikeThisTask(object par1)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Item item = par1 as Item;
            string action = "addLikePost";
            if (item.Liked_me == "#2b98f0")
                action = "removeLikePost";
            var res = await DataStore.ILikeThis((string)item.Id, action);
            if (res == 2)
            {
                var num = Int32.Parse(item.LikesNum);
                num -= 1;
                item.Likes = num.ToString();
                item.Liked_me = "0";
            }
            else if (res == 0)
            {
                var num = Int32.Parse(item.LikesNum);
                num += 1;
                item.Likes = num.ToString();
                item.Liked_me = "1";
            }
            IsBusy = false;
        }

        async Task GetMyUser()
        {
            me = await DataStore.GetMyUser(me);
            PostsN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
            LikesN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Liked" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_likes.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
            FriendsN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Followers" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_friends.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
        }

        async Task ICommentThisTask(object par1)
        {
            CustomTabbedPage page = Application.Current.MainPage as CustomTabbedPage;
            page.TabBarHidden = true;
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(par1)));
            //await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(par1))));
        }
    }
}
