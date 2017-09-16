using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace StritWalk
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ObservableRangeCollection<CommentsItem> CommentsItems { get; set; }

        public ItemDetailViewModel(object item = null)
        {
            Item = item as Item;
            Title = "Comments";
            CommentsItems = new ObservableRangeCollection<CommentsItem>();
            if (Item.Comments != null)
            {                
                Console.WriteLine("### array : " + Item.Comments);
                var items = Item.Comments.ToObject<IList<CommentsItem>>();
                CommentsItems.ReplaceRange(items);
            }
        }

        int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }
    }
}
