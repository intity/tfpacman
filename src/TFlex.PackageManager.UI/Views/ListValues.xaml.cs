using System.Windows;
using System.Windows.Controls;

namespace TFlex.PackageManager.UI.Views
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

            #region initialize resources
            Title            = Properties.Resources.Strings["ui_2:title"][0];
            checkBox.Content = Properties.Resources.Strings["ui_2:cbx_1"][0];
            button1.Content  = Properties.Resources.Strings["ui_2:btn_1"][0];
            button2.Content  = Properties.Resources.Strings["ui_2:btn_2"][0];
            #endregion
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

        #region events
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
        #endregion
    }
}