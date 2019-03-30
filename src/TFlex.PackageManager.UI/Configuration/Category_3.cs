using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The class for file output category definition.
    /// </summary>
    [CustomCategoryOrder(Resource.CATEGIRY_3, 3)]
    public class Category_3 : Translator, INotifyDataErrorInfo
    {
        #region private fields
        private string outputExtension;
        private string subDirectoryName;
        private string fileNameSuffix;
        private string templateFileName;

        private readonly byte[] objState;
        private readonly string[] s_values;
        private readonly string[] error_messages;
        private readonly Dictionary<string, List<string>> objErrors;

        private bool isChanged;
        #endregion

        public Category_3()
        {
            outputExtension  = string.Empty;
            subDirectoryName = string.Empty;
            fileNameSuffix   = string.Empty;
            templateFileName = string.Empty;

            objState         = new byte[4];
            s_values         = new string[4];
            objErrors        = new Dictionary<string, List<string>>();
            error_messages   = new string[]
            {
                Resource.GetString(Resource.CATEGIRY_3, "message1", 0),
                Resource.GetString(Resource.CATEGIRY_3, "message2", 0),
                Resource.GetString(Resource.CATEGIRY_3, "message3", 0)
            };
        }

        #region public properties
        /// <summary>
        /// The sub directory name definition for include in the target directory.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resource.CATEGIRY_3, "category3")]
        [CustomDisplayName(Resource.CATEGIRY_3, "dn3_1")]
        [CustomDescription(Resource.CATEGIRY_3, "dn3_1")]
        public string SubDirectoryName
        {
            get { return subDirectoryName; }
            set
            {
                if (subDirectoryName != value)
                {
                    subDirectoryName = value;
                    char[] pattern = Path.GetInvalidPathChars();
                    string error = string.Format(error_messages[0], pattern.ToString(""));

                    if (IsPathValid(value, pattern))
                    {
                        RemoveError("SubDirectoryName", error);
                    }
                    else
                    {
                        AddError("SubDirectoryName", error);
                    }

                    OnChanged(12);
                }
            }
        }

        /// <summary>
        /// The file name suffix.
        /// </summary>
        [PropertyOrder(13)]
        [CustomCategory(Resource.CATEGIRY_3, "category3")]
        [CustomDisplayName(Resource.CATEGIRY_3, "dn3_2")]
        [CustomDescription(Resource.CATEGIRY_3, "dn3_2")]
        public string FileNameSuffix
        {
            get { return fileNameSuffix; }
            set
            {
                if (fileNameSuffix != value)
                {
                    fileNameSuffix = value;
                    char[] pattern = Path.GetInvalidFileNameChars();
                    string error = string.Format(error_messages[1], pattern.ToString(""));

                    if (IsPathValid(value, pattern))
                    {
                        RemoveError("FileNameSuffix", error);
                    }
                    else
                    {
                        AddError("FileNameSuffix", error);
                    }

                    OnChanged(13);
                }
            }
        }

        /// <summary>
        /// Template name of the file definition.
        /// </summary>
        [PropertyOrder(14)]
        [CustomCategory(Resource.CATEGIRY_3, "category3")]
        [CustomDisplayName(Resource.CATEGIRY_3, "dn3_3")]
        [CustomDescription(Resource.CATEGIRY_3, "dn3_3")]
        public string TemplateFileName
        {
            get { return templateFileName; }
            set
            {
                if (templateFileName != value)
                {
                    templateFileName = value;
                    string path = value;
                    char[] pattern = Path.GetInvalidFileNameChars();
                    string error = string.Format(error_messages[2], pattern.ToString(""));

                    foreach (Match i in Regex.Matches(value, @"\{(.*?)\}"))
                    {
                        path = path.Replace(i.Value, "");
                    }

                    if (IsPathValid(path, pattern))
                    {
                        RemoveError("TemplateFileName", error);
                    }
                    else
                    {
                        AddError("TemplateFileName", error);
                    }

                    OnChanged(14);
                }
            }
        }

        /// <summary>
        /// The output file extension.
        /// </summary>
        [Browsable(false)]
        public string OutputExtension
        {
            get { return outputExtension; }
            set
            {
                if (outputExtension != value)
                {
                    outputExtension = value;
                    OnChanged(15);
                }
            }
        }
        #endregion

        #region internal properties
        internal override bool IsChanged
        {
            get { return (isChanged); }
        }
        #endregion

        #region internal methods
        internal override void OnLoaded()
        {
            s_values[0] = subDirectoryName;
            s_values[1] = fileNameSuffix;
            s_values[2] = templateFileName;
            s_values[3] = outputExtension;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 12: objState[1] = (byte)(s_values[0] != subDirectoryName ? 1 : 0); break;
                case 13: objState[2] = (byte)(s_values[1] != fileNameSuffix   ? 1 : 0); break;
                case 14: objState[3] = (byte)(s_values[2] != templateFileName ? 1 : 0); break;
                case 15: objState[0] = (byte)(s_values[3] != outputExtension  ? 1 : 0); break;
            }

            isChanged = false;

            foreach (var i in objState)
            {
                if (i > 0)
                {
                    isChanged = true;
                    break;
                }
            }

            base.OnChanged(index);
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "SubDirectoryName":
                    if (flag == 0)
                        subDirectoryName = value;
                    else
                        value = subDirectoryName;
                    break;
                case "FileNameSuffix":
                    if (flag == 0)
                        fileNameSuffix = value;
                    else
                        value = fileNameSuffix;
                    break;
                case "TemplateFileName":
                    if (flag == 0)
                        templateFileName = value;
                    else
                        value = templateFileName;
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Validating the path name.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private bool IsPathValid(string path, char[] pattern)
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
        protected void RaiseErrorChanged(string name)
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

            RaiseErrorChanged(name);
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
                RaiseErrorChanged(name);
            }
        }
        #endregion
    }
}
