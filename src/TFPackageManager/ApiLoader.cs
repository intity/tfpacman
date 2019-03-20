using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows;
using Microsoft.Win32;

namespace TFlex
{
    public class ApiLoader
    {
        #region private fields
        private const string API_VERSION = "16.0.35.0";
        private static List<string> folders;
        private static bool isLoaded;
        #endregion

        #region public methods
        /// <summary>
        /// Preload T-FLEX API.
        /// </summary>
        public static void Preload()
        {
            folders = new List<string>();

            string[] products = new string[]
            {
                @"T-FLEX CAD 3D 16\Rus",
                @"T-FLEX CAD 3D 16\Eng",
                @"TENADO CAD 3D 16\Ger"
            };

            string path = null;

            foreach (var i in products)
            {
                if ((path = GetPath(i)).Length > 0)
                    folders.Add(path);
            }

            if (folders.Count == 0)
            {
                throw new FileNotFoundException("T-FLEX CAD not installed");
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
        }

        /// <summary>
        /// Initialize T-FLEX API.
        /// </summary>
        /// <returns></returns>
        public static bool InitSession()
        {
            if (!isLoaded)
            {
                throw new FileNotFoundException("T-FLEX API not installed");
            }

            ApplicationSessionSetup setup = new ApplicationSessionSetup
            {
                ReadOnly = false,
                ProtectionLicense = ApplicationSessionSetup.License.Auto
            };

            if (Application.InitSession(setup))
            {
                Debug.WriteLine(string.Format("InitSession [product: {0}, language: {1}]",
                    Application.Product,
                    Application.InterfaceLanguage));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Exit the T-FLEX API session.
        /// </summary>
        public static void ExitSession()
        {
            Application.ExitSession();
            Debug.WriteLine("ExitSession");
        }
        #endregion

        #region private methods
        private static string GetPath(string product)
        {
            string path = string.Empty;

            if (string.IsNullOrEmpty(product))
                return path;

            string regPath  = string.Format(@"SOFTWARE\Top Systems\{0}\", product);
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath,
                RegistryKeyPermissionCheck.ReadSubTree,
                RegistryRights.ReadKey);

            if (key == null || API_VERSION != (string)key.GetValue("SetupProductVersion"))
                return path;

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

                int index = name.IndexOf(",");
                if (index > 0)
                    name = name.Substring(0, index);

                foreach (string path in folders)
                {
                    if (!Directory.Exists(path))
                        continue;

                    string fileName = string.Format("{0}{1}.dll", path, name);

                    if (!File.Exists(fileName))
                        continue;

                    Directory.SetCurrentDirectory(path);

                    if ((assembly = Assembly.LoadFile(fileName)) != null)
                    {
                        isLoaded = true;

                        Debug.WriteLine(string.Format("AssemblyResolve [assembly loaded: {0}]",
                            assembly.FullName));
                    }

                    return assembly;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error loading assembly {0}.\n\nDescription:\n{1}", 
                    args.Name, ex.Message),
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