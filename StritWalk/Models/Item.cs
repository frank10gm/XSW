using System;

using Newtonsoft.Json;
using Xamarin.Forms;

namespace StritWalk
{
    public class Item : ObservableObject
    {
        string id = string.Empty;

        //[JsonIgnore]
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
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
                if(Distanza == "0.01"){
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
                return new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = Creator + "\n", FontAttributes=FontAttributes.Bold, FontSize=16.0F, ForegroundColor=Color.FromHex("#4885ED")},
                        new Span { Text = Details + "\n\n", FontSize=10.0F, ForegroundColor=Color.FromHex("#333333") },   
                        new Span { Text = "{ ", FontSize=14.0F, ForegroundColor=Color.FromHex("#4885ed") },
                        new Span { Text = Name, FontAttributes=FontAttributes.Bold, FontSize=14.0F, ForegroundColor=Color.FromHex("#000000") },
                        new Span { Text = " }", FontSize = 14.0F, ForegroundColor = Color.FromHex("#4885ed") }
                    }
                };
            }
            set { }
		}
    }
}
