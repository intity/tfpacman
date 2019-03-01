﻿using System;
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
using TFlex.Model.Model3D;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Package configuration base class.
    /// </summary>
    [CustomCategoryOrder(Resource.PACKAGE_0, 1)]
    [CustomCategoryOrder(Resource.PACKAGE_0, 2)]
    [CustomCategoryOrder(Resource.PACKAGE_0, 3)]
    public class Package_0 : INotifyPropertyChanged
    {
        #region private fields
        private Header header;
        private int countProcessingFiles;

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
        private bool useDocumentNamed;

        private string[] pg_names, pj_names;
        private readonly byte[] objState    = new byte[16];
        private readonly bool[] pg_types    = new bool[5];
        private readonly bool[] b_values    = new bool[7];
        private readonly string[] s_values  = new string[4];
        private readonly decimal[] m_values = new decimal[2]; 
        private bool isLoaded, isChanged;
        #endregion

        public Package_0(Header header)
        {
            this.header      = header;
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
            useDocumentNamed = true;
        }

        private void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChanged(5);
        }

        #region internal properties
        /// <summary>
        /// The package is changed.
        /// </summary>
        internal virtual bool IsChanged { get { return (isChanged); } }

        /// <summary>
        /// The package is loaded.
        /// </summary>
        internal bool IsLoaded  { get { return (isLoaded); } }
        #endregion

        #region properties
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_1")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_1")]
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_2")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_2")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_3")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_3")]
        [Editor(typeof(InputScaleControl), typeof(UITypeEditor))]
        [DefaultValue(typeof(decimal), "99999")]
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_4")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_4")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5")]
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
        [CustomCategory(Resource.PACKAGE_0, "category1")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_6")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_6")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category2")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn2_1")]
        [CustomDescription(Resource.PACKAGE_0, "dn2_1")]
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
        [CustomCategory(Resource.PACKAGE_0, "category2")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn2_2")]
        [CustomDescription(Resource.PACKAGE_0, "dn2_2")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category2")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn2_3")]
        [CustomDescription(Resource.PACKAGE_0, "dn2_3")]
        [Editor(typeof(InputScaleControl), typeof(UITypeEditor))]
        [DefaultValue(typeof(decimal), "99999")]
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
        [CustomCategory(Resource.PACKAGE_0, "category2")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn2_4")]
        [CustomDescription(Resource.PACKAGE_0, "dn2_4")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category2")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn2_5")]
        [CustomDescription(Resource.PACKAGE_0, "dn2_5")]
        [DefaultValue(false)]
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
        [CustomCategory(Resource.PACKAGE_0, "category3")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn3_1")]
        [CustomDescription(Resource.PACKAGE_0, "dn3_1")]
        public string SubDirectoryName
        {
            get { return subDirectoryName; }
            set
            {
                if (subDirectoryName != value)
                {
                    subDirectoryName = value;
                    OnChanged(12);
                }
            }
        }

        /// <summary>
        /// The file name suffix.
        /// </summary>
        [PropertyOrder(13)]
        [CustomCategory(Resource.PACKAGE_0, "category3")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn3_2")]
        [CustomDescription(Resource.PACKAGE_0, "dn3_2")]
        public string FileNameSuffix
        {
            get { return fileNameSuffix; }
            set
            {
                if (fileNameSuffix != value)
                {
                    fileNameSuffix = value;
                    OnChanged(13);
                }
            }
        }

        /// <summary>
        /// Template name of the file definition.
        /// </summary>
        [PropertyOrder(14)]
        [CustomCategory(Resource.PACKAGE_0, "category3")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn3_3")]
        [CustomDescription(Resource.PACKAGE_0, "dn3_3")]
        public string TemplateFileName
        {
            get { return templateFileName; }
            set
            {
                if (templateFileName != value)
                {
                    templateFileName = value;
                    OnChanged(14);
                }
            }
        }

        /// <summary>
        /// Used the document name for output file definition.
        /// </summary>
        [PropertyOrder(15)]
        [CustomCategory(Resource.PACKAGE_0, "category3")]
        [CustomDisplayName(Resource.PACKAGE_0, "dn3_4")]
        [CustomDescription(Resource.PACKAGE_0, "dn3_4")]
        [DefaultValue(true)]
        public bool UseDocumentNamed
        {
            get { return useDocumentNamed; }
            set
            {
                if (useDocumentNamed != value)
                {
                    useDocumentNamed = value;
                    OnChanged(15);
                }
            }
        }
        #endregion

        #region methods
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
            b_values[6] = useDocumentNamed;

            pg_types[0] = pageTypes.Normal;
            pg_types[1] = pageTypes.Workplane;
            pg_types[2] = pageTypes.Auxiliary;
            pg_types[3] = pageTypes.Text;
            pg_types[4] = pageTypes.BillOfMaterials;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
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
                case 15:
                    if (b_values[6] != useDocumentNamed)
                        objState[15] = 1;
                    else
                        objState[15] = 0;
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
        /// <param name="flag"></param>
        internal void ConfigurationTask(XElement element, int flag)
        {
            if (flag != 2)
            {
                foreach (var i in element.Elements())
                {
                    PackageTask(i, flag);
                }

                isLoaded = true;
                OnLoaded();

                isChanged = false;
            }
            else
            {
                element.Remove();
                isLoaded = false;
            }
        }

        /// <summary>
        /// Create new package metadata to Xml.
        /// </summary>
        /// <param name="package">Package type.</param>
        /// <returns></returns>
        internal XElement NewPackage(PackageType package)
        {
            XElement element = new XElement("package", new XAttribute("id", package),
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
                    new XAttribute("value", templateFileName)),
                new XElement("parameter",
                    new XAttribute("name", "UseDocumentNamed"),
                    new XAttribute("value", useDocumentNamed ? "1" : "0")));

            return element;
        }

        /// <summary>
        /// Append package metadata to parent element.
        /// </summary>
        /// <param name="parent">Parent element from metadata Xml.</param>
        /// <param name="package">Package type.</param>
        internal virtual void AppendPackageToXml(XElement parent, PackageType package)
        {
            parent.Add(NewPackage(package));
        }

        /// <summary>
        /// Virtual method for processing package parameters.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write, (2) - delete
        /// </param>
        internal virtual void PackageTask(XElement element, int flag)
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
                case "UseDocumentNamed":
                    if (flag == 0)
                        useDocumentNamed = value == "1" ? true : false;
                    else
                        value = useDocumentNamed ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="path">Full path file name.</param>
        internal virtual void Export(Document document, Page page, string path) { }

        /// <summary>
        /// Processing file.
        /// </summary>
        /// <param name="path"></param>
        internal void ProcessingFile(string path)
        {
            Document document = Application.OpenDocument(path, false);
            FileInfo fileInfo = new FileInfo(path);
            string directory = fileInfo.Directory.FullName.Replace(
                header.InitialCatalog,
                header.TargetDirectory + "\\" + (subDirectoryName.Length > 0 ? subDirectoryName : outputExtension));
            string targetDirectory = Directory.Exists(directory)
                ? directory
                : Directory.CreateDirectory(directory).FullName;

            document.BeginChanges(string.Format("Processing file: {0}", fileInfo.Name));
            ProcessingPages(document, targetDirectory);

            if (document.Changed)
            {
                document.EndChanges();
                document.Save();
            }
            else
                document.CancelChanges();

            document.Close();

            if (Directory.GetFiles(targetDirectory).Length == 0 &&
                Directory.GetDirectories(targetDirectory).Length == 0)
                Directory.Delete(targetDirectory, false);
        }
        #endregion

        #region private methods
        private string[] Groups(string expression)
        {
            string pattern = @"\((.*?)\)";
            string[] groups = expression.Split(new string[] { ".type", ".format" }, StringSplitOptions.None);

            if (groups.Length > 1)
            {
                groups[1] = Regex.Match(expression, pattern).Groups[1].Value;
            }

            return groups;
        }

        private string GetValue(Document document, Page page, string expression)
        {
            string result = null;
            string[] groups = Groups(expression);
            string[] argv;
            Variable variable;

            if (groups.Length > 1)
            {
                if (expression.Contains("page.type"))
                {
                    if ((argv = groups[1].Split(',')).Length < 2) return result;

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

        private string ExpressionParse(Document document, Page page, string expression)
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

            return result = result ?? string.Empty;
        }

        private string GetOutputFileName(Document document, Page page)
        {
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (templateFileName.Length > 0 && !useDocumentNamed)
                fileName = templateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (fileNameSuffix.Length > 0)
                    fileName += ExpressionParse(document, page, fileNameSuffix);
                return fileName;
            }

            foreach (Match i in Regex.Matches(fileName, pattern))
            {
                if ((expVal = ExpressionParse(document, page, i.Groups[1].Value)) == null)
                    continue;

                fileName = fileName.Replace(i.Groups[0].Value, expVal);
            }

            return fileName;
        }

        private bool IsValidDrawingTemplate(Document document, Page page)
        {
            if (document.GetFragments().Where(
                f => f.GroupType == ObjectType.Fragment && f.Page == page &&
                f.DisplayName.Contains("<Форматки>")).FirstOrDefault() != null)
                return true;

            return false;
        }

        private bool CheckPageType(Page page)
        {
            bool result = false;
            uint[,] pt = new uint[,]
            {
                {
                    (uint)(pageTypes.Normal ? 1 : 0),
                    (uint)PageType.Normal
                },
                {
                    (uint)(pageTypes.Workplane ? 1 : 0),
                    (uint)PageType.Workplane
                },
                {
                    (uint)(pageTypes.Auxiliary ? 1 : 0),
                    (uint)PageType.Auxiliary
                },
                {
                    (uint)(pageTypes.Text ? 1 : 0),
                    (uint)PageType.Text
                },
                {
                    (uint)(pageTypes.BillOfMaterials ? 1 : 0),
                    (uint)PageType.BillOfMaterials
                }
            };

            for (int i = 0; i < pt.GetLength(0); i++)
            {
                if (pt[i, 0] > 0 && pt[i, 1] == (uint)page.PageType)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private List<Page> GetPagesOnType(Document document)
        {
            List<Page> pages = new List<Page>();

            foreach (var i in document.GetPages())
            {
                if (CheckPageType(i) && (pageNames.Count() > 0 ? pageNames.Contains(i.Name) : true))
                    pages.Add(i);
            }
            return pages;
        }

        private void ProcessingPages(Document document, string targetDirectory)
        {
            RegenerateOptions options;
            int n1, n2, n3, count = 0;
            uint num;
            string path;
            IEnumerable<Page> pages = GetPagesOnType(document);

            foreach (var i in pages)
            {
                n1 = pageNames.Contains(i.Name) ? 1 : 0;
                n2 = excludePage ? 1 : 0;
                n3 = CheckPageType(i) ? 1 : 0;
                num = Convert.ToUInt32(Convert.ToUInt32(
                    n1.ToString() +
                    n2.ToString() +
                    n3.ToString(), 2).ToString("X"), 16);

                if (!(num == 1 || num == 5)) continue;

                if (checkDrawingTemplate && !IsValidDrawingTemplate(document, i)) return;

                if (i.Scale.Value != (double)pageScale && pageScale != 99999)
                {
                    i.Scale = new Parameter((double)pageScale);

                    if (savePageScale)
                    {
                        options = new RegenerateOptions
                        {
                            Full = true
                        };
                        document.Regenerate(options);
                    }
                }

                if (enableProcessingOfProjections) ProcessingProjections(document, i.Name);

                path = targetDirectory + "\\" + GetOutputFileName(document, i);

                if (pages.Where(p => p.PageType == i.PageType).Count() > 1)
                {
                    path += "_" + (count + 1).ToString() + "." + outputExtension.ToLower();
                    count++;
                }
                else
                    path += "." + outputExtension.ToLower();

                Export(document, i, path);
                countProcessingFiles++;
            }
        }

        private void ProcessingProjections(Document document, string pageName)
        {
            RegenerateOptions options;
            int n1, n2, n3;
            uint num;
            double scale;

            foreach (var i in document.GetProjections().Where(p => p.Page.Name == pageName))
            {
                n1 = projectionNames.Contains(i.Name) ? 1 : 0;
                n2 = excludeProjection ? 1 : 0;
                n3 = enableProcessingOfProjections ? 1 : 0;
                num = Convert.ToUInt32(Convert.ToUInt32(
                    n1.ToString() +
                    n2.ToString() +
                    n3.ToString(), 2).ToString("X"), 16);

                if (!(num == 1 || num == 5)) continue;

                if (i.Scale.Value != (double)projectionScale)
                {
                    scale = projectionScale == 99999 ? Parameter.Default().Value : (double)projectionScale;
                    i.Scale = new Parameter(scale);

                    if (saveProjectionScale)
                    {
                        options = new RegenerateOptions
                        {
                            Projections = true
                        };
                        document.Regenerate(options);
                    }
                }
            }
        }
        #endregion

        #region events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5_1")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5_1")]
        [DefaultValue(true)]
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
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5_2")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5_2")]
        [DefaultValue(false)]
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
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5_3")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5_3")]
        [DefaultValue(false)]
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
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5_4")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5_4")]
        [DefaultValue(false)]
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
        [CustomDisplayName(Resource.PACKAGE_0, "dn1_5_5")]
        [CustomDescription(Resource.PACKAGE_0, "dn1_5_5")]
        [DefaultValue(false)]
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

        #region events
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}