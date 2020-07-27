using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Properties
{
    public class Resources
    {
        #region constants
        public const string ABOUT_US      = "AboutUs.resx";
        public const string HEADER_UI     = "Header.resx";
        public const string MODULES_UI    = "Modules.resx";
        public const string LINKS         = "Links.resx";
        public const string FILES         = "Files.resx";
        public const string MAIN_WINDOW   = "Main.resx";
        public const string LIST_VALUES   = "ListValues.resx";
        public const string OPTIONS_UI    = "Options.resx";
        public const string VARIABLES_UI  = "VariablesUI.resx";
        public const string TRANSLATOR_0  = "Translator_0.resx";
        public const string TRANSLATOR_1  = "Translator_1.resx";
        public const string TRANSLATOR_3  = "Translator_3.resx";
        public const string TRANSLATOR_6  = "Translator_6.resx";
        public const string TRANSLATOR_7  = "Translator_7.resx";
        public const string TRANSLATOR_9  = "Translator_9.resx";
        public const string TRANSLATOR_10 = "Translator_10.resx";
        public const string TRANSLATOR_3D = "Translator_3D.resx";

        public const string BASE_URI = @"pack://application:,,,/TFlex.PackageManager.UI;component/Resources/";
        public const string LOG_FILE = "processing.log";
        #endregion

        #region properties
        /// <summary>
        /// Application name.
        /// </summary>
        public static string AppName
        {
            get
            {
                string result  = null;
                string version = Application.Version.Major.ToString();
                switch (Application.InterfaceLanguage)
                {
                    case Application.Language.Russian:
                    case Application.Language.English:
                        result = string.Format("T-FLEX Package Manager {0}", version);
                        break;
                    case Application.Language.German:
                        result = string.Format("TENADO Package Manager {0}", version);
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// The root registry key path of the configurations.
        /// </summary>
        public static string RootKey
        {
            get
            {
                string rootKey = @"Software\Top Systems\" + AppName;
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
        public static string AppDirectory => Path
            .GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// User directory.
        /// </summary>
        public static string UserDirectory => Path
            .Combine(Environment
            .GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "Top Systems", AppName, "Configurations");

        /// <summary>
        /// String resources.
        /// </summary>
        public static Dictionary<string, string[]> Strings => new Dictionary<string, string[]>
        {
            // Main
            { "ui_0:msg_1", GetString(MAIN_WINDOW, "message1") },
            { "ui_0:msg_2", GetString(MAIN_WINDOW, "message2") },
            { "ui_0:tbl_1", GetString(MAIN_WINDOW, "tb_label1") },
            { "ui_0:tbl_2", GetString(MAIN_WINDOW, "tb_label2") },
            { "ui_0:sbl_1", GetString(MAIN_WINDOW, "sb_label1") },
            { "ui_0:sbl_2", GetString(MAIN_WINDOW, "sb_label2") },
            { "ui_0:sbl_3", GetString(MAIN_WINDOW, "sb_label3") },
            { "ui_0:ctl_1", GetString(MAIN_WINDOW, "menuItem1") },
            { "ui_0:ctl_2", GetString(MAIN_WINDOW, "menuItem2") },
            { "ui_0:ctl_3", GetString(MAIN_WINDOW, "menuItem3") },
            { "ui_0:ctl_4", GetString(MAIN_WINDOW, "menuItem4") },
            { "ui_0:c_1_1", GetString(MAIN_WINDOW, "menuItem1_1") },
            { "ui_0:c_1_2", GetString(MAIN_WINDOW, "menuItem1_2") },
            { "ui_0:c_1_3", GetString(MAIN_WINDOW, "menuItem1_3") },
            { "ui_0:c_1_4", GetString(MAIN_WINDOW, "menuItem1_4") },
            { "ui_0:c_1_5", GetString(MAIN_WINDOW, "menuItem1_5") },
            { "ui_0:c_1_6", GetString(MAIN_WINDOW, "menuItem1_6") },
            { "ui_0:c_1_7", GetString(MAIN_WINDOW, "menuItem1_7") },
            { "ui_0:c_1_8", GetString(MAIN_WINDOW, "menuItem1_8") },
            { "ui_0:c_2_1", GetString(MAIN_WINDOW, "menuItem2_1") },
            { "ui_0:c_2_2", GetString(MAIN_WINDOW, "menuItem2_2") },
            { "ui_0:c_3_1", GetString(MAIN_WINDOW, "menuItem3_1") },
            { "ui_0:c_3_2", GetString(MAIN_WINDOW, "menuItem3_2") },
            { "ui_0:c_3_3", GetString(MAIN_WINDOW, "menuItem3_3") },
            { "ui_0:c_4_1", GetString(MAIN_WINDOW, "menuItem4_1") },
            { "ui_0:c_5_1", GetString(MAIN_WINDOW, "menuItem5_1") },
            { "ui_0:c_5_2", GetString(MAIN_WINDOW, "menuItem5_2") },
            // AboutUs
            { "ui_1:title", GetString(ABOUT_US, "title") },
            { "ui_1:lab_1", GetString(ABOUT_US, "label1") },
            { "ui_1:lab_2", GetString(ABOUT_US, "label2") },
            // ListValues
            { "ui_2:title", GetString(LIST_VALUES, "title") },
            { "ui_2:cbx_1", GetString(LIST_VALUES, "checkBox") },
            { "ui_2:btn_1", GetString(LIST_VALUES, "button1") },
            { "ui_2:btn_2", GetString(LIST_VALUES, "button2") },
            // VariablesUI
            { "ui_3:btn_1", GetString(VARIABLES_UI, "button1") },
            { "ui_3:btn_2", GetString(VARIABLES_UI, "button2") },
            { "ui_3:col_1", GetString(VARIABLES_UI, "column1") },
            { "ui_3:col_2", GetString(VARIABLES_UI, "column2") },
            { "ui_3:col_3", GetString(VARIABLES_UI, "column3") },
            { "ui_3:col_4", GetString(VARIABLES_UI, "column4") },
            { "ui_3:col_5", GetString(VARIABLES_UI, "column5") },
            { "ui_3:msg_1", GetString(VARIABLES_UI, "message1") },
            { "ui_3:msg_2", GetString(VARIABLES_UI, "message2") },
            { "ui_3:msg_3", GetString(VARIABLES_UI, "message3") },
            { "ui_3:msg_4", GetString(VARIABLES_UI, "message4") },
            { "ui_3:msg_5", GetString(VARIABLES_UI, "message5") },
            // Links
            { "link:msg_1", GetString(LINKS, "message1") },
            // OutputFiles
            { "file:msg_1", GetString(FILES, "message1") },
            { "file:msg_2", GetString(FILES, "message2") },
            // Translator_0
            { "tr_0:1_3_1", GetString(TRANSLATOR_0, "dn1_3_1") },
            { "tr_0:1_3_2", GetString(TRANSLATOR_0, "dn1_3_2") },
            // Translator_1
            { "tr_1:5_1_0", GetString(TRANSLATOR_1, "dn5_1_0") },
            { "tr_1:5_1_1", GetString(TRANSLATOR_1, "dn5_1_1") },
            { "tr_1:5_2_0", GetString(TRANSLATOR_1, "dn5_2_0") },
            { "tr_1:5_2_1", GetString(TRANSLATOR_1, "dn5_2_1") },
            { "tr_1:5_3_0", GetString(TRANSLATOR_1, "dn5_3_0") },
            { "tr_1:5_3_1", GetString(TRANSLATOR_1, "dn5_3_1") },
            { "tr_1:5_3_2", GetString(TRANSLATOR_1, "dn5_3_2") },
            { "tr_1:5_4_0", GetString(TRANSLATOR_1, "dn5_4_0") },
            { "tr_1:5_4_1", GetString(TRANSLATOR_1, "dn5_4_1") },
            { "tr_1:5_5_0", GetString(TRANSLATOR_1, "dn5_5_0") },
            { "tr_1:5_5_1", GetString(TRANSLATOR_1, "dn5_5_1") },
            { "tr_1:5_5_2", GetString(TRANSLATOR_1, "dn5_5_2") },
            { "tr_1:5_6_0", GetString(TRANSLATOR_1, "dn5_6_0") },
            { "tr_1:5_6_1", GetString(TRANSLATOR_1, "dn5_6_1") },
            // Translator_3D
            { "tr3d:5_1_0", GetString(TRANSLATOR_3D, "dn5_1_0") },
            { "tr3d:5_1_1", GetString(TRANSLATOR_3D, "dn5_1_1") },
            { "tr3d:5_2_0", GetString(TRANSLATOR_3D, "dn5_2_0") },
            { "tr3d:5_2_1", GetString(TRANSLATOR_3D, "dn5_2_1") },
            { "tr3d:6_1_0", GetString(TRANSLATOR_3D, "dn6_1_0") },
            { "tr3d:6_1_1", GetString(TRANSLATOR_3D, "dn6_1_1") },
            { "tr3d:6_1_2", GetString(TRANSLATOR_3D, "dn6_1_2") },
            { "tr3d:6_2_0", GetString(TRANSLATOR_3D, "dn6_2_0") },
            { "tr3d:6_2_1", GetString(TRANSLATOR_3D, "dn6_2_1") },
            { "tr3d:6_2_2", GetString(TRANSLATOR_3D, "dn6_2_2") },
            { "tr3d:6_3_0", GetString(TRANSLATOR_3D, "dn6_3_0") },
            { "tr3d:6_3_1", GetString(TRANSLATOR_3D, "dn6_3_1") },
            { "tr3d:6_3_2", GetString(TRANSLATOR_3D, "dn6_3_2") },
        };
        #endregion

        #region methods
        /// <summary>
        /// Get string resource.
        /// </summary>
        /// <param name="fileName">The resource file name.</param>
        /// <param name="name">The resource name.</param>
        /// <returns>
        /// Gets string array [0: value, 1: comment].
        /// </returns>
        public static string[] GetString(string fileName, string name)
        {
            string path = null;
            string[] result = null;
            ResXResourceReader resource = null;

            switch (Application.InterfaceLanguage)
            {
                case Application.Language.Russian:
                    path = Path.Combine(AppDirectory, "Resources", "ru", fileName);
                    break;
                case Application.Language.English:
                    path = Path.Combine(AppDirectory, "Resources", "en", fileName);
                    break;
                case Application.Language.German:
                    path = Path.Combine(AppDirectory, "Resources", "de", fileName);
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
                        result = new string[]
                        {
                            node.GetValue((ITypeResolutionService)null).ToString(),
                            node.Comment ?? string.Empty
                        };
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