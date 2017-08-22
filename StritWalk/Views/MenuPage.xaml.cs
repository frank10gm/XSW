using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }


        void Handle_Clicked(object sender, System.EventArgs e)
        {
            App.LogOut();
        }
    }
}
