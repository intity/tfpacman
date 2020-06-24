using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows;
using Microsoft.Win32;

#pragma warning disable CA1303

namespace TFlex
{
    public static class ApiLoader
    {
        #region private fields
        private const string API_VERSION = "16.0.68.0";
        private static List<string> folders;
        private static readonly string error_msg = string.Format(
            CultureInfo.InvariantCulture, 
            "T-FLEX CAD {0} version not installed", API_VERSION);
        #endregion

        /// <summary>
        /// T-FLEX CAD API is Loaded.
        /// </summary>
        public static bool IsLoaded { get; private set; }

        #region public methods
        /// <summary>
        /// Preload T-FLEX CAD API.
        /// </summary>
        /// <returns></returns>
        public static bool Preload()
        {
            folders = new List<string>();

            string[] products = new string[]
            {
                @"T-FLEX CAD 3D 16\Rus",
                @"T-FLEX CAD SE 16\Rus",
                @"T-FLEX CAD 3D 16\Eng",
                @"T-FLEX CAD SE 16\Eng",
                @"TENADO CAD 3D 16\Ger",
                @"TENADO CAD SE 16\Ger"
            };

            string path = null;

            foreach (var i in products)
            {
                if ((path = GetPath(i)).Length > 0)
                    folders.Add(path);
            }

            if (folders.Count == 0)
            {
                MessageBox.Show(error_msg, "T-FLEX");
                return false;
            }

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            return true;
        }

        /// <summary>
        /// Initialize T-FLEX CAD API session.
        /// </summary>
        public static bool InitSession()
        {
            if (!IsLoaded)
            {
                throw new FileNotFoundException(error_msg);
            }

            ApplicationSessionSetup setup = new ApplicationSessionSetup
            {
                ReadOnly = false,
                ProtectionLicense = ApplicationSessionSetup.License.Auto
            };

            if (Application.InitSession(setup))
            {
                //Debug.WriteLine(string.Format("InitSession [product: {0}, language: {1}]",
                //    Application.Product,
                //    Application.InterfaceLanguage));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Exit the T-FLEX CAD API session.
        /// </summary>
        public static void ExitSession()
        {
            Application.ExitSession();
            //Debug.WriteLine("ExitSession");
        }
        #endregion

        #region private methods
        private static string GetPath(string product)
        {
            string path = string.Empty;

            if (string.IsNullOrEmpty(product))
                return path;

            string regPath  = @"SOFTWARE\Top Systems\" + product;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath,
                RegistryKeyPermissionCheck.ReadSubTree,
                RegistryRights.ReadKey);

            if (key == null)
                return path;

            if (API_VERSION != (string)key.GetValue("SetupProductVersion"))
            {
                key.Close();
                return path;
            }

            path = (string)key.GetValue("SetupHelpPath", string.Empty);
            key.Close();

            if (path.Length > 0 && path[path.Length - 1] != '\\')
                path += @"\";

            return path;
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (folders == null || folders.Count == 0)
                return null;

            try
            {
                Assembly assembly;
                string name = args.Name;
#pragma warning disable CA1307
                int index = name.IndexOf(",");
#pragma warning restore CA1307
                if (index > 0)
                    name = name.Substring(0, index);

                foreach (string path in folders)
                {
                    if (!Directory.Exists(path))
                        continue;

                    string fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}.dll", path, name);

                    if (!File.Exists(fileName))
                        continue;

                    Directory.SetCurrentDirectory(path);

                    if ((assembly = Assembly.LoadFile(fileName)) != null)
                    {
                        IsLoaded = true;

                        //Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                        //    "AssemblyResolve [assembly loaded: {0}]",
                        //    assembly.FullName));
                    }

                    return assembly;
                }
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                    "Error loading assembly {0}.\n\nDescription:\n{1}", 
                    args.Name, e.Message),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }

            return null;
        }
        #endregion
    }
}