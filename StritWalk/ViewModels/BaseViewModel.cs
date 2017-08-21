using Xamarin.Forms;

namespace StritWalk
{
    public class BaseViewModel : ObservableObject
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
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
						new Span { Text = "Gino è un super robottino." + "", FontSize=10.0F, ForegroundColor=Color.FromHex("#333333") }
					}
				};
            }
            set { }
        }

		public FormattedString PostsN
		{
			get
			{
				return new FormattedString
				{
					Spans =
					{
						new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
						new Span { Text = "191", FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
					}
				};
			}
			set { }
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
