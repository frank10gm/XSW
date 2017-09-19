﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace StritWalk
{
    public class CustomListView : ListView
    {

        public CustomListView(ListViewCachingStrategy strategy) : base(strategy)
        {
        }

		public static readonly BindableProperty KeyboardOnProperty =
            BindableProperty.Create("KeyboardOn", typeof(bool), typeof(CustomListView), false);

		public bool KeyboardOn
		{
			get { return (bool)GetValue(KeyboardOnProperty); }
			set { SetValue(KeyboardOnProperty, value); }
		}

        //public static readonly BindableProperty ItemsProperty =
        //BindableProperty.Create("Items", typeof(IList<Item>), typeof(CustomListView), new List<Item>());

        //      public IList<Item> Items
        //{
        //          get { return (IList<Item>)GetValue(ItemsProperty); }
        //	set { SetValue(ItemsProperty, value); }
        //}

        //public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        //public void NotifyItemSelected(object item)
        //{
        //	if (ItemSelected != null)
        //	{
        //		ItemSelected(this, new SelectedItemChangedEventArgs(item));
        //	}
        //}
    }
}
