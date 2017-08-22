using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.iOS;
using UIKit;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace StritWalk.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer()
        {
        }

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if ((Control != null) && (e.NewElement != null))
            {
                var entryExt = (e.NewElement as CustomEntry);

                Control.ReturnKeyType = (e.NewElement as CustomEntry).ReturnKeyType.GetValueFromDescription();

				Control.ShouldReturn += (UITextField tf) =>
				{
                    entryExt?.InvokeCompleted();
					return true;
				};
            }
                
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomEntry.ReturnKeyPropertyName)
			{
                //D.WriteLine($"{(sender as CustomEntry).ReturnKeyType.ToString()}");
                Control.ReturnKeyType = (sender as CustomEntry).ReturnKeyType.GetValueFromDescription();

			}
        }

    }


	public static class EnumExtensions
	{
		public static UIReturnKeyType GetValueFromDescription(this ReturnKeyTypes value)
		{
			var type = typeof(UIReturnKeyType);
			if (!type.IsEnum) throw new InvalidOperationException();
			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field,
					typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attribute != null)
				{
					if (attribute.Description == value.ToString())
						return (UIReturnKeyType)field.GetValue(null);
				}
				else
				{
					if (field.Name == value.ToString())
						return (UIReturnKeyType)field.GetValue(null);
				}
			}
			throw new NotSupportedException($"Not supported on iOS: {value}");
		}
	}

}
