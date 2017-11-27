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

        bool isWorking = false;
        public bool IsWorking
        {
            get { return isWorking; }
            set { SetProperty(ref isWorking, value); }
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
                        new Span { Text = Settings.UserId + "\n", FontAttributes=FontAttributes.Bold, FontSize=16.0F, ForegroundColor=Color.FromHex("#000000"), FontFamily="Akzidenz-Grotesk Pro"},
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


		FormattedString likesN = new FormattedString
		{
			Spans =
			{
				new Span { Text = "Liked" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
				new Span { Text = Settings.Num_likes.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
			}
		};
		public FormattedString LikesN
		{
			get => likesN;
			set => SetProperty(ref likesN, value);
		}


		FormattedString friendsN = new FormattedString
		{
			Spans =
			{
				new Span { Text = "Followers" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
				new Span { Text = Settings.Num_friends.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
			}
		};
		public FormattedString FriendsN
		{
			get => friendsN;
			set => SetProperty(ref friendsN, value);
		}
	}
}