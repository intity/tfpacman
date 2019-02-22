using System.Windows;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for OptionsUI.xaml
    /// </summary>
    public partial class OptionsUI : Window
    {
        private Common.Options options;

        public OptionsUI(Common.Options options)
        {
            InitializeComponent();
            this.options = options;

            Title           = Resource.GetString(Resource.OPTIONS_UI, "title", 0);
            button1.Content = Resource.GetString(Resource.OPTIONS_UI, "button1", 0);
            button2.Content = Resource.GetString(Resource.OPTIONS_UI, "button2", 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            propertyGrid.SelectedObject = options;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            options.OptionsTask(1);
            Close();
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
    }
}