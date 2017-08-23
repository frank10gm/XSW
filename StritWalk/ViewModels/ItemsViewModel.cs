using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;

using Xamarin.Forms;

namespace StritWalk
{
    public class ItemsViewModel : BaseViewModel
    {
        public int start = 0;
        bool result = false;
        public ObservableRangeCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand PostCommand { get; }
        public CustomEditor PostEditor { get; set; }

		string newPostDescription = string.Empty;
		public string NewPostDescription
		{
			get { return newPostDescription; }
			set { SetProperty(ref newPostDescription, value); }
		}

        public ItemsViewModel()
        {
            Title = "Seahorse";
            Items = new ObservableRangeCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            PostCommand = new Command(async () => await PostTask());
        }

		async Task PostTask()
		{
			try
			{
				// Post method
				result = await TryPostAsync();
			}
			finally
			{
                if(result)
                {
                    Items.Insert(0, new Item { Id = "new", Creator = Settings.UserId , Description = newPostDescription });

					string text = "Posted. Do you want to post something else?";
					PostEditor.Placeholder = text;
					if (Device.iOS == Device.RuntimePlatform)
					{
                        PostEditor.TextColor = Color.FromHex("#888888");
					    PostEditor.Text = text;					  
					}
				}
			}
		}

		public async Task<bool> TryPostAsync()
		{
            return await DataStore.Post("","","","","",newPostDescription);
		}

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                start = 0;
                Settings.listEnd = 0;
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Items.ReplaceRange(items);
                //Items.Insert(0, new Item());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessagingCenter.Send(new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Unable to load items.",
                    Cancel = "OK"
                }, "message");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
