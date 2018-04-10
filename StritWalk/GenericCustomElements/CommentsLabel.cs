using System;
using Xamarin.Forms;

namespace StritWalk
{
    public class CommentsLabel : Label
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
}
