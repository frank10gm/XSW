using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.MediaManager;

namespace StritWalk
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();

        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as Item;
            //if (item == null)
                //return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //await CrossMediaManager.Current.Play("http://www.hackweb.it/api/uploads/music/" + item.Audio);

            // Manually deselect item
            ItemsListView.SelectedItem = null;
        }

        private void OnCellTapped(object sender, EventArgs args)
        {
            
        }

        private void OnFocused(object sender, FocusEventArgs args)
        {
            
            //PostEditor.Text = "";
            //PostEditor.TextColor = Color.FromHex("#000000");
            viewModel.IsPosting = true;

			string text = "Posted. Do you want to post something else?";
            string original_text = "Do you wand to post something?";
            if(PostEditor.Placeholder == text)
            {
				if (Device.iOS == Device.RuntimePlatform)
				{
                    PostEditor.Text = original_text;
					PostEditor.TextColor = Color.FromHex("#888888");
				}
                PostEditor.Placeholder = original_text;
            }
	

            //PostEditor.HeightRequest = 40;
            //ItemsListView.ScrollTo(viewModel.Items[0],ScrollToPosition.Start,true);

        }

        private void OnCheckTest(object sender, EventArgs args)
        {
            if (String.IsNullOrEmpty(PostEditor.Text) || String.IsNullOrWhiteSpace(PostEditor.Text))
            {
                viewModel.IsPosting = false;
                //PostEditor.TextColor = Color.FromHex("#888888");
                //PostEditor.Text = "Do you want to post something?";
            }
        }

        private void OnPosting(object sender, EventArgs args)
        {
            PostEditor.Unfocus();
			if (String.IsNullOrEmpty(PostEditor.Text) || String.IsNullOrWhiteSpace(PostEditor.Text))
			{
				viewModel.IsPosting = false;
				//PostEditor.Text = "Do you want to post something?";
				//PostEditor.TextColor = Color.FromHex("#888888");
			}
            else
            {
                viewModel.IsPosting = false;
                //PostEditor.TextColor = Color.FromHex("#888888");
				//ThreadStart myThreadDelegate = new ThreadStart(TypeWriter);
				//Thread myThread = new Thread(myThreadDelegate);
				//myThread.Start();
				//postare qui
				string text = "Posted. Do you want to post something else?";
                PostEditor.Placeholder = text;
                if (Device.iOS == Device.RuntimePlatform)
                {
                    PostEditor.Text = text;
                    PostEditor.TextColor = Color.FromHex("#888888");
                }
            }
        }

        public void TypeWriter()
        {
			string text = "Posted. Do you want to post something else?";
			PostEditor.Text = "";
			for (int i = 0; i < text.Length; i++)
			{
				PostEditor.Text += text[i];
				Thread.Sleep(50);
			}
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            ItemsListView.SelectedItem = null;
        }

        async void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {
            if (viewModel.Items[viewModel.Items.Count - 4] == args.Item && Settings.listEnd == 0)
            {
                viewModel.start += 20;
                var items = await DataStore.GetItemsAsync(true, viewModel.start);
                viewModel.Items.AddRange(items);
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
