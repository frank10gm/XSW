using System;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace StritWalk
{
    public class CommentsItem 
    {
        public CommentsItem()
        {
        }
	
		public string Comment
		{
            get;
            set;
		}

        public string User_name
        {
            get;
            set;
        }
    }
}