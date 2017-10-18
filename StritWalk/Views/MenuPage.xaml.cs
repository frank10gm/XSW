using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class MenuPage : ContentPage
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public MenuPage()
        {
            InitializeComponent();
        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await DataStore.removePushId();
            App.LogOut();        
        }
    }
}
