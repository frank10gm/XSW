﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using StritWalk;

namespace StritWalk
{
    class ClickableLabel : Label
    {
        public event EventHandler<ItemSelectedArgs> Clicked;

        public static BindableProperty ItemProperty
        = BindableProperty.Create(nameof(ItemSelected), typeof(Item), typeof(Item));

        public Item ItemSelected
        {
            get { return (Item)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public void InvokeClicked(Item item)
        {
            Clicked?.Invoke(this, new ItemSelectedArgs(item));
        }
    }

    public class ItemSelectedArgs : EventArgs
    {
        private readonly Item _item;

        public ItemSelectedArgs(Item item)
        {
            _item = item;
        }

        public Item PItem
        {
            get { return _item; }
        }
    }
}
