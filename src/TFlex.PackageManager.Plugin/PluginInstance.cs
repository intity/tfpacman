using System.Drawing;
using TFlex.Model;
using TFlex.PackageManager.UI.Views;
using TFlex.PackageManager.UI.Properties;

namespace TFlex.PackageManager.Plugin
{
    /// <summary>
    /// The extension commands pane and the main menu.
    /// </summary>
    enum Commands
    {
        PackageManager = 1
    }

    /// <summary>
    /// The extension class.
    /// </summary>
    public partial class PluginInstance : TFlex.Plugin
    {
        readonly string command1, toolbar1;
        const string PLUGIN = "Plugin.resx";

        /// <summary>
        /// The Constructor.
        /// </summary>
        /// <param name="factory"></param>
        public PluginInstance(Factory factory) : base(factory)
        {
            command1 = Resources.GetString(PLUGIN, "command1")[0];
            toolbar1 = Resources.GetString(PLUGIN, "toolbar1")[0];
        }

        /// <summary>
        /// Application initialization function
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// In this function, the application must create its own toolbar, 
        /// register their menu items as well as create other user interface means 
        /// (non-modal windows, etc.).
        /// </summary>
        protected override void OnCreateTools()
        {
            base.OnCreateTools();

            RegisterCommand((int)Commands.PackageManager,
                command1,
                new Icon(Properties.Resources.package_manager, 16, 16),
                new Icon(Properties.Resources.package_manager, 32, 32));

            Application.ActiveMainWindow.InsertPluginMenuItem((int)Commands.PackageManager,
                command1, 
                MainWindow.InsertMenuPosition.EndOfTools, this);

            CreateToolbar(toolbar1, new int[] {
                (int)Commands.PackageManager
            });

            RibbonGroup ribbonGroup = RibbonBar.ApplicationsTab.AddGroup(toolbar1);
            ribbonGroup.AddButton((int)Commands.PackageManager, this, 
                RibbonButtonStyle.LargeIconAndCaption);
        }

        /// <summary>
        /// The application overrides this feature for commands that, 
        /// this application is registered.
        /// </summary>
        /// <param name="document">The Document.</param>
        /// <param name="id">The command id.</param>
        protected override void OnCommand(Document document, int id)
        {
            switch ((Commands)id)
            {
                default:
                    base.OnCommand(document, id);
                    break;
                case Commands.PackageManager:
                    Main tfpacman = new Main();
                    tfpacman.Show();
                    break;
            }
        }
    }
}