using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The translator configuration base class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 1)]
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 2)]
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 3)]
    public class Translator_0 : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region private fields
        private string outputExtension;
        private string [] pageNames;
        private bool excludePage;
        private decimal pageScale;
        private bool savePageScale;
        private PageTypes pageTypes;
        private bool checkDrawingTemplate;
        private string [] projectionNames;
        private bool excludeProjection;
        private decimal projectionScale;
        private bool saveProjectionScale;
        private bool enableProcessingOfProjections;
        private string subDirectoryName;
        private string fileNameSuffix;
        private string templateFileName;

        private string[] pg_names, pj_names;
        private readonly byte[] objState    = new byte[15];
        private readonly bool[] pg_types    = new bool[5];
        private readonly bool[] b_values    = new bool[6];
        private readonly string[] s_values  = new string[4];
        private readonly decimal[] m_values = new decimal[2];
        
        private readonly Dictionary<string, List<string>> objErrors;
        private readonly string[] error_messages;
        private bool isLoaded, isChanged;
        #endregion

        public Translator_0()
        {
            objErrors        = new Dictionary<string, List<string>>();
            error_messages   = new string[]
            {
                Resource.GetString(Resource.TRANSLATOR_0, "message1", 0),
                Resource.GetString(Resource.TRANSLATOR_0, "message2", 0),
                Resource.GetString(Resource.TRANSLATOR_0, "message3", 0)
            };

            outputExtension  = string.Empty;
            pageNames        = new string[] { };
            pageScale        = 99999;
            pageTypes        = new PageTypes { Normal = true };
            pageTypes.PropertyChanged += PageTypes_PropertyChanged;
            projectionNames  = new string[] { };
            projectionScale  = 99999;
            subDirectoryName = string.Empty;
            fileNameSuffix   = string.Empty;
            templateFileName = string.Empty;
        }

        #region event handlers
        private void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChanged(5);
        }
        #endregion

        #region internal properties
        /// <summary>
        /// The package is changed.
        /// </summary>
        internal virtual bool IsChanged
        {
            get { return (isChanged); }
        }

        /// <summary>
        /// The package is loaded.
        /// </summary>
        internal bool IsLoaded
        {
            get { return (isLoaded); }
        }
        #endregion

        #region public properties
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
                    OnChanged(0);
                }
            }
        }

        /// <summary>
        /// The page name list.
        /// </summary>
        [PropertyOrder(1)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_1")]
        [ExpandableObject]
        [Editor(typeof(InputCollectionControl), typeof(UITypeEditor))]
        public string[] PageNames
        {
            get { return pageNames; }
            set
            {
                if (pageNames != value)
                {
                    pageNames = value;
                    OnChanged(1);
                }
            }
        }

        /// <summary>
        /// Exclude the page name from search.
        /// </summary>
        [PropertyOrder(2)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_2")]
        public bool ExcludePage
        {
            get { return excludePage; }
            set
            {
                if (excludePage != value)
                {
                    excludePage = value;
                    OnChanged(2);
                }
            }
        }

        /// <summary>
        /// The page scale value.
        /// </summary>
        [PropertyOrder(3)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_3")]
        [Editor(typeof(InputScaleControl), typeof(UITypeEditor))]
        public decimal PageScale
        {
            get { return pageScale; }
            set
            {
                if (pageScale != value)
                {
                    pageScale = value;
                    OnChanged(3);
                }
            }
        }

        /// <summary>
        /// Save change the page scale value in document.
        /// </summary>
        [PropertyOrder(4)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4")]
        public bool SavePageScale
        {
            get { return savePageScale; }
            set
            {
                if (savePageScale != value)
                {
                    savePageScale = value;
                    OnChanged(4);
                }
            }
        }

        /// <summary>
        /// The page type list.
        /// </summary>
        [PropertyOrder(5)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5")]
        [ExpandableObject]
        [Editor(typeof(PageTypesEditor), typeof(UITypeEditor))]
        public PageTypes PageTypes
        {
            get { return (pageTypes); }
        }

        /// <summary>
        /// Check the drawing template.
        /// </summary>
        [PropertyOrder(6)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_6")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_6")]
        public bool CheckDrawingTemplate
        {
            get { return checkDrawingTemplate; }
            set
            {
                if (checkDrawingTemplate != value)
                {
                    checkDrawingTemplate = value;
                    OnChanged(6);
                }
            }
        }

        /// <summary>
        /// The projection name list.
        /// </summary>
        [PropertyOrder(7)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_1")]
        [ExpandableObject]
        [Editor(typeof(InputCollectionControl), typeof(UITypeEditor))]
        public string[] ProjectionNames
        {
            get { return projectionNames; }
            set
            {
                if (projectionNames != value)
                {
                    projectionNames = value;
                    OnChanged(7);
                }
            }
        }

        /// <summary>
        /// Exclude the projection name from search.
        /// </summary>
        [PropertyOrder(8)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_2")]
        public bool ExcludeProjection
        {
            get { return excludeProjection; }
            set
            {
                if (excludeProjection != value)
                {
                    excludeProjection = value;
                    OnChanged(8);
                }
            }
        }

        /// <summary>
        /// The projection scale value.
        /// </summary>
        [PropertyOrder(9)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_3")]
        [Editor(typeof(InputScaleControl), typeof(UITypeEditor))]
        public decimal ProjectionScale
        {
            get { return projectionScale; }
            set
            {
                if (projectionScale != value)
                {
                    projectionScale = value;
                    OnChanged(9);
                }
            }
        }

        /// <summary>
        /// Save change the projection scale value in document.
        /// </summary>
        [PropertyOrder(10)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_4")]
        public bool SaveProjectionScale
        {
            get { return saveProjectionScale; }
            set
            {
                if (saveProjectionScale != value)
                {
                    saveProjectionScale = value;
                    OnChanged(10);
                }
            }
        }

        /// <summary>
        /// Enable the processing of projections.
        /// </summary>
        [PropertyOrder(11)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_5")]
        public bool EnableProcessingOfProjections
        {
            get { return enableProcessingOfProjections; }
            set
            {
                if (enableProcessingOfProjections != value)
                {
                    enableProcessingOfProjections = value;
                    OnChanged(11);
                }
            }
        }

        /// <summary>
        /// The sub directory name definition for include in the target directory.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_1")]
        public string SubDirectoryName
        {
            get { return subDirectoryName; }
            set
            {
                if (subDirectoryName != value)
                {
                    subDirectoryName = value;
                    char[] pattern   = Path.GetInvalidPathChars();
                    string error     = string.Format(error_messages[0], pattern.ToString(""));

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
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_2")]
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
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_3")]
        public string TemplateFileName
        {
            get { return templateFileName; }
            set
            {
                if (templateFileName != value)
                {
                    templateFileName = value;
                    string path      = value;
                    char[] pattern   = Path.GetInvalidFileNameChars();
                    string error     = string.Format(error_messages[2], pattern.ToString(""));

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
        #endregion

        #region internal methods
        /// <summary>
        /// Cloneable values this object on loaded.
        /// </summary>
        internal virtual void OnLoaded()
        {
            s_values[0] = outputExtension;

            pg_names = (string[])pageNames.Clone();
            pj_names = (string[])projectionNames.Clone();

            b_values[0] = excludePage;
            m_values[0] = pageScale;
            b_values[1] = savePageScale;
            b_values[2] = checkDrawingTemplate;
            b_values[3] = excludeProjection;
            m_values[1] = projectionScale;
            b_values[4] = saveProjectionScale;
            b_values[5] = enableProcessingOfProjections;
            s_values[1] = subDirectoryName;
            s_values[2] = fileNameSuffix;
            s_values[3] = templateFileName;

            pg_types[0] = pageTypes.Normal;
            pg_types[1] = pageTypes.Workplane;
            pg_types[2] = pageTypes.Auxiliary;
            pg_types[3] = pageTypes.Text;
            pg_types[4] = pageTypes.BillOfMaterials;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            isLoaded = true;

            //Debug.WriteLine("OnLoaded");
        }

        /// <summary>
        /// Verified this object a change and calls event for 'IsChanged' property.
        /// </summary>
        /// <param name="index">Property index.</param>
        internal virtual void OnChanged(int index)
        {
            if (!isLoaded) return;

            switch (index)
            {
                case 0:
                    if (s_values[0] != outputExtension)
                        objState[0] = 1;
                    else
                        objState[0] = 0;
                    break;
                case 1:
                    if (!Enumerable.SequenceEqual(pg_names, pageNames))
                        objState[1] = 1;
                    else
                        objState[1] = 0;
                    break;
                case 2:
                    if (b_values[0] != excludePage)
                        objState[2] = 1;
                    else
                        objState[2] = 0;
                    break;
                case 3:
                    if (m_values[0] != pageScale)
                        objState[3] = 1;
                    else
                        objState[3] = 0;
                    break;
                case 4:
                    if (b_values[1] != savePageScale)
                        objState[4] = 1;
                    else
                        objState[4] = 0;
                    break;
                case 5:
                    if (pg_types[0] != pageTypes.Normal ||
                        pg_types[1] != pageTypes.Workplane ||
                        pg_types[2] != pageTypes.Auxiliary ||
                        pg_types[3] != pageTypes.Text ||
                        pg_types[4] != pageTypes.BillOfMaterials)
                        objState[5] = 1;
                    else
                        objState[5] = 0;
                    break;
                case 6:
                    if (b_values[2] != checkDrawingTemplate)
                        objState[6] = 1;
                    else
                        objState[6] = 0;
                    break;
                case 7:
                    if (!Enumerable.SequenceEqual(pj_names, projectionNames))
                        objState[7] = 1;
                    else
                        objState[7] = 0;
                    break;
                case 8:
                    if (b_values[3] != excludeProjection)
                        objState[8] = 1;
                    else
                        objState[8] = 0;
                    break;
                case 9:
                    if (m_values[1] != projectionScale)
                        objState[9] = 1;
                    else
                        objState[9] = 0;
                    break;
                case 10:
                    if (b_values[4] != saveProjectionScale)
                        objState[10] = 1;
                    else
                        objState[10] = 0;
                    break;
                case 11:
                    if (b_values[5] != enableProcessingOfProjections)
                        objState[11] = 1;
                    else
                        objState[11] = 0;
                    break;
                case 12:
                    if (s_values[1] != subDirectoryName)
                        objState[12] = 1;
                    else
                        objState[12] = 0;
                    break;
                case 13:
                    if (s_values[2] != fileNameSuffix)
                        objState[13] = 1;
                    else
                        objState[13] = 0;
                    break;
                case 14:
                    if (s_values[3] != templateFileName)
                        objState[14] = 1;
                    else
                        objState[14] = 0;
                    break;
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

            OnPropertyChanged("IsChanged");
        }

        /// <summary>
        /// Configuration task method.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write
        /// </param>
        internal void ConfigurationTask(XElement element, int flag)
        {
            isLoaded = false;

            foreach (var i in element.Elements())
            {
                TranslatorTask(i, flag);
            }

            OnLoaded();
        }

        /// <summary>
        /// Create new translator metadata to Xml.
        /// </summary>
        /// <param name="translator">Translator type.</param>
        /// <returns></returns>
        internal XElement NewTranslator(TranslatorType translator)
        {
            XElement element = new XElement("translator", new XAttribute("id", translator),
                new XElement("parameter",
                    new XAttribute("name", "PageNames"),
                    new XAttribute("value", pageNames.ToString("\r\n"))),
                new XElement("parameter",
                    new XAttribute("name", "ExcludePage"),
                    new XAttribute("value", excludePage ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "PageScale"),
                    new XAttribute("value", pageScale)),
                new XElement("parameter",
                    new XAttribute("name", "SavePageScale"),
                    new XAttribute("value", savePageScale ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "PageTypes"),
                    new XAttribute("value", pageTypes.ToString())),
                new XElement("parameter",
                    new XAttribute("name", "CheckDrawingTemplate"),
                    new XAttribute("value", checkDrawingTemplate ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ProjectionNames"),
                    new XAttribute("value", projectionNames.ToString("\r\n"))),
                new XElement("parameter",
                    new XAttribute("name", "ExcludeProjection"),
                    new XAttribute("value", excludeProjection ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ProjectionScale"),
                    new XAttribute("value", projectionScale)),
                new XElement("parameter",
                    new XAttribute("name", "SaveProjectionScale"),
                    new XAttribute("value", saveProjectionScale ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "EnableProcessingOfProjections"),
                    new XAttribute("value", enableProcessingOfProjections ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "SubDirectoryName"),
                    new XAttribute("value", subDirectoryName)),
                new XElement("parameter",
                    new XAttribute("name", "FileNameSuffix"),
                    new XAttribute("value", fileNameSuffix)),
                new XElement("parameter",
                    new XAttribute("name", "TemplateFileName"),
                    new XAttribute("value", templateFileName)));

            return element;
        }

        /// <summary>
        /// Append translator metadata to parent element.
        /// </summary>
        /// <param name="parent">Parent element from metadata Xml.</param>
        /// <param name="translator">Translator type.</param>
        internal virtual void AppendTranslatorToXml(XElement parent, TranslatorType translator)
        {
            parent.Add(NewTranslator(translator));
        }

        /// <summary>
        /// Virtual method for processing translator parameters.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write
        /// </param>
        internal virtual void TranslatorTask(XElement element, int flag)
        {
            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "PageNames":
                    if (flag == 0)
                    {
                        pageNames = value.Length > 0 
                            ? value.Replace("\r", "").Split('\n') 
                            : new string[] { };
                    }
                    else
                        value = pageNames.ToString("\r\n");
                    break;
                case "ExcludePage":
                    if (flag == 0)
                        excludePage = value == "1" ? true : false;
                    else
                        value = excludePage ? "1" : "0";
                    break;
                case "PageScale":
                    if (flag == 0)
                        pageScale = decimal.Parse(value, 
                            NumberStyles.Float, CultureInfo.InvariantCulture);
                    else
                        value = pageScale.ToString(CultureInfo.InvariantCulture);
                    break;
                case "SavePageScale":
                    if (flag == 0)
                        savePageScale = value == "1" ? true : false;
                    else
                        value = savePageScale ? "1" : "0";
                    break;
                case "PageTypes":
                    if (flag == 0)
                    {
                        string[] values = value.Split(' ');

                        pageTypes.Normal          = values[0] == "01" ? true : false;
                        pageTypes.Workplane       = values[1] == "01" ? true : false;
                        pageTypes.Auxiliary       = values[2] == "01" ? true : false;
                        pageTypes.Text            = values[3] == "01" ? true : false;
                        pageTypes.BillOfMaterials = values[4] == "01" ? true : false;
                    }
                    else
                        value = pageTypes.ToString();
                    break;
                case "CheckDrawingTemplate":
                    if (flag == 0)
                        checkDrawingTemplate = value == "1" ? true : false;
                    else
                        value = checkDrawingTemplate ? "1" : "0";
                    break;
                case "ProjectionNames":
                    if (flag == 0)
                    {
                        projectionNames = value.Length > 0 
                            ? value.Replace("\r", "").Split('\n') 
                            : new string[] { };
                    }
                    else
                        value = projectionNames.ToString("\r\n");
                    break;
                case "ExcludeProjection":
                    if (flag == 0)
                        excludeProjection = value == "1" ? true : false;
                    else
                        value = excludeProjection ? "1" : "0";
                    break;
                case "ProjectionScale":
                    if (flag == 0)
                        projectionScale = decimal.Parse(value, 
                            NumberStyles.Float, CultureInfo.InvariantCulture);
                    else
                        value = projectionScale.ToString(CultureInfo.InvariantCulture);
                    break;
                case "SaveProjectionScale":
                    if (flag == 0)
                        saveProjectionScale = value == "1" ? true : false;
                    else
                        value = saveProjectionScale ? "1" : "0";
                    break;
                case "EnableProcessingOfProjections":
                    if (flag == 0)
                        enableProcessingOfProjections = value == "1" ? true : false;
                    else
                        value = enableProcessingOfProjections ? "1" : "0";
                    break;
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

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pages"></param>
        /// <param name="logFile"></param>
        internal virtual void Export(Document document, Dictionary<Page, string> pages, LogFile logFile) { }
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

        #region INotifyPropertyChanged members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OpPropertyChanged event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

    /// <summary>
    /// The page types class.
    /// </summary>
    public class PageTypes : INotifyPropertyChanged
    {
        #region private fields
        private bool normal;
        private bool workplane;
        private bool auxiliary;
        private bool text;
        private bool billOfMaterials;
        #endregion

        #region properties
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5_1")]
        public bool Normal
        {
            get { return normal; }
            set
            {
                if (normal != value)
                {
                    normal = value;
                    OnPropertyChanged("Normal");
                }
            }
        }

        [PropertyOrder(2)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5_2")]
        public bool Workplane
        {
            get { return workplane; }
            set
            {
                if (workplane != value)
                {
                    workplane = value;
                    OnPropertyChanged("Workplane");
                }
            }
        }

        [PropertyOrder(3)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5_3")]
        public bool Auxiliary
        {
            get { return auxiliary; }
            set
            {
                if (auxiliary != value)
                {
                    auxiliary = value;
                    OnPropertyChanged("Auxiliary");
                }
            }
        }

        [PropertyOrder(4)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5_4")]
        public bool Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        [PropertyOrder(5)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5_5")]
        public bool BillOfMaterials
        {
            get { return billOfMaterials; }
            set
            {
                if (billOfMaterials != value)
                {
                    billOfMaterials = value;
                    OnPropertyChanged("BillOfMaterials");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string[] values = new string[5];

            values[0] = normal          ? "01" : "00";
            values[1] = workplane       ? "01" : "00";
            values[2] = auxiliary       ? "01" : "00";
            values[3] = text            ? "01" : "00";
            values[4] = billOfMaterials ? "01" : "00";

            return values.ToString(" ");
        }
        #endregion

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}