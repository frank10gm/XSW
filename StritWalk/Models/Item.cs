using System;

using Newtonsoft.Json;
using Xamarin.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace StritWalk
{
    public class Item
    {
        

        //[JsonIgnore]
        public string Id
        {
            get;
            set;
        }

        public bool Primo
        {
            get;
            set;
        }

        public bool Nuovo
        {
            get;
            set;
        }

        public bool Secondo
        {
            get;
            set;
        }

        public string Lat
        {
            get;
            set;
        }

        public string Lng
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Creator
        {
            get;
            set;
        }

        public string Audio
        {
            get;
            set;
        }

        public string Distanza
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Added
        {
            get;
            set;
        }

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
        }

        //string post = string.Empty;
        //public string Post
        //{
        //	get
        //	{
        //              return "{ " + Name + "}";
        //	}
        //	set { SetProperty(ref post, value); }
        //}

        public FormattedString Post
        {
            get
            {

                FormattedString result = new FormattedString();

                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#2b98f0") };
                Span details = new Span { Text = Details + "\n\n", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#2b98f0") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#2b98f0") };
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
        }

        public FormattedString Username
        {
            get
            {

                FormattedString result = new FormattedString();

                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#2b98f0") };
                Span details = new Span { Text = Details + "", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#2b98f0") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#2b98f0") };
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
        }

        public string LikesText
        {
            get
            {
                string text = "  Likes";
                if (Int32.Parse(Likes) == 1) text = "  Like";
                return Likes + text;
            }
        }

        public string Likes
        {
            get;
            set;
        }

        public string LikesNum
        {
            get { return Likes; }
        }


        public string Comments_count { get; set; }

        public string Comments_countText
        {
            get
            {
                string text = "  Comments";
                if (Int32.Parse(Comments_count) == 1) text = " Comment";
                return Comments_count + text;
            }
        }

        public string Liked_meText
        {
			get
			{
                if (Int32.Parse(Liked_me) == 1)
					return "#2b98f0";
				else return "#000000";
			}
		
        }

        public string Liked_me { get; set; }

        public JArray Comments
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool VisibleComments
        {
            get
            {
                if (Comments != null) return true;
                return false;
            }
        }

        [JsonIgnore]
        public FormattedString ViewComments
        {
            get
            {
                if (Int32.Parse(Comments_count) == 0)
                {
                    return "";
                }
                else
                {
                    FormattedString result = new FormattedString();
                    Span testo = new Span
                    {
                        Text = "View all " + Comments_count + " comments" + "\n" + Comments[0]["user_name"]
                            + ": " + Comments[0]["comment"] + "\n" + Comments[1]["user_name"] + ": " + Comments[0]["comment"]
                    };
                    result.Spans.Add(testo);
                    return result;
                }
            }
        }

    }
}
