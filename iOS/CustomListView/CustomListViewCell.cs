using System;

using Foundation;
using UIKit;
using Xamarin.Forms;

namespace StritWalk.iOS
{
    public class CustomListViewCell : UITableViewCell, INativeElementView
    {

		//UILabel headingLabel, subheadingLabel;
		//UIImageView imageView;

        public CustomViewCell CustomViewCell { get; private set; }
        public Element Element => CustomViewCell; 
		

        public CustomListViewCell(string cellId, CustomViewCell cell) : base (UITableViewCellStyle.Default, cellId)
        {
            
            CustomViewCell = cell;

            SelectionStyle = UITableViewCellSelectionStyle.Blue;
			
			ContentView.BackgroundColor = UIColor.FromRGB(33, 247, 48);
           			
			//imageView = new UIImageView();

			//headingLabel = new UILabel()
			//{
			//	Font = UIFont.FromName("Cochin-BoldItalic", 22f),
			//	TextColor = UIColor.FromRGB(127, 51, 0),
			//	BackgroundColor = UIColor.Clear
			//};

			//subheadingLabel = new UILabel()
			//{
			//	Font = UIFont.FromName("AmericanTypewriter", 12f),
			//	TextColor = UIColor.FromRGB(38, 127, 0),
			//	TextAlignment = UITextAlignment.Center,
			//	BackgroundColor = UIColor.Clear
			//};

			//ContentView.Add(headingLabel);
			//ContentView.Add(subheadingLabel);
			//ContentView.Add(imageView);
		}
    }
}
