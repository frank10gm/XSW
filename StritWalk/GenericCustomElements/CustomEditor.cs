using System;
using Xamarin.Forms;
namespace StritWalk
{
    public class CustomEditor : Editor
    {
		public static BindableProperty PlaceholderProperty
        = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomEditor));

		public static BindableProperty PlaceholderColorProperty
        = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(CustomEditor), Color.FromHex("#888888"));

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public Color PlaceholderColor
		{
			get { return (Color)GetValue(PlaceholderColorProperty); }
			set { SetValue(PlaceholderColorProperty, value); }
		}
    }
}
