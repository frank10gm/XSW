﻿using System;

using Newtonsoft.Json;
using Xamarin.Forms;

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

        bool primo = true;
        public bool Primo
        {
            get { return primo; }
            set { SetProperty(ref primo, value); }
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
                if (Distanza == "0.01")
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

                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#4885ED") };
                Span details = new Span { Text = Details + "\n\n", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4885ed") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4885ed") };
                Span name = new Span { Text = Name, FontAttributes = FontAttributes.Bold, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };
                Span description = new Span { Text = Description, FontSize = 14.0F, ForegroundColor = Color.FromHex("#000000") };

                if (Id == "new")
                {
                    details = new Span { Text = "here, now" + "\n\n", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                }

                //result.Spans.Add(creator);
                //result.Spans.Add(details);
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

                Span creator = new Span { Text = Creator + "\n", FontAttributes = FontAttributes.Bold, FontSize = 16.0F, ForegroundColor = Color.FromHex("#4885ED") };
                Span details = new Span { Text = Details + "", FontSize = 10.0F, ForegroundColor = Color.FromHex("#333333") };
                Span brace1 = new Span { Text = "{ ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4885ed") };
                Span brace2 = new Span { Text = " } ", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4885ed") };
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
            get {
                string text = " Likes";
                if (Int32.Parse(likes) == 1) text = " Like"; 
                return likes + text;
            }
            set { SetProperty(ref likes, value); }
        }

        string commentscount = string.Empty;
        public string Comments
        {
            get
            {
                string text = " Comments";
                if (Int32.Parse(commentscount) == 1) text = " Comment";
                return commentscount + text;
            }
            set { SetProperty(ref commentscount, value); }
        }

    }
}
