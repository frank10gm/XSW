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
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            await CrossMediaManager.Current.Play("http://www.hackweb.it/api/uploads/music/"+item.Audio);

            // Manually deselect item
            ItemsListView.SelectedItem = null;
        }

        async void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {
            
            if(viewModel.Items[viewModel.Items.Count - 4] == args.Item && Settings.listEnd == 0)
            {                
                viewModel.start += 10;
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
