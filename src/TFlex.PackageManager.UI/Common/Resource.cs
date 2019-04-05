using System;
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Common
{
    public static class Resource
    {
        #region private fields
        private static readonly string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string appName = string.Format("T-FLEX Package Manager {0}", Application.Version.Major);
        private static string userDirectory = 
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Top Systems\" + appName + @"\Configurations";
        #endregion

        #region constants
        public const string ABOUT_US      = "AboutUs";
        public const string HEADER_UI     = "Header";
        public const string MAIN_WINDOW   = "PackageManager";
        public const string LIST_VALUES   = "ListValues";
        public const string OPTIONS_UI    = "Options";
        public const string CATEGIRY_3    = "Category_3";
        public const string TRANSLATOR_0  = "Translator_0";
        public const string TRANSLATOR_1  = "Translator_1";
        public const string TRANSLATOR_3  = "Translator_3";
        public const string TRANSLATOR_9  = "Translator_9";
        public const string TRANSLATOR_10 = "Translator_10";
        public const string TRANSLATOR_3D = "Translator_3D";

        public const string BASE_URI = @"pack://application:,,,/TFlex.PackageManager.UI;component/Resources/";
        public const string LOG_FILE = "processing.log";
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
            ResXResourceReader resource = null;

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

            try
            {
                resource = new ResXResourceReader(path)
                {
                    UseResXDataNodes = true
                };

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

                        break;
                    }
                }
                
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (resource != null)
                    resource.Close();
            }
            return result;
        }
        #endregion
    }
}
