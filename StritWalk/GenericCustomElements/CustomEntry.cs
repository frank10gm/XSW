using System;
using Xamarin.Forms;

namespace StritWalk
{
    public class CustomEntry : Entry
    {
		public const string ReturnKeyPropertyName = "ReturnKeyType";

        public new event EventHandler<EventArgs> Completed;

        public CustomEntry() { }

		public static readonly BindableProperty ReturnKeyTypeProperty = BindableProperty.Create(
			propertyName: ReturnKeyPropertyName,
			returnType: typeof(ReturnKeyTypes),
            declaringType: typeof(CustomEntry),
			defaultValue: ReturnKeyTypes.Done);

		public ReturnKeyTypes ReturnKeyType
		{
			get { return (ReturnKeyTypes)GetValue(ReturnKeyTypeProperty); }
			set { SetValue(ReturnKeyTypeProperty, value); }
		}

		public void InvokeCompleted()
		{
			this.Completed?.Invoke(this, null);
		}
    }


	// Not all of these are support on Android, consult EntryEditText.ImeOptions
	public enum ReturnKeyTypes : int
	{
		Default,
		Go,
		Google,
		Join,
		Next,
		Route,
		Search,
		Send,
		Yahoo,
		Done,
		EmergencyCall,
		Continue
	}
}
