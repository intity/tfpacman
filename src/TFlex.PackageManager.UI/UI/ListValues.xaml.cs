using System.Windows;
using System.Windows.Controls;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for ListValues.xaml
    /// </summary>
    public partial class ListValues : Window
    {
        string value1;
        bool   value2;

        public ListValues()
        {
            InitializeComponent();

            Title            = Resource.GetString(Resource.LIST_VALUES, "title", 0);
            checkBox.Content = Resource.GetString(Resource.LIST_VALUES, "checkBox", 0);
            button_1.Content = Resource.GetString(Resource.LIST_VALUES, "button1", 0);
            button_2.Content = Resource.GetString(Resource.LIST_VALUES, "button2", 0);
        }

        #region public properties
        /// <summary>
        /// Multiline text value.
        /// </summary>
        public string MultilineText
        {
            get => textBox.Text;
            set
            {
                if (textBox.Text != value)
                    textBox.Text = value;
            }
        }

        /// <summary>
        /// Exclude from search.
        /// </summary>
        public bool ExcludeFromSeach
        {
            get => checkBox.IsChecked ?? false;
            set
            {
                if (checkBox.IsChecked != value)
                    checkBox.IsChecked = value;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.Length == 0)
                checkBox.IsEnabled = false;

            value1 = MultilineText;
            value2 = ExcludeFromSeach;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                checkBox.IsEnabled = true;
            }
            else
            {
                checkBox.IsChecked = false;
                checkBox.IsEnabled = false;
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = value1 != MultilineText || value2 != ExcludeFromSeach;
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
    }
}