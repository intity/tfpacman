using System.Windows;
using TFlex.PackageManager.UI.Views;

namespace TFlex.PackageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void AppStartup(object sender, StartupEventArgs e)
        {
            bool debug = false;

            for (int i = 0; i < e.Args.Length; ++i)
            {
                if (e.Args[i] == "--api-info")
                    debug = true;
            }

            APILoader.Initialize(debug);
            MainWindow = new Main();
            MainWindow.Show();
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            APILoader.Terminate();
        }
    }
}