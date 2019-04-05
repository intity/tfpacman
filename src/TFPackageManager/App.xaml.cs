using System;
using System.Windows;

namespace TFlex.PackageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            ApiLoader.Preload();
            ApiLoader.InitSession();

            UI.MainWindow packageManager = new UI.MainWindow();
            packageManager.Show();
            packageManager.Closed += PackageManager_Closed;
        }

        private void PackageManager_Closed(object sender, EventArgs e)
        {
            ApiLoader.ExitSession();
        }
    }
}