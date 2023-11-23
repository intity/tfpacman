using System.Windows;
using TFlex.PackageManager.UI.Views;

namespace TFlex.PackageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void AppStartup(object sender, StartupEventArgs args)
        {
            APILoader.Initialize();

            MainWindow = new Main();
            MainWindow.Show();
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            APILoader.Terminate();
        }
    }
}