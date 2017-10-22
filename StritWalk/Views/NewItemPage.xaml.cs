using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }
        public StackLayout mainLayout;
        public StackLayout listLayout;

        public NewItemPage(ObservableRangeCollection<Item> Items)
        {
            InitializeComponent();            


            Item = new Item
            {
                Id = "", //lo riceviamo dal post task 
                Name = "",
                Nuovo = true,
                Creator = Settings.UserId,
                Description = "",
                Likes = "0",
                Comments_count = "0",
                Distanza = "0",
                Liked_me = "0",
                Comments = new Newtonsoft.Json.Linq.JArray(),                
                Todo_list = ""
            };           

            BindingContext = this;
            DesignLayout();
        }

        void DesignLayout()
        {           
            ScrollView view = new ScrollView();
            Content = view;            

            mainLayout = new StackLayout()
            {
                Spacing = 10,
                Padding = 5
            };            

            Label title_l = new Label() { FontSize = 16, Text = "Title" };
            Entry title = new Entry();            
            title.SetBinding(Entry.TextProperty, "Item.Name");
            mainLayout.Children.Add(title_l);
            mainLayout.Children.Add(title);

            Label duedate_l = new Label() { FontSize = 16, Text = "Due Date" };
            DatePicker duedate = new DatePicker()
            {
                Format = "yyyy-MM-dd",
                Date = DateTime.Now.AddMonths(1)
            };
            duedate.SetBinding(DatePicker.DateProperty, "Item.Duedate");
            mainLayout.Children.Add(duedate_l);
            mainLayout.Children.Add(duedate);

            Label description_l = new Label() { FontSize = 16, Text = "Description" };
            Editor description = new Editor();
            description.SetBinding(Editor.TextProperty, "Item.Description");
            mainLayout.Children.Add(description_l);
            mainLayout.Children.Add(description);            

            listLayout = new StackLayout();
            Label steps = new Label() { FontSize = 16, Text = "To-Do" };
            Entry step1 = new Entry() { Placeholder = "first step..." };
            mainLayout.Children.Add(steps);
            listLayout.Children.Add(step1);
            mainLayout.Children.Add(listLayout);

            Button newstep = new Button() { Text = "+ step" };
            newstep.Clicked += Newstep_Clicked;
            mainLayout.Children.Add(newstep);

            duedate.Date = DateTime.Now.AddMonths(1);            

            view.Content = mainLayout;
        }

        private void Newstep_Clicked(object sender, EventArgs e)
        {            
            Entry newstep = new Entry() { Placeholder = "new step..." };            
            listLayout.Children.Add(newstep);
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            string todo_list = "";
            foreach(Entry step in listLayout.Children)
            {
                todo_list += "[ ] " + step.Text + "\n";
            }
            Item.Todo_list = todo_list;
            await Navigation.PopToRootAsync();
        }

    }
}
