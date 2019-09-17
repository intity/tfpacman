using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The class for file output category definition.
    /// </summary>
    [CustomCategoryOrder(Resource.CATEGIRY_4, 4)]
    public class Category_3 : Translator, INotifyDataErrorInfo
    {
        #region private fields
        string targetExtension;
        string fileNameSuffix;
        string templateFileName;

        readonly string[] error_messages;
        readonly Dictionary<string, List<string>> objErrors;

        XAttribute data_3_1;
        XAttribute data_3_2;
        XAttribute data_3_3;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extension">Target extension the file.</param>
        public Category_3(string extension)
        {
            targetExtension  = extension;
            fileNameSuffix   = string.Empty;
            templateFileName = string.Empty;

            objErrors        = new Dictionary<string, List<string>>();
            error_messages   = new string[]
            {
                Resource.GetString(Resource.CATEGIRY_4, "message2", 0),
                Resource.GetString(Resource.CATEGIRY_4, "message3", 0)
            };
        }

        #region public properties
        /// <summary>
        /// The target extension.
        /// </summary>
        [Browsable(false)]
        public string TargetExtension
        {
            get { return targetExtension; }
            set
            {
                if (targetExtension != value)
                {
                    targetExtension = value;
                    data_3_1.Value = value;

                    OnPropertyChanged("TargetExtension");
                }
            }
        }

        /// <summary>
        /// The file name suffix.
        /// </summary>
        [PropertyOrder(13)]
        [CustomCategory(Resource.CATEGIRY_4, "category4")]
        [CustomDisplayName(Resource.CATEGIRY_4, "dn4_2")]
        [CustomDescription(Resource.CATEGIRY_4, "dn4_2")]
        [Editor(typeof(CustomTextBoxEditor), typeof(UITypeEditor))]
        public string FileNameSuffix
        {
            get { return fileNameSuffix; }
            set
            {
                if (fileNameSuffix != value)
                {
                    var name = "FileNameSuffix";

                    fileNameSuffix = value;
                    data_3_2.Value = value;

                    char[] pattern = Path.GetInvalidFileNameChars();
                    string error = string
                        .Format(error_messages[0], pattern.ToString(""));

                    if (IsPathValid(value, pattern))
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
        }

        /// <summary>
        /// Template name of the file definition.
        /// </summary>
        [PropertyOrder(14)]
        [CustomCategory(Resource.CATEGIRY_4, "category4")]
        [CustomDisplayName(Resource.CATEGIRY_4, "dn4_3")]
        [CustomDescription(Resource.CATEGIRY_4, "dn4_3")]
        [Editor(typeof(CustomTextBoxEditor), typeof(UITypeEditor))]
        public string TemplateFileName
        {
            get { return templateFileName; }
            set
            {
                if (templateFileName != value)
                {
                    var name = "TemplateFileName";

                    templateFileName = value;
                    data_3_3.Value = value;

                    string path = value;
                    char[] pattern = Path.GetInvalidFileNameChars();
                    string error = string
                        .Format(error_messages[1], pattern.ToString(""));

                    foreach (Match i in Regex.Matches(value, @"\{(.*?)\}"))
                    {
                        path = path.Replace(i.Value, "");
                    }

                    if (IsPathValid(path, pattern))
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
        }
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_3_1 = new XAttribute("value", TargetExtension);
            data_3_2 = new XAttribute("value", FileNameSuffix);
            data_3_3 = new XAttribute("value", TemplateFileName);

            data.Add(new XElement("parameter",
                new XAttribute("name", "TargetExtension"),
                data_3_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "FileNameSuffix"),
                data_3_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "TemplateFileName"),
                data_3_3));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "TargetExtension":
                    targetExtension = a.Value;
                    data_3_1 = a;
                    break;
                case "FileNameSuffix":
                    fileNameSuffix = a.Value;
                    data_3_2 = a;
                    break;
                case "TemplateFileName":
                    templateFileName = a.Value;
                    data_3_3 = a;
                    break;
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Validating the path name.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static bool IsPathValid(string path, char[] pattern)
        {
            if (path.Length > 0 && path.IndexOfAny(pattern) >= 0)
                return false;

            return true;
        }
        #endregion

        #region INotifyDataErrorInfo members
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
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        [Browsable(false)]
        public bool HasErrors
        {
            get { return (objErrors.Count > 0); }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return objErrors.Values;

            objErrors.TryGetValue(name, out List<string> errors);
            return errors;
        }

        /// <summary>
        /// Add error to dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="error">Error message.</param>
        internal void AddError(string name, string error)
        {
            if (objErrors.TryGetValue(name, out List<string> errors) == false)
            {
                errors = new List<string>();
                objErrors.Add(name, errors);
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
            if (objErrors.TryGetValue(name, out List<string> errors))
            {
                errors.Remove(error);
            }

            if (errors == null)
                return;

            if (errors.Count == 0)
            {
                objErrors.Remove(name);
                OnRaiseErrorChanged(name);
            }
        }
        #endregion
    }
}
