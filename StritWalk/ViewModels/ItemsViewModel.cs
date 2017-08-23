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
        public ObservableRangeCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand PostCommand { get; }

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
				await TryPostAsync();
			}
			finally
			{

			}
		}

		public async Task<bool> TryPostAsync()
		{
			return await DataStore.Login(Username, Password);
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
