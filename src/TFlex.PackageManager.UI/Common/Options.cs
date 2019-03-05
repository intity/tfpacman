using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
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
        private string targetDirectory;
        private bool enableLogFile;

        private readonly List<string> contents;
        private string logFile;
        #endregion

        public Options()
        {
            OptionsTask(0);

            if (targetDirectory == null)
                targetDirectory = Resource.UserDirectory;

            contents = new List<string>();
        }

        #region properties
        /// <summary>
        /// Target directory a configurations.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.OPTIONS_UI, "dn1_1")]
        [CustomDescription(Resource.OPTIONS_UI, "dn1_1")]
        [Editor(typeof(InputPathControl), typeof(UITypeEditor))]
        public string TargetDirectory
        {
            get { return targetDirectory; }
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;
                }
            }
        }

        /// <summary>
        /// Enable the output of data processing in the log file.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resource.OPTIONS_UI, "dn1_2")]
        [CustomDescription(Resource.OPTIONS_UI, "dn1_2")]
        [DefaultValue(false)]
        public bool EnableLogFile
        {
            get { return enableLogFile; }
            set
            {
                if (enableLogFile != value)
                {
                    enableLogFile = value;
                }
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Create log file the documents processing.
        /// </summary>
        /// <param name="targetDirectory"></param>
        internal void CreateLogFile(string targetDirectory)
        {
            if (!enableLogFile)
                return;

            logFile = Path.Combine(targetDirectory, Resource.LOG_FILE);

            if (contents.Count > 0)
                contents.Clear();

            if (File.Exists(logFile))
                File.Delete(logFile);

            File.AppendAllLines(logFile, contents);
        }

        /// <summary>
        /// Append new line to contents.
        /// </summary>
        /// <param name="value"></param>
        internal void AppendLine(string value)
        {
            if (!enableLogFile)
                return;

            contents.Add(value);
        }

        /// <summary>
        /// Set contents to log file.
        /// </summary>
        internal void SetContentsToLog()
        {
            if (!enableLogFile)
                return;

            File.AppendAllLines(logFile, contents);
        }

        /// <summary>
        /// Open log file.
        /// </summary>
        internal void OpenLogFile()
        {
            if (!enableLogFile)
                return;

            if (File.Exists(logFile))
                Process.Start("notepad.exe", logFile);
        }

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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OptionTask(RegistryKey subKey, string name, int flag)
        {
            switch (name)
            {
                case "TargetDirectory":
                    if (flag == 0)
                        targetDirectory = (string)subKey.GetValue(name);
                    else
                        subKey.SetValue(name, targetDirectory);
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