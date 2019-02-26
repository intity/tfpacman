using System;
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Resources;

namespace TFlex.PackageManager.Common
{
    public class Resource
    {
        #region private fields
        private static readonly string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string appName = string.Format("T-FLEX Package Manager {0}", Application.Version.Major);
        private static string userDirectory = 
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Top Systems\" + appName + @"\Configurations";
        #endregion

        #region constants
        public const string ABOUT_US    = "AboutUs";
        public const string HEADER_UI   = "Header";
        public const string MAIN_WINDOW = "PackageManager";
        public const string LIST_VALUES = "ListValues";
        public const string OPTIONS_UI  = "Options";
        public const string PACKAGE_0   = "Package0";
        public const string PACKAGE_1   = "Package1";
        public const string PACKAGE_3   = "Package3";
        public const string PACKAGE_9   = "Package9";

        public const string BASE_URI = @"pack://application:,,,/TFlex.PackageManager.UI;component/Resources/";
        #endregion

        #region properties
        /// <summary>
        /// Application name.
        /// </summary>
        public static string AppName { get { return (appName); } }

        /// <summary>
        /// The root registry key path of the configurations.
        /// </summary>
        public static string RootKey
        {
            get
            {
                string rootKey = @"Software\Top Systems\" + appName;

                switch (Application.InterfaceLanguage)
                {
                    case Application.Language.Russian:
                        rootKey += @"\Rus";
                        break;
                    case Application.Language.English:
                        rootKey += @"\Eng";
                        break;
                    case Application.Language.German:
                        rootKey += @"\Ger";
                        break;
                }

                return rootKey;
            }
        }

        /// <summary>
        /// Application directory.
        /// </summary>
        public static string AppDirectory { get { return (appDirectory); } }

        /// <summary>
        /// User directory.
        /// </summary>
        public static string UserDirectory
        {
            get { return userDirectory; }
            set
            {
                if (userDirectory != value)
                    userDirectory = value;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// The method for get resource data.
        /// </summary>
        /// <param name="fileName">The resource file name.</param>
        /// <param name="name">The resource name.</param>
        /// <param name="flag">Define: (0) - value, (1) - comment.</param>
        /// <returns>Gets string value.</returns>
        public static string GetString(string fileName, string name, int flag)
        {
            string path = null;
            string result = null;

            switch (Application.InterfaceLanguage)
            {
                case Application.Language.Russian:
                    path = appDirectory + @"\Resources\ru\" + fileName + ".resx";
                    break;
                case Application.Language.English:
                    path = appDirectory + @"\Resources\en\" + fileName + ".resx";
                    break;
                case Application.Language.German:
                    path = appDirectory + @"\Resources\de\" + fileName + ".resx";
                    break;
                case Application.Language.Polish:
                    path = appDirectory + @"\Resources\pl\" + fileName + ".resx";
                    break;
            }

            using (var resource = new ResXResourceReader(path))
            {
                resource.UseResXDataNodes = true;
                IDictionaryEnumerator dict = resource.GetEnumerator();

                while (dict.MoveNext())
                {
                    var node = dict.Value as ResXDataNode;

                    if (node.Name == name)
                    {
                        switch (flag)
                        {
                            case 0:
                                result = node.GetValue((ITypeResolutionService)null).ToString();
                                break;
                            case 1:
                                result = node.Comment ?? string.Empty;
                                break;
                        }
                        resource.Close();
                        return result;
                    }
                }
                resource.Close();
            }
            return result;
        }
        #endregion
    }
}
