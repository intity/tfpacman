using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows;
using Microsoft.Win32;

namespace TFlex
{
    public class ApiLoader
    {
        private static List<string> folders = null;

        public static void PreLoad()
        {
            if (folders != null)
                return;
            else
                folders = new List<string>();

            string path = GetPath(@"T-FLEX CAD 3D 16\Rus");
            if (string.IsNullOrEmpty(path))
                throw new System.IO.FileNotFoundException("T-FLEX CAD not installed");

            folders.Add(path);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
        }

        public static bool InitSession()
        {
            if (folders == null)
                throw new InvalidOperationException("Call Initialize first");

            ApplicationSessionSetup setup = new ApplicationSessionSetup
            {
                ReadOnly = false,
                ProtectionLicense = ApplicationSessionSetup.License.Auto
            };
            return Application.InitSession(setup);
        }

        public static void ExitSession()
        {
            if (folders == null)
                return;

            Application.ExitSession();
            folders = null;
        }

        private static string GetPath(string product)
        {
            string path = "";

            if (string.IsNullOrEmpty(product))
                return path;

            string regPath = string.Format(@"SOFTWARE\Top Systems\{0}\", product);

            RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath,
                RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

            if (key == null)
                return path;

            if (string.IsNullOrEmpty(path))
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
                string name = args.Name;

                int index = name.IndexOf(",");
                if (index > 0)
                    name = name.Substring(0, index);

                foreach (string path in folders)
                {
                    if (!System.IO.Directory.Exists(path))
                        continue;

                    string fileName = string.Format("{0}{1}.dll", path, name);

                    if (!System.IO.File.Exists(fileName))
                        continue;

                    System.IO.Directory.SetCurrentDirectory(path);
                    return Assembly.LoadFile(fileName);
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
    }
}
