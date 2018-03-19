using System;

using Newtonsoft.Json;
using Xamarin.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace StritWalk
{
    public class Item : ObservableObject
    {

        public Item()
        {

        }

        string id = string.Empty;
        //[JsonIgnore]
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        string deadline = string.Empty;
        public string Deadline
        {
            get
            {
                return deadline;
            }
            set { SetProperty(ref deadline, value); }
        }

        string duedate = String.Empty;
        public string Duedate
        {
            get
            {
                return duedate;
            }
            set { SetProperty(ref duedate, value); }
        }

        DateTime duedate_insert;
        public DateTime Duedate_insert
        {
            get
            {
                return duedate_insert;
            }
            set { SetProperty(ref duedate_insert, value); }
        }

        public DateTime Duedate_show
        {
            get
            {
                if (duedate == null || duedate == "0000-00-00 00:00:00")
                    return DateTime.Now;
                Console.WriteLine("### conversione " + duedate);
                //DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                //dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(duedate)).ToLocalTime();                
                DateTime dtDateTime = DateTime.Parse(duedate);
                return dtDateTime;
            }
            set { }
        }

        public string Duedate_post
        {
            get
            {
                if (duedate != null || duedate != "0000-00-00 00:00:00")
                {
                    return " - due date: " + Duedate_show.ToString("dd/MM/yyyy");
                }
                else return "";
            }
            set { }
        }

        string notification_id = string.Empty;
        public string Notification_id
        {
            get
            {
                return notification_id;
            }
            set { SetProperty(ref notification_id, value); }
        }

        string creator_id = string.Empty;
        public string Creator_id
        {
            get
            {
                return creator_id;
            }
            set { SetProperty(ref creator_id, value); }
        }

        bool primo = true;
        public bool Primo
        {
            get { return primo; }
            set { SetProperty(ref primo, value); }
        }

        bool nuovo = false;
        public bool Nuovo
        {
            get { return nuovo; }
            set { SetProperty(ref nuovo, value); }
        }

        bool secondo = false;
        public bool Secondo
        {
            get { return secondo; }
            set { SetProperty(ref secondo, value); }
        }

        string lat = string.Empty;
        public string Lat
        {
            get { return lat; }
            set { SetProperty(ref lat, value); }
        }

        string lng = string.Empty;
        public string Lng
        {
            get { return lng; }
            set { SetProperty(ref lng, value); }
        }

        string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        string creator = string.Empty;
        public string Creator
        {
            get { return creator; }
            set { SetProperty(ref creator, value); }
        }

        string audio = string.Empty;
        public string Audio
        {
            get { return audio; }
            set { SetProperty(ref audio, value); }
        }

        string distanza = string.Empty;
        public string Distanza
        {
            get { return distanza; }
            set { SetProperty(ref distanza, value); }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        string todo_list = string.Empty;
        public string Todo_list
        {
            get
            {
                return todo_list;
            }
            set { SetProperty(ref todo_list, value); }
        }

        string added = string.Empty;
        public string Added
        {
            get { return added; }
            set { SetProperty(ref added, value); }
        }

        string details = string.Empty;
        public string Details
        {
            get
            {
                if (Distanza == "null")
                {
                    return Added + "ago";
                }

                if (float.Parse(Distanza, CultureInfo.InvariantCulture.NumberFormat) < 0.01)
                {
                    return Added + "ago, " + "here";
                }
                return Added + "ago, " + Distanza + " km away";
            }
            set { SetProperty(ref details, value); }
        }

        //string post = string.Empty;
        //public string Post
        //{
        //  get
        //  {
        //              return "{ " + Name + "}";
        //  }
        //  set { SetProperty(ref post, value); }
        //}

        public FormattedString Post
        {
            get
            {
                FormattedString result = new FormattedString();                              
                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span details = new Span { Text = Details + Duedate_post + "\n\n", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span name = new Span { Text = Name, FontAttributes = FontAttributes.Bold, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };
                Span description = new Span { Text = Description, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };

                if (Nuovo == true)
                {
                    details = new Span { Text = "here, now" + "\n\n", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                }

                result.Spans.Add(creator);
                result.Spans.Add(details);
                if (!string.IsNullOrEmpty(Name) || !string.IsNullOrWhiteSpace(Name))
                {
                    result.Spans.Add(brace1);
                    result.Spans.Add(name);
                    result.Spans.Add(brace2);
                }
                result.Spans.Add(description);

                return result;

            }
            set { }
        }

        public FormattedString Username
        {
            get
            {

                FormattedString result = new FormattedString();

                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span details = new Span { Text = Details + "", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4484fb") };
                Span name = new Span { Text = Name, FontAttributes = FontAttributes.Bold, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };
                Span description = new Span { Text = Description, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };

                if (Id == "new")
                {
                    details = new Span { Text = "here, now" + "", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                }

                result.Spans.Add(creator);
                result.Spans.Add(details);

                return result;

            }
            set { }
        }

        string likes = string.Empty;
        public string Likes
        {
            get
            {
                string text = "  Likes";
                if (Int32.Parse(likes) == 1) text = "  Like";
                return likes + text;
            }
            set { SetProperty(ref likes, value); }
        }
        public string LikesNum
        {
            get { return likes; }
        }

        string comments_count = string.Empty;
        public string Comments_count
        {
            get
            {
                string text = "  Comments";
                if (Int32.Parse(comments_count) == 1) text = " Comment";
                return comments_count + text;
            }
            set { SetProperty(ref comments_count, value); }
        }



        string liked_me = string.Empty;
        public string Liked_me
        {
            get
            {
                if (Int32.Parse(liked_me) == 1)
                    return "#4484fb";
                else return "#000000";
            }
            set { SetProperty(ref liked_me, value); }
        }

        JArray comments = null;
        public JArray Comments
        {
            get
            {
                return comments;
            }
            set { SetProperty(ref comments, value); }
        }

        [JsonIgnore]
        bool visibleComments = false;
        public bool VisibleComments
        {
            get
            {
                if (Int32.Parse(comments_count) > 0) return true;
                return false;
            }
            set { SetProperty(ref visibleComments, value); }
        }

        [JsonIgnore]
        FormattedString viewComments = string.Empty;
        public FormattedString ViewComments
        {
            get
            {
                if (Int32.Parse(comments_count) == 0)
                {
                    return "";
                }
                else
                {
                    FormattedString result = new FormattedString();
                    if (comments == null)
                        return result;

                    Span testo;

                    if (Int32.Parse(comments_count) == 1)
                    {
                        testo = new Span
                        {
                            Text = "There is " + comments_count + " comment" + "\n" + comments[0]["user_name"]
                            + ": " + comments[0]["comment"] + ""
                        };
                    }
                    else
                    {
                        testo = new Span
                        {
                            Text = "There are " + comments_count + " comments" + "\n" + comments[0]["user_name"]
                            + ": " + comments[0]["comment"] + "\n"
                        };
                    }

                    if (Int32.Parse(comments_count) > 1)
                        testo.Text += comments[1]["user_name"] + ": " + comments[1]["comment"];

                    result.Spans.Add(testo);
                    return result;
                }
            }
            set { SetProperty(ref viewComments, value); }
        }

    }
}