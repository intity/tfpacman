﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Windows;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Common
{
    /// <summary>
    /// The application options class.
    /// </summary>
    public class Options
    {
        #region private fields
        private string userDirectory;
        private bool openLogFile;
        #endregion

        public Options()
        {
            OptionsTask(0);
            if (userDirectory == null)
                userDirectory = Resources.UserDirectory;
        }

        #region properties
        /// <summary>
        /// Target directory a configurations.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resources.OPTIONS_UI, "dn1_1")]
        [CustomDescription(Resources.OPTIONS_UI, "dn1_1")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        public string UserDirectory
        {
            get => userDirectory;
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
        /// Open the log file when processing is complete.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resources.OPTIONS_UI, "dn1_2")]
        [CustomDescription(Resources.OPTIONS_UI, "dn1_2")]
        public bool OpenLogFile
        {
            get => openLogFile;
            set
            {
                if (openLogFile != value)
                {
                    openLogFile = value;
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
            string path = Path.Combine(Resources.RootKey, "Options");
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
                case "OpenLogFile":
                    if (flag == 0)
                        openLogFile = (int)subKey.GetValue(name) > 0;
                    else
                        subKey.SetValue(name, openLogFile ? 1 : 0);
                    break;
            }
        }
        #endregion
    }
}