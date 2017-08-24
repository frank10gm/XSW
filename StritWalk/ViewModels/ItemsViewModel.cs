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

        bool isNotEnd = true;
        public bool IsNotEnd { get { return isNotEnd; } set { SetProperty(ref isNotEnd, value); } }

        public ItemsViewModel()
        {
            Title = "Seahorse";
            Items = new ObservableRangeCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            PostCommand = new Command(async () => await PostTask());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            MessagingCenter.Subscribe<CloudDataStore, bool>(this, "NotEnd", (sender, arg) => {
                IsNotEnd = arg;			
			});
        }

        async Task PostTask()
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
                if (result)
                {
                    Items.Insert(0, new Item { Id = "new", Creator = Settings.UserId, Description = newPostDescription });

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
                    PostEditor.Unfocus();
                    IsPosting = false;
                }
                else
                {
                    string text = "Do you want to post something?";
                    PostEditor.Placeholder = text;
                    PostEditor.Unfocus();
                    IsPosting = false;
                }
            }
        }

        public async Task<bool> TryPostAsync()
        {
            await Task.Delay(2000);
            return await DataStore.Post("", "", "", "", "", newPostDescription);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                start = 0;
                IsNotEnd = true;
                Settings.listEnd = false;
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
