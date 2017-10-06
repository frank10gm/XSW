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
	}
}