using Xamarin.Forms;

namespace StritWalk
{
    public class BaseViewModel : ObservableObject
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();


        public INavigation Navigation
        {
            get; set;
        }

        string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        string username = string.Empty;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        bool formIsNotReady = true;
        public bool FormIsNotReady
        {
            get { return formIsNotReady; }
            set { SetProperty(ref formIsNotReady, value); }
        }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        bool isPosting = false;
        public bool IsPosting
        {
            get { return isPosting; }
            set { SetProperty(ref isPosting, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public FormattedString Gino
        {
            get
            {
                return new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = Settings.UserId + "\n", FontAttributes=FontAttributes.Bold, FontSize=16.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = Settings.UserDescription + "", FontSize=10.0F, ForegroundColor=Color.FromHex("#333333") }
                    }
                };
            }
            set { }
        }

        FormattedString postsN = new FormattedString
        {
            Spans =
                    {
                        new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = Settings.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
        };
        public FormattedString PostsN
        {
            get { return postsN; }
            set { SetProperty(ref postsN, value); }
        }

        public FormattedString FriendsN
        {
            get
            {
                return new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = "Friends" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = "225", FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
                };
            }
            set { }
        }

        public FormattedString LikesN
        {
            get
            {
                return new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = "Likes" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = "441", FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
                };
            }
            set { }
        }
    }
}
