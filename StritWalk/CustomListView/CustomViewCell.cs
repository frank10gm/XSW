using System;
using Xamarin.Forms;

namespace StritWalk
{
    public class CustomViewCell : ViewCell
    {

        public new event EventHandler<EventArgs> Tapges;

		public void InvokeTap()
		{
            Tapges?.Invoke(this.BindingContext,null);
		}
        
    }
}
