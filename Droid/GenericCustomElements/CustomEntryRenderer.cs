using System;
using System.ComponentModel;
using Android.Views.InputMethods;
using StritWalk;
using StritWalk.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace StritWalk.Droid
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
				Control.ImeOptions = entryExt.ReturnKeyType.GetValueFromDescription();
				// This is hackie ;-) / A Android-only bindable property should be added to the EntryExt class 
				Control.SetImeActionLabel(entryExt.ReturnKeyType.ToString(), Control.ImeOptions);

				Control.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
				{
                    if (entryExt.ReturnKeyType.ToString() != "Next")
                        entryExt.Unfocus();
				
					entryExt?.InvokeCompleted();
				};
			}
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomEntry.ReturnKeyPropertyName)
			{
                var entryExt = (sender as CustomEntry);
				Control.ImeOptions = entryExt.ReturnKeyType.GetValueFromDescription();
				// This is hackie ;-) / A Android-only bindable property should be added to the EntryExt class 
				Control.SetImeActionLabel(entryExt.ReturnKeyType.ToString(), Control.ImeOptions);
			}
        }
		

	}

	public static class EnumExtensions
	{
		public static ImeAction GetValueFromDescription(this ReturnKeyTypes value)
		{
			var type = typeof(ImeAction);
			if (!type.IsEnum) throw new InvalidOperationException();
			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field,
					typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attribute != null)
				{
					if (attribute.Description == value.ToString())
						return (ImeAction)field.GetValue(null);
				}
				else
				{
					if (field.Name == value.ToString())
						return (ImeAction)field.GetValue(null);
				}
			}
			throw new NotSupportedException($"Not supported on Android: {value}");
		}
	}

}
