using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Common
{
    /// <summary>
    /// The application options class.
    /// </summary>
    public class Options
    {
        #region private fields
        private string userDirectory;
        private bool enableLogFile;
        #endregion

        public Options()
        {
            OptionsTask(0);
            userDirectory = Resource.UserDirectory;
        }

        #region properties
        /// <summary>
        /// Target directory a configurations.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.OPTIONS_UI, "dn1_1")]
        [CustomDescription(Resource.OPTIONS_UI, "dn1_1")]
        [Editor(typeof(InputPathControl), typeof(UITypeEditor))]
        public string UserDirectory
        {
            get { return userDirectory; }
            set
            {
                if (userDirectory != value)
                {
                    userDirectory = value;
                    OptionsTask(1);
                }
            }
        }

        /// <summary>
        /// Enable the output of data processing in the log file.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resource.OPTIONS_UI, "dn1_2")]
        [CustomDescription(Resource.OPTIONS_UI, "dn1_2")]
        public bool EnableLogFile
        {
            get { return enableLogFile; }
            set
            {
                if (enableLogFile != value)
                {
                    enableLogFile = value;
                    OptionsTask(1);
                }
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// The application options task method.
        /// </summary>
        /// <param name="flag">(0) - read, (1) - write, (2) - delete</param>
        internal void OptionsTask(int flag)
        {
            string path = Resource.RootKey + "\\Options";
            RegistryKey subKey;
            try
            {
                if (flag == 0)
                    subKey = Registry.CurrentUser.OpenSubKey(path, false);
                else
                    subKey = Registry.CurrentUser.CreateSubKey(path, 
                        RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (subKey == null)
                    return;
                else if (flag == 1)
                {
                    foreach (var i in GetType().GetProperties())
                    {
                        OptionTask(subKey, i.Name, flag);
                    }
                }
                else if (flag == 2)
                {
                    subKey.DeleteSubKey(path);
                }
                else
                {
                    foreach (var i in subKey.GetValueNames())
                    {
                        OptionTask(subKey, i, flag);
                    }
                }

                subKey.Close();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OptionTask(RegistryKey subKey, string name, int flag)
        {
            switch (name)
            {
                case "UserDirectory":
                    if (flag == 0)
                        userDirectory = (string)subKey.GetValue(name);
                    else
                        subKey.SetValue(name, userDirectory);
                    break;
                case "EnableLogFile":
                    if (flag == 0)
                        enableLogFile = (int)subKey.GetValue(name) > 0 ? true : false;
                    else
                        subKey.SetValue(name, enableLogFile ? 1 : 0);
                    break;
            }
        }
        #endregion
    }
}