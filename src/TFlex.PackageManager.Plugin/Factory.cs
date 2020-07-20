using System;
using System.Windows;
using TFlex.PackageManager.Properties;

namespace TFlex.PackageManager.Plugin
{
    /// <summary>
    /// The class definition in terms of the application entry
    /// </summary>
    public class Factory : PluginFactory
    {
        /// <summary>
        /// GUID application.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return new Guid("{F1653D63-FDF7-4367-AF0E-71C5D5885B8E}");
            }
        }

        /// <summary>
        /// Application name
        /// </summary>
        public override string Name
        {
            get
            {
                return Resources.AppName;
            }
        }

        /// <summary>
        /// Create instance an application.
        /// </summary>
        /// <returns>Instance an application.</returns>
        public override TFlex.Plugin CreateInstance()
        {
            TFlex.Plugin result;
            try
            {
                result = new PluginInstance(this);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                result = null;
            }
            return result;
        }
    }
}
