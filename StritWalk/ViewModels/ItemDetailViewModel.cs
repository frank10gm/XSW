using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Com.OneSignal;

namespace StritWalk
{
    public class ItemDetailViewModel : BaseViewModel
    {

        string result = string.Empty;
        public Item Item { get; set; }
        public ObservableRangeCollection<CommentsItem> CommentsItems { get; set; }
        public Command PostCommentCommand { get; }
        public CustomListView listView { get; set; }
        int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }


        public ItemDetailViewModel(object item = null)
        {
            Item = item as Item;
            Title = "Comments";
            CommentsItems = new ObservableRangeCollection<CommentsItem>();
            PostCommentCommand = new Command(async (par1) => await PostCommentTask((object)par1));
        }

        public async Task LoadComments()
        {
            IsLoading = true;
            try
            {
                if (Item.Comments != null)
                {
                    var items = await DataStore.GetComments(Item.Id);
                    CommentsItems.ReplaceRange(items);
                    Item.Comments_count = CommentsItems.Count.ToString();
                    if (Item.Comments == null)
                        Item.Comments = new JArray();
                    var jitem = new JObject();
                    if (CommentsItems.Count() > 1)
                    {
                        jitem["user_name"] = items[items.Count() - 2].User_name;
                        jitem["comment"] = items[items.Count() - 2].Comment;
                        Item.Comments.Insert(0, jitem);
                        jitem = new JObject();
                    }
                    if (items.Count() > 0)
                    {
                        jitem["user_name"] = items[items.Count() - 1].User_name;
                        jitem["comment"] = items[items.Count() - 1].Comment;
                        Item.Comments.Insert(0, jitem);
                        Item.VisibleComments = true;
                        Item.NumberOfLikes = "3";
                        if (Device.RuntimePlatform == Device.Android) Item.ViewComments = ""; //disabilitato in modo che su ios la pagina precedente non si aggiorni
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("# EXCEPTION # \n" + ex);
            }
            IsLoading = false;
        }

        async Task PostCommentTask(object par1)
        {
            try
            {
                result = await DataStore.PostComment(Item.Id, (string)par1);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var item = new CommentsItem { User_name = Settings.UserId, Comment = (string)par1, NewComment = true };
                    CommentsItems.Add(item);
                    if (Device.RuntimePlatform == Device.iOS)
                        listView.ScrollTo(item, ScrollToPosition.End, true);
                    Item.Comments_count = CommentsItems.Count.ToString();
                    var jitem = new JObject();
                    jitem["user_name"] = item.User_name;
                    jitem["comment"] = item.Comment;
                    if (Item.Comments == null)
                        Item.Comments = new JArray();
                    Item.Comments.Insert(0, jitem);
                    Item.VisibleComments = true;
                    Item.NumberOfLikes = "3";
                    if (Device.RuntimePlatform == Device.Android) Item.ViewComments = ""; //disabilitato in modo che su ios la pagina precedente non si aggiorni                    
                    //MessagingCenter.Send(this, "NewComment", Item);

                    //invio della notifica                    
                    var notification = new Dictionary<string, object>();
                    notification["headings"] = new Dictionary<string, string>() { { "en", Settings.UserId } };
                    notification["contents"] = new Dictionary<string, string>() { { "en", "commented: " + (string)par1 } };
                    notification["include_player_ids"] = new List<string>() { Item.Notification_id };
                    // Example of scheduling a notification in the future.
                    //notification["send_after"] = System.DateTime.Now.ToUniversalTime().AddSeconds(30).ToString("U");
                    string text = "commented: " + (string)par1;
                    var data = $"{{ user_id: '{Settings.AuthToken}', notification_text: '{text}', post_id: '{Item.Id}', user_name: '{Settings.UserId}', creator: '{Item.Creator}', creator_id: '{Item.Creator_id}' }}";
                    await DataStore.sendNotifications(data);
                    if (Item.Notification_id != null && !string.IsNullOrEmpty(Item.Notification_id))
                    {
                        //OneSignal.Current.PostNotification(notification);                        
                    }
                }
            }
        }
    }
}
