using System.Windows;
using TFlex.PackageManager.UI;

namespace TFlex.PackageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            APILoader.Initialize();

            MainWindow = new Main();
            MainWindow.Show();
        }
    }
}