using Xamarin.Forms;

namespace StritWalk

{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FirstTemplate { get; set; }

        public DataTemplate SecondTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Item)item).VisibleComments > 0 ? SecondTemplate : FirstTemplate;
        }
    }
}

