using System;
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

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as Item;
            //if (item == null)
                //return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //await CrossMediaManager.Current.Play("http://www.hackweb.it/api/uploads/music/" + item.Audio);

            // Manually deselect item
            ItemsListView.SelectedItem = null;
        }

        private void OnFocused(object sender, FocusEventArgs args)
        {
            PostEditor.Text = "";
            PostEditor.TextColor = Color.FromHex("#000000");
            viewModel.IsPosting = true;
            //PostEditor.HeightRequest = 40;
            //ItemsListView.ScrollTo(viewModel.Items[0],ScrollToPosition.Start,true);

        }

        private async void OnPosting(object sender, EventArgs args)
        {
            if (String.IsNullOrEmpty(PostEditor.Text) || String.IsNullOrWhiteSpace(PostEditor.Text))
            {
                viewModel.IsPosting = false;
                PostEditor.Text = "Do you want to post something?";
                PostEditor.TextColor = Color.FromHex("#888888");
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
