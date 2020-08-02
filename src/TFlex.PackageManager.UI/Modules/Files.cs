using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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
    [CustomCategoryOrder(Resources.FILES, 4)]
    public class Files : Translator, INotifyDataErrorInfo
    {
        #region private fields
        string targetExtension;
        string fileNameSuffix;
        string templateFileName;
        XAttribute data_4_1;
        XAttribute data_4_2;
        XAttribute data_4_3;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extension">Target extension the file.</param>
        public Files(string extension)
        {
            targetExtension  = extension;
            fileNameSuffix   = string.Empty;
            templateFileName = string.Empty;

            Errors = new Dictionary<string, List<string>>();
        }

        #region public properties
        /// <summary>
        /// The target extension.
        /// </summary>
        [Browsable(false)]
        public string TargetExtension
        {
            get => targetExtension;
            set
            {
                if (targetExtension != value)
                {
                    targetExtension = value;
                    data_4_1.Value = value;
                    OnPropertyChanged("TargetExtension");
                }
            }
        }

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
                data_4_2.Value = value;

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
                data_4_3.Value = value;

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

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_1 = new XAttribute("value", TargetExtension);
            data_4_2 = new XAttribute("value", FileNameSuffix);
            data_4_3 = new XAttribute("value", TemplateFileName);

            data.Add(new XElement("parameter",
                new XAttribute("name", "TargetExtension"),
                data_4_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "FileNameSuffix"),
                data_4_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "TemplateFileName"),
                data_4_3));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "TargetExtension":
                    targetExtension = a.Value;
                    data_4_1 = a;
                    break;
                case "FileNameSuffix":
                    fileNameSuffix = a.Value;
                    data_4_2 = a;
                    break;
                case "TemplateFileName":
                    templateFileName = a.Value;
                    data_4_3 = a;
                    break;
            }
        }

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

        #region INotifyDataErrorInfo Members
        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// The RaiseErrorChanged event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnRaiseErrorChanged(string name)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        }

        /// <summary>
        /// Error messages container.
        /// </summary>
        protected Dictionary<string, List<string>> Errors { get; }

        /// <summary>
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        [Browsable(false)]
        public bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Errors.Values;

            Errors.TryGetValue(name, out List<string> errors);
            return errors;
        }

        /// <summary>
        /// Add error to dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="error">Error message.</param>
        internal void AddError(string name, string error)
        {
            if (Errors.TryGetValue(name, out List<string> errors) == false)
            {
                errors = new List<string>();
                Errors.Add(name, errors);
            }

            if (errors.Contains(error) == false)
            {
                errors.Add(error);
            }

            OnRaiseErrorChanged(name);
        }

        /// <summary>
        /// Remove error from dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="error">Error message.</param>
        internal void RemoveError(string name, string error)
        {
            if (Errors.TryGetValue(name, out List<string> errors))
            {
                errors.Remove(error);
            }

            if (errors == null)
                return;

            if (errors.Count == 0)
            {
                Errors.Remove(name);
                OnRaiseErrorChanged(name);
            }
        }
        #endregion
    }
}
