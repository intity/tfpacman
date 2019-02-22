using System.Windows;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for InputCollection.xaml
    /// </summary>
    public partial class ListValues : Window
    {
        public ListValues()
        {
            InitializeComponent();

            Title           = Resource.GetString(Resource.LIST_VALUES, "title", 0);
            button1.Content = Resource.GetString(Resource.LIST_VALUES, "button1", 0);
            button2.Content = Resource.GetString(Resource.LIST_VALUES, "button2", 0);
        }

        /// <summary>
        /// Multiline text value.
        /// </summary>
        public string MultilineText
        {
            get { return textbox.Text; }
            set
            {
                if (textbox.Text != value)
                    textbox.Text = value;
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
    }
}