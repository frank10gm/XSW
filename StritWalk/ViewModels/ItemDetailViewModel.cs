using System;
using System.Diagnostics;
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

        string result = string.Empty;   

        public Item Item { get; set; }
        public ObservableRangeCollection<CommentsItem> CommentsItems { get; set; }
        public Command PostCommentCommand { get; }

        public ItemDetailViewModel(object item = null)
        {
            Item = item as Item;
            Title = "Comments";
            CommentsItems = new ObservableRangeCollection<CommentsItem>();
            if (Item.Comments != null)
            {                
                var items = Item.Comments.ToObject<IList<CommentsItem>>();
                CommentsItems.ReplaceRange(items);
            }

            PostCommentCommand = new Command(async (par1) => await PostCommentTask((object)par1));
        }

        int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }

        async Task PostCommentTask(object par1)
        {
            try
            {
                // Post method
                IsLoading = true;
                result = await DataStore.PostComment(Item.Id, (string)par1);
            }
            finally
            {
                IsLoading = false;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Debug.WriteLine("### insert id: " + result);
                    //var newitem = new Item { Id = result, Nuovo = true, Creator = Settings.UserId, Description = newPostDescription, Likes = "0", Comments_count = "0", Distanza = "0", Liked_me = "0" };
                    //Items.Insert(0, newitem);

                    //Settings.Num_posts += 1;
                    //me.Num_posts += 1;
                    //PostsN = new FormattedString
                    //{
                    //    Spans =
                    //    {
                    //        new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                    //        new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    //    }
                    //};

                    string text = "Posted. Do you want to post something else?";
                    //PostEditor.Placeholder = text;

                    if (Device.iOS == Device.RuntimePlatform)
                    {
                        //PostEditor.Text = text;
                    }
                    else
                    {
                        //PostEditor.Text = "";
                    }
                }
                else
                {
                    string text = "Do you want to post something?";
                    //PostEditor.Placeholder = text;
                }
                IsPosting = false;
                //PostEditor.Unfocus();
            }
        }
    }
}
