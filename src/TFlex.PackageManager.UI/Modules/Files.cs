using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Files extension modules.
    /// </summary>
    [CustomCategoryOrder(Resources.FILES, 4), Serializable]
    public class Files : Translator
    {
        #region private fields
        string iExtension;
        string oExtension;
        string fileNameSuffix;
        string templateFileName;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Files()
        {
            iExtension       = ".grb";
            oExtension       = ".grb";
            fileNameSuffix   = string.Empty;
            templateFileName = string.Empty;
        }

        #region internal properties
        /// <summary>
        /// The input file extension.
        /// </summary>
        internal string IExtension
        {
            get => iExtension;
            set
            {
                if (iExtension != value)
                {
                    iExtension = value;
                    OnPropertyChanged("IExtension");
                }
            }
        }

        /// <summary>
        /// The output file extension.
        /// </summary>
        internal string OExtension
        {
            get => oExtension;
            set
            {
                if (oExtension != value)
                {
                    oExtension = value;
                    OnPropertyChanged("OExtension");
                }
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// The file name suffix.
        /// </summary>
        [PropertyOrder(13)]
        [CustomCategory(Resources.FILES, "category4")]
        [CustomDisplayName(Resources.FILES, "dn4_2")]
        [CustomDescription(Resources.FILES, "dn4_2")]
        [Editor(typeof(CustomTextBoxEditor), typeof(UITypeEditor))]
        public string FileNameSuffix
        {
            get => fileNameSuffix;
            set
            {
                if (fileNameSuffix == value)
                    return;

                fileNameSuffix = value;

                string name = "FileNameSuffix";
                char[] pattern = Path.GetInvalidFileNameChars();
                string error = string.Format(Resources.Strings["md_4:msg_1"][0], 
                    pattern.ToString(""));

                if (value.IsValid(pattern))
                {
                    RemoveError(name, error);
                }
                else
                {
                    AddError(name, error);
                }
                OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// Template name of the file definition.
        /// </summary>
        [PropertyOrder(14)]
        [CustomCategory(Resources.FILES, "category4")]
        [CustomDisplayName(Resources.FILES, "dn4_3")]
        [CustomDescription(Resources.FILES, "dn4_3")]
        [Editor(typeof(CustomTextBoxEditor), typeof(UITypeEditor))]
        public string TemplateFileName
        {
            get => templateFileName;
            set
            {
                if (templateFileName == value)
                    return;

                templateFileName = value;

                string name = "TemplateFileName";
                string path = value;
                char[] pattern = Path.GetInvalidFileNameChars();
                string error = string.Format(Resources.Strings["md_4:msg_2"][0], 
                    pattern.ToString(""));

                foreach (Match i in Regex.Matches(value, @"\{(.*?)\}"))
                {
                    path = path.Replace(i.Value, "");
                }
                if (path.IsValid(pattern))
                {
                    RemoveError(name, error);
                }
                else
                {
                    AddError(name, error);
                }
                OnPropertyChanged(name);
            }
        }
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 4 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "IExtension":
                        iExtension = reader.GetAttribute(1);
                        break;
                    case "OExtension":
                        oExtension = reader.GetAttribute(1);
                        break;
                    case "FileNameSuffix":
                        fileNameSuffix = reader.GetAttribute(1);
                        break;
                    case "TemplateFileName":
                        templateFileName = reader.GetAttribute(1);
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "IExtension");
            writer.WriteAttributeString("value", IExtension);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "OExtension");
            writer.WriteAttributeString("value", OExtension);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "FileNameSuffix");
            writer.WriteAttributeString("value", FileNameSuffix);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "TemplateFileName");
            writer.WriteAttributeString("value", TemplateFileName);
            writer.WriteEndElement();
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Get output file name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns>Returns Output File name.</returns>
        internal string GetFileName(Document document, Page page)
        {
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (TemplateFileName.Length > 0)
                fileName = TemplateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (FileNameSuffix.Length > 0)
                    fileName += ParseExpression(document, page, FileNameSuffix);
                return fileName;
            }

            foreach (Match i in Regex.Matches(fileName, pattern))
            {
                if ((expVal = ParseExpression(document, page, i.Groups[1].Value)) == null)
                    continue;

                fileName = fileName.Replace(i.Groups[0].Value, expVal);
            }

            return fileName;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Extension method to split expression on tokens.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Returns expression tokens.</returns>
        private static string[] Groups(string expression)
        {
            string pattern = @"\((.*?)\)";
            string[] groups = expression.Split(new string[] { ".type", ".format" },
                StringSplitOptions.None);

            if (groups.Length > 1)
            {
                groups[1] = Regex.Match(expression, pattern).Groups[1].Value;
            }

            return groups;
        }

        /// <summary>
        /// Extension method to parse expression tokens.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns>Returns variable value from document.</returns>
        private static string GetValue(Document document, Page page, string expression)
        {
            string result = null;
            string[] groups = Groups(expression);
            string[] argv;
            Variable variable;

            if (groups.Length > 1)
            {
                if (expression.Contains("page.type") && page != null)
                {
                    if ((argv = groups[1].Split(',')).Length < 2)
                        return result;

                    switch (argv[0])
                    {
                        case "0":
                            if (page.PageType == PageType.Normal)
                                result = argv[1];
                            break;
                        case "1":
                            if (page.PageType == PageType.Workplane)
                                result = argv[1];
                            break;
                        case "3":
                            if (page.PageType == PageType.Auxiliary)
                                result = argv[1];
                            break;
                        case "4":
                            if (page.PageType == PageType.Text)
                                result = argv[1];
                            break;
                        case "5":
                            if (page.PageType == PageType.BillOfMaterials)
                                result = argv[1];
                            break;
                    }
                }
                else if ((variable = document.FindVariable(groups[0])) != null)
                {
                    result = variable.IsText
                        ? variable.TextValue
                        : variable.RealValue.ToString(groups[1]);
                }
            }
            else
            {
                if ((variable = document.FindVariable(expression)) != null)
                {
                    result = variable.IsText
                        ? variable.TextValue
                        : variable.RealValue.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Parse expression.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns>Returns variable value from document.</returns>
        private static string ParseExpression(Document document, Page page, string expression)
        {
            string result = null;
            string pattern = @"(?:\?\?)";
            string[] tokens = new string[] { "??" };
            string[] group = expression.Split(tokens, StringSplitOptions.None).ToArray();
            MatchCollection matches = Regex.Matches(expression, pattern);

            if (group.Length > 1)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    switch (matches[i].Value)
                    {
                        case "??":

                            if ((result = GetValue(document, page, group[i])) != null ||
                                (result = GetValue(document, page, group[i + 1])) != null)
                            {
                                return result;
                            }
                            else
                                result = group[i + 1];
                            break;
                    }
                }
            }
            else
                result = GetValue(document, page, expression);

            return result ?? string.Empty;
        }
        #endregion
    }
}
