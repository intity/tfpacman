using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

#pragma warning disable CA1303
#pragma warning disable CA1305
#pragma warning disable CA1307

namespace TFlex
{
    public static class APILoader
    {
        #region private fields
        private static string folder;
        private static Version version;
        private static TextWriterTraceListener trace;
        #endregion

        #region public methods
        /// <summary>
        /// Initialize T-FLEX CAD API.
        /// </summary>
        public static void Initialize(bool debug)
        {
            if (debug)
            {
                var s = new StreamWriter("api-loader.log");
                trace = new TextWriterTraceListener(s)
                {
                    Name = "myListener"
                };
            }

            version = new Version("17.1.0.0"); // minimum supported version

            if ((folder = GetFolder()) == null)
            {
                throw new FileNotFoundException("T-FLEX CAD not installed");
            }

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            if (!InitializeAPI())
            {
                throw new InvalidOperationException("Initialize API failed");
            }

            trace?.Flush();
            trace?.Close();
        }

        /// <summary>
        /// Terminate T-FLEX CAD API session.
        /// </summary>
        public static void Terminate()
        {
            if (folder != null)
            {
                Application.ExitSession();
                folder = null;
            }
        }
        #endregion

        #region private methods
        private static bool InitializeAPI()
        {
            bool result;
            var setup = new ApplicationSessionSetup
            {
                ReadOnly = false
            };

            trace?.WriteLine("T-FLEX CAD APILoader Info\n");

            if (result = Application.InitSession(setup))
            {
                trace?.WriteLine($"Culture              : {Application.Culture}");
                trace?.WriteLine($"FileLinksAutoRefresh : {Application.FileLinksAutoRefresh}");
                trace?.WriteLine($"IsDOCsEnabled        : {Application.IsDOCsEnabled}");
                trace?.WriteLine($"IsMacrosEnabled      : {Application.IsMacrosEnabled}");
                trace?.WriteLine($"IsSessionInitialized : {Application.IsSessionInitialized}");
                trace?.WriteLine($"InterfaceLanguage    : {Application.InterfaceLanguage}");
                trace?.WriteLine($"MeasuringSystem      : {Application.MeasuringSystem}");
                trace?.WriteLine($"Product              : {Application.Product}");
                trace?.WriteLine($"RegistryName         : {Application.RegistryName}");
                trace?.WriteLine($"StudentVersion       : {Application.StudentVersion}");
                trace?.WriteLine($"SystemPath           : {Application.SystemPath}");
                trace?.WriteLine($"Version              : {Application.Version}");
            }
            else
            {
                trace?.WriteLine("Failed to initialize API...");
            }

            return result;
        }

        private static string GetFolder()
        {
            var root = @"SOFTWARE\Top Systems\";
            var rootKey = Registry.LocalMachine.OpenSubKey(root);
            if (rootKey == null)
                return null;

            string result = null;
            using (rootKey)
            {
                foreach (var product in rootKey.GetSubKeyNames())
                {
                    switch (product)
                    {
                        case "T-FLEX CAD 3D 17":
                        case "T-FLEX CAD SE 17":
                        case "TENADO CAD 3D 17":
                        case "TENADO CAD SE 17":
                            if (result == null)
                                result = GetPath_1(rootKey, product);
                            break;
                    }
                }
            }

            rootKey.Close();
            return result;
        }

        private static string GetPath_1(RegistryKey rootKey, string product)
        {
            var pKey = rootKey.OpenSubKey(product);
            if (pKey == null)
                return null;

            string result = null;
            foreach (var language in pKey.GetSubKeyNames())
            {
                switch (language)
                {
                    case "Rus":
                    case "Eng":
                    case "Ger":
                        if (result == null)
                            result = GetPath_2(pKey, language);
                        break;
                }
            }

            pKey.Close();
            return result;
        }

        private static string GetPath_2(RegistryKey pKey, string language)
        {
            string path = null;

            var lKey = pKey.OpenSubKey(language);
            if (lKey == null)
                return null;

            var pv = new Version((string)lKey.GetValue("SetupProductVersion"));
            if (pv.Major == version.Major && pv.Build >= version.Build)
            {
                path = (string)lKey.GetValue("SetupHelpPath");
                if (!Directory.Exists(path))
                    path = null;
            }

            lKey.Close();
            return path;
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (folder == null)
                return null;

            try
            {
                string name = args.Name;
                int index = name.IndexOf(",", StringComparison.InvariantCulture);
                if (index > 0)
                {
                    name = name.Substring(0, index);
                }

                if (!Path.HasExtension(name))
                {
                    name = Path.ChangeExtension(name, "dll");
                }

                var path = Path.Combine(folder, name);
                if (!File.Exists(path))
                {
                    return null;
                }

                return Assembly.LoadFrom(path);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
        #endregion
    }
}