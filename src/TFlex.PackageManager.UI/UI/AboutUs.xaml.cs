using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for AboutUs.xaml
    /// </summary>
    public partial class AboutUs : Window
    {
        public AboutUs()
        {
            InitializeComponent();

            Title          = Resource.GetString(Resource.ABOUT_US, "title", 0);
            label1.Content = string.Format(
                             Resource.GetString(Resource.ABOUT_US, "label1", 0), TFlex.Application.Version);
            label2.Content = Resource.GetString(Resource.ABOUT_US, "label2", 0);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
