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
            if (ApiLoader.Preload())
            {
                ApiLoader.InitSession();

                UI.MainWindow packageManager = new UI.MainWindow();
                packageManager.Show();
                packageManager.Closed += PackageManager_Closed;
            }
            else
            {
                Shutdown();
            }
        }

        private void PackageManager_Closed(object sender, EventArgs e)
        {
            if (ApiLoader.IsLoaded)
            {
                ApiLoader.ExitSession();
            }
        }
    }
}