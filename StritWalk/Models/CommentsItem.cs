using System;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace StritWalk
{
    public class CommentsItem : ObservableObject
    {
        public CommentsItem()
        {
        }

        string comment = string.Empty;
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        string user_name = string.Empty;
        public string User_name
        {
            get { return user_name; }
            set { SetProperty(ref user_name, value); }
        }

        string added = string.Empty;
        public string Added
        {
            get { return added; }
            set { SetProperty(ref added, value); }
        }

        bool newcomment = false;
        [JsonIgnore]
        public bool NewComment
        {
            get { return newcomment; }
            set { SetProperty(ref newcomment, value); }
        }

        [JsonIgnore]
        public FormattedString Result
        {
            get
            {
                FormattedString result = new FormattedString();
                Span user = new Span { Text = user_name + " ", FontAttributes = FontAttributes.Bold, FontSize = 12.0F, ForegroundColor = Color.FromHex("#293e49") }; //2b98f0 blue
                Span text = new Span { Text = comment + "\n", FontSize = 12.0F, ForegroundColor = Color.FromHex("#000000") };
                Span details = new Span { Text = added + "ago", FontSize = 8.0F, ForegroundColor = Color.FromHex("#333333") };
                if (newcomment)
                    details.Text = "now";
                //return user_name + ": " + comment + " (" + added + " ago)";
                result.Spans.Add(user);
                result.Spans.Add(text);
                result.Spans.Add(details);
                return result;
            }
        }
    }
}