using Xamarin.Forms;
using System;

namespace StritWalk

{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FirstTemplate { get; set; }
        public DataTemplate SecondTemplate { get; set; }
        private readonly DataTemplate templateOne;
        private readonly DataTemplate templateTwo;

        public CustomDataTemplateSelector()
        {
            // Retain instances!
            this.templateOne = new DataTemplate(typeof(TemplateOneViewCell));
            this.templateTwo = new DataTemplate(typeof(TemplateTwoViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Item)item).VisibleComments > 0 ? SecondTemplate : FirstTemplate;
            //return ((Item)item).VisibleComments > 0 ? this.templateTwo : this.templateOne;
        }
    }
}

