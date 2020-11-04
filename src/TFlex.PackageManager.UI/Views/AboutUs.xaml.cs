using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace TFlex.PackageManager.UI.Views
{
    /// <summary>
    /// Interaction logic for AboutUs.xaml
    /// </summary>
    public partial class AboutUs : Window
    {
        public AboutUs()
        {
            InitializeComponent();

            #region initialize resources
            Title          = Properties.Resources.Strings["ui_1:title"][0];
            label1.Content = string.Format(
                             Properties.Resources.Strings["ui_1:lab_1"][0], Application.Version);
            label2.Content = Properties.Resources.Strings["ui_1:lab_2"][0];
            #endregion
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
