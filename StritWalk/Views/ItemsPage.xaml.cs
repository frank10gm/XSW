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

            viewModel.Navigation = Navigation;

            viewModel.PostEditor = PostEditor;

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
			
        }

        private void OnFocused(object sender, EventArgs args)
        {
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

        private void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            //ItemsListView.SelectedItem = null;
        }

        async void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {
            if (viewModel.Items[viewModel.Items.Count - 8] == args.Item && !Settings.listEnd)
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
