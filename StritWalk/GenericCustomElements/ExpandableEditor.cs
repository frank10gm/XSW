using System;
using System.Linq;
using Xamarin.Forms;
namespace StritWalk
{
    public class ExpandableEditor : Editor
    {
        //bool sized = false;
        public double lineHeight = 0;

        public static BindableProperty PlaceholderProperty
        = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ExpandableEditor));

        public static BindableProperty PlaceholderColorProperty
        = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExpandableEditor), Color.FromHex("#888888"));

        public static BindableProperty ReadyProperty
        = BindableProperty.Create(nameof(Ready), typeof(string), typeof(ExpandableEditor));

        public static BindableProperty ScrollReadyProperty
        = BindableProperty.Create(nameof(ScrollReady), typeof(bool), typeof(ExpandableEditor), false);

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

        public bool Ready
        {
            get { return (bool)GetValue(ReadyProperty); }
            set { SetValue(ReadyProperty, value); }
        }

        public bool ScrollReady
        {
            get { return (bool)GetValue(ScrollReadyProperty); }
            set { SetValue(ScrollReadyProperty, value); }
        }

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //  if (!sized)
        //  {
        //      int count = Text.Count(c => c == '\n');
        //      lineHeight = (height / (count + 1));
        //      sized = true;
        //  }
        //  base.OnSizeAllocated(width, height);
        //}

        public ExpandableEditor()
        {
            TextChanged += OnTextChanged;
        }

        ~ExpandableEditor()
        {
            TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //if (!ScrollReady)
            //this.InvalidateMeasure();
            //if(this.Height != -1){
            //    this.HeightRequest = this.Height * 2;
            //}
            var bounds = this.Bounds;
            bounds.Y = 0;
            this.LayoutTo(bounds);
        }

    }
}
