using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Export;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Configuration Header class.
    /// </summary>
    public class Header
    {
        #region private fields
        private Package package_0;
        private ExportToPackage1 package_1;
        private ExportToPackage3 package_3;
        private ExportToPackage9 package_9;
        private ObservableDictionary<string, object> translators;
        private readonly List<string> loadedTranslators;
        private string configurationName;
        private string initialCatalog;
        private string targetDirectory;
        private string inputExtension;
        private TranslatorTypes translatorTypes;

        private readonly byte[] objState = new byte[6];
        private readonly string[] s_values = new string[4];
        private readonly bool[] tr_types = new bool[12];
        private bool isCreated, isChanged, isLoaded;
        #endregion

        public Header()
        {
            configurationName = string.Empty;
            initialCatalog    = string.Empty;
            targetDirectory   = string.Empty;
            inputExtension    = "*.grb";
            translators       = new ObservableDictionary<string, object>();
            translatorTypes   = new TranslatorTypes();
            translatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;
            translatorTypes.Default = true;

            loadedTranslators = new List<string>();
            isCreated = true;
        }

        private void Package_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender.Equals(package_0) ? package_0.IsChanged :
                sender.Equals(package_1) ? package_1.IsChanged :
                sender.Equals(package_3) ? package_3.IsChanged : package_9.IsChanged)
                objState[5] = 1;
            else
                objState[5] = 0;

            OnChanged(5);
        }

        private void TranslatorTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine(string.Format("Translator: {0}", e.PropertyName));

            switch (e.PropertyName)
            {
                case "Default":
                    if ((sender as TranslatorTypes).Default)
                    {
                        package_0 = new Package();
                        package_0.PropertyChanged += Package_PropertyChanged;
                        translators.Add(e.PropertyName, package_0);
                    }
                    else
                    {
                        translators.Remove(e.PropertyName);
                    }
                    break;
                case "Acad":
                    if ((sender as TranslatorTypes).Acad)
                    {
                        package_1 = new ExportToPackage1(this);
                        package_1.PropertyChanged += Package_PropertyChanged;
                        translators.Add(e.PropertyName, package_1);
                    }
                    else
                    {
                        translators.Remove(e.PropertyName);
                    }   
                    break;
                case "Bitmap":
                    if ((sender as TranslatorTypes).Bitmap)
                    {
                        package_3 = new ExportToPackage3(this);
                        package_3.PropertyChanged += Package_PropertyChanged;
                        translators.Add(e.PropertyName, package_3);
                    }
                    else
                    {
                        translators.Remove(e.PropertyName);
                    }
                    break;
                case "Pdf":
                    if ((sender as TranslatorTypes).Pdf)
                    {
                        package_9 = new ExportToPackage9(this);
                        package_9.PropertyChanged += Package_PropertyChanged;
                        translators.Add(e.PropertyName, package_9);
                    }
                    else
                    {
                        translators.Remove(e.PropertyName);
                    }
                    break;
            }

            OnChanged(4);
        }

        #region internal properties
        /// <summary>
        /// Translator list.
        /// </summary>
        internal ObservableDictionary<string, object> Translators
        {
            get { return (translators); }
        }

        /// <summary>
        /// The configuration is created.
        /// </summary>
        internal bool IsCreated { get { return (isCreated); } }

        /// <summary>
        /// The configuration is changed.
        /// </summary>
        internal bool IsChanged { get { return (isChanged); } }

        /// <summary>
        /// The configuration is loaded.
        /// </summary>
        internal bool IsLoaded { get { return (isLoaded); } }

        /// <summary>
        /// Old configuration name.
        /// </summary>
        internal string OldName { get; set; }
        #endregion

        #region properties
        /// <summary>
        /// The configuration name definition.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_1")]
        [CustomDescription(Resource.HEADER_UI, "dn1_1")]
        public string ConfigurationName
        {
            get { return configurationName; }
            set
            {
                if (configurationName != value)
                {
                    configurationName = value;
                    OnChanged(0);
                }
            }
        }

        /// <summary>
        /// The initial catalog definition.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_2")]
        [CustomDescription(Resource.HEADER_UI, "dn1_2")]
        [Editor(typeof(InputPathControl), typeof(UITypeEditor))]
        public string InitialCatalog
        {
            get { return initialCatalog; }
            set
            {
                if (initialCatalog != value)
                {
                    initialCatalog = value;
                    OnChanged(1);
                }
            }
        }

        /// <summary>
        /// The target directory definition.
        /// </summary>
        [PropertyOrder(3)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_3")]
        [CustomDescription(Resource.HEADER_UI, "dn1_3")]
        [Editor(typeof(InputPathControl), typeof(UITypeEditor))]
        public string TargetDirectory
        {
            get { return targetDirectory; }
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;
                    OnChanged(2);
                }
            }
        }

        /// <summary>
        /// The input file extension.
        /// </summary>
        [PropertyOrder(4)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_4")]
        [CustomDescription(Resource.HEADER_UI, "dn1_4")]
        [ItemsSource(typeof(InputExtensionItems))]
        [DefaultValue("*.grb")]
        public string InputExtension
        {
            get { return inputExtension; }
            set
            {
                if (inputExtension != value)
                {
                    inputExtension = value;
                    OnChanged(3);
                }
            }
        }

        [PropertyOrder(5)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5")]
        [ExpandableObject]
        [Editor(typeof(TranslatorTypesEditor), typeof(UITypeEditor))]
        public TranslatorTypes TranslatorTypes
        {
            get { return (translatorTypes); }
        }
        #endregion

        #region methods
        private void OnLoaded()
        {
            s_values[00] = configurationName;
            s_values[01] = initialCatalog;
            s_values[02] = targetDirectory;
            s_values[03] = inputExtension;

            tr_types[00] = translatorTypes.Default;
            tr_types[01] = translatorTypes.Acad;
            tr_types[02] = translatorTypes.Acis;
            tr_types[03] = translatorTypes.Bitmap;
            tr_types[04] = translatorTypes.Bmf;
            tr_types[05] = translatorTypes.Emf;
            tr_types[06] = translatorTypes.Iges;
            tr_types[07] = translatorTypes.Jt;
            tr_types[08] = translatorTypes.Parasolid;
            tr_types[09] = translatorTypes.Pdf;
            tr_types[10] = translatorTypes.Step;
            tr_types[11] = translatorTypes.Stl;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
        }

        private void OnChanged(int index)
        {
            if (!isLoaded) return;

            switch (index)
            {
                case 0: objState[0] = (byte)(s_values[0] != configurationName ? 1 : 0); break;
                case 1: objState[1] = (byte)(s_values[1] != initialCatalog    ? 1 : 0); break;
                case 2: objState[2] = (byte)(s_values[2] != targetDirectory   ? 1 : 0); break;
                case 3: objState[3] = (byte)(s_values[3] != inputExtension    ? 1 : 0); break;
                case 4:
                    if (tr_types[00] != translatorTypes.Default || 
                        tr_types[01] != translatorTypes.Acad || 
                        tr_types[02] != translatorTypes.Acis || 
                        tr_types[03] != translatorTypes.Bitmap || 
                        tr_types[04] != translatorTypes.Bmf || 
                        tr_types[05] != translatorTypes.Emf || 
                        tr_types[06] != translatorTypes.Iges || 
                        tr_types[07] != translatorTypes.Jt || 
                        tr_types[08] != translatorTypes.Parasolid || 
                        tr_types[09] != translatorTypes.Pdf || 
                        tr_types[10] != translatorTypes.Step || 
                        tr_types[11] != translatorTypes.Stl)
                        objState[4] = 1;
                    else
                        objState[4] = 0;
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

            //Debug.WriteLine(string.Format("OnChanged: [{0}, {1}]", index, isChanged));
        }

        /// <summary>
        /// Extension method to get packages.
        /// </summary>
        /// <param name="element"></param>
        internal void GetPackages(XElement element)
        {
            foreach (XElement i in element.Element("packages").Elements())
            {
                switch (i.Attribute("id").Value)
                {
                    case "Default":
                        translatorTypes.Default = true;
                        package_0.ConfigurationTask(i, 0);
                        break;
                    case "Acad":
                        translatorTypes.Acad = true;
                        package_1.ConfigurationTask(i, 0);
                        break;
                    case "Bitmap":
                        translatorTypes.Bitmap = true;
                        package_3.ConfigurationTask(i, 0);
                        break;
                    case "Pdf":
                        translatorTypes.Pdf = true;
                        package_9.ConfigurationTask(i, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Extension method to set packages.
        /// </summary>
        /// <param name="element"></param>
        internal void SetPackages(XElement element)
        {
            XElement parent = element.Element("packages");

            foreach (XElement i in parent.Elements())
            {
                string value = i.Attribute("id").Value;
                switch (value)
                {
                    case "Default":
                        if (translatorTypes.Default)
                        {
                            package_0.ConfigurationTask(i, 1);
                        }
                        else if (package_0 != null)
                        {
                            package_0.ConfigurationTask(i, 2);
                        }
                        break;
                    case "Acad":
                        
                        if (translatorTypes.Acad)
                        {
                            package_1.ConfigurationTask(i, 1);
                        }
                        else if (package_1 != null)
                        {
                            package_1.ConfigurationTask(i, 2);
                        }
                        break;
                    case "Bitmap":
                        if (translatorTypes.Bitmap)
                        {
                            package_3.ConfigurationTask(i, 1);
                        }
                        else if (package_3 != null)
                        {
                            package_3.ConfigurationTask(i, 2);
                        }
                        break;
                    case "Pdf":
                        if (translatorTypes.Pdf)
                        {
                            package_9.ConfigurationTask(i, 1);
                        }
                        else if (package_9 != null)
                        {
                            package_9.ConfigurationTask(i, 2);
                        }
                        break;
                }
            }

            foreach (var i in translators)
            {
                switch (i.Key)
                {
                    case "Default":
                        if (package_0.IsLoaded == false)
                        {
                            package_0.AppendPackageToXml(parent, PackageType.Default);
                        }
                        break;
                    case "Acad":
                        if (package_1.IsLoaded == false)
                        {
                            package_1.AppendPackageToXml(parent, PackageType.Acad);
                        }
                        break;
                    case "Bitmap":
                        if (package_3.IsLoaded == false)
                        {
                            package_3.AppendPackageToXml(parent, PackageType.Bitmap);
                        }
                        break;
                    case "Pdf":
                        if (package_9.IsLoaded == false)
                        {
                            package_9.AppendPackageToXml(parent, PackageType.Pdf);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Create new configuration to Xml.
        /// </summary>
        /// <returns></returns>
        internal XDocument NewConfiguration()
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("configuration", 
                    new XElement("header",
                    new XElement("parameter",
                            new XAttribute("name", "ConfigurationName"),
                            new XAttribute("value", configurationName)),
                        new XElement("parameter",
                            new XAttribute("name", "InitialCatalog"),
                            new XAttribute("value", initialCatalog)),
                        new XElement("parameter",
                            new XAttribute("name", "TargetDirectory"),
                            new XAttribute("value", targetDirectory)),
                        new XElement("parameter",
                            new XAttribute("name", "InputExtension"),
                            new XAttribute("value", inputExtension)),
                        new XElement("parameter",
                            new XAttribute("name", "TranslatorTypes"),
                            new XAttribute("value", translatorTypes.ToString()))),
                    new XElement("packages", package_0.NewPackage(PackageType.Default))));

            return document;
        }

        /// <summary>
        /// The configuration task method.
        /// </summary>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write, (2) - delete
        /// </param>
        internal void ConfigurationTask(int flag)
        {
            string path = Resource.UserDirectory + "\\" + configurationName + ".config";

            if (flag != 2)
            {
                XDocument document = null;

                if (File.Exists(path))
                    document = XDocument.Load(path);
                else
                    document = NewConfiguration();

                XElement element = document.Element("configuration");

                foreach (var i in element.Element("header").Elements())
                {
                    HeaderTask(i, flag);
                }

                if (flag == 0)
                {
                    GetPackages(element);
                }
                else
                {
                    SetPackages(element);
                    element.Save(path);
                }

                isLoaded = true;
                OnLoaded();

                isCreated = false;
                isChanged = false;
            }
            else
            {
                File.Delete(path);
                isLoaded = false;
            }
        }

        /// <summary>
        /// Extension method for processing header parameters.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag"></param>
        private void HeaderTask(XElement element, int flag)
        {
            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "ConfigurationName":
                    if (flag == 0)
                        configurationName = value;
                    else
                        value = configurationName;
                    break;
                case "InitialCatalog":
                    if (flag == 0)
                        initialCatalog = value;
                    else
                        value = initialCatalog;
                    break;
                case "TargetDirectory":
                    if (flag == 0)
                        targetDirectory = value;
                    else
                        value = targetDirectory;
                    break;
                case "InputExtension":
                    if (flag == 0)
                        inputExtension = value;
                    else
                        value = inputExtension;
                    break;
                case "TranslatorTypes":
                    if (flag == 0)
                    {
                        string[] values = value.Split(' ');

                        translatorTypes.Default   = values[00] == "01" ? true : false;
                        translatorTypes.Acad      = values[01] == "01" ? true : false;
                        translatorTypes.Acis      = values[02] == "01" ? true : false;
                        translatorTypes.Bitmap    = values[03] == "01" ? true : false;
                        translatorTypes.Bmf       = values[04] == "01" ? true : false;
                        translatorTypes.Emf       = values[05] == "01" ? true : false;
                        translatorTypes.Iges      = values[06] == "01" ? true : false;
                        translatorTypes.Jt        = values[07] == "01" ? true : false;
                        translatorTypes.Parasolid = values[08] == "01" ? true : false;
                        translatorTypes.Pdf       = values[09] == "01" ? true : false;
                        translatorTypes.Step      = values[10] == "01" ? true : false;
                        translatorTypes.Stl       = values[11] == "01" ? true : false;
                    }
                    else
                        value = translatorTypes.ToString();
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

    /// <summary>
    /// The TranslatorTypes class.
    /// </summary>
    public class TranslatorTypes : INotifyPropertyChanged
    {
        #region private fields
        private bool document;
        private bool acad;
        private bool acis;
        private bool bitmap;
        private bool bmf;
        private bool emf;
        private bool iges;
        private bool jt;
        private bool parasolid;
        private bool pdf;
        private bool step;
        private bool stl;
        #endregion

        #region properties
        [PropertyOrder(0)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_0")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_0")]
        [DefaultValue(false)]
        public bool Default
        {
            get { return document; }
            set
            {
                if (document != value)
                {
                    document = value;
                    OnPropertyChanged("Default");
                }
            }
        }

        [PropertyOrder(1)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_1")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_1")]
        [DefaultValue(false)]
        public bool Acad
        {
            get { return acad; }
            set
            {
                if (acad != value)
                {
                    acad = value;
                    OnPropertyChanged("Acad");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(2)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_2")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_2")]
        [DefaultValue(false)]
        public bool Acis
        {
            get { return acis; }
            set
            {
                if (acis != value)
                {
                    acis = value;
                    OnPropertyChanged("Acis");
                }
            }
        }

        [PropertyOrder(3)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_3")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_3")]
        [DefaultValue(false)]
        public bool Bitmap
        {
            get { return bitmap; }
            set
            {
                if (bitmap != value)
                {
                    bitmap = value;
                    OnPropertyChanged("Bitmap");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(4)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_4")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_4")]
        [DefaultValue(false)]
        public bool Bmf
        {
            get { return bmf; }
            set
            {
                if (bmf != value)
                {
                    bmf = value;
                    OnPropertyChanged("Bmf");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(5)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_5")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_5")]
        [DefaultValue(false)]
        public bool Emf
        {
            get { return emf; }
            set
            {
                if (emf != value)
                {
                    emf = value;
                    OnPropertyChanged("Emf");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(6)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_6")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_6")]
        [DefaultValue(false)]
        public bool Iges
        {
            get { return iges; }
            set
            {
                if (iges != value)
                {
                    iges = value;
                    OnPropertyChanged("Iges");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(7)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_7")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_7")]
        [DefaultValue(false)]
        public bool Jt
        {
            get { return jt; }
            set
            {
                if (jt != value)
                {
                    jt = value;
                    OnPropertyChanged("Jt");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(8)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_8")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_8")]
        [DefaultValue(false)]
        public bool Parasolid
        {
            get { return parasolid; }
            set
            {
                if (parasolid != value)
                {
                    parasolid = value;
                    OnPropertyChanged("Parasolid");
                }
            }
        }

        [PropertyOrder(9)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_9")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_9")]
        [DefaultValue(false)]
        public bool Pdf
        {
            get { return pdf; }
            set
            {
                if (pdf != value)
                {
                    pdf = value;
                    OnPropertyChanged("Pdf");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(10)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_10")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_10")]
        [DefaultValue(false)]
        public bool Step
        {
            get { return step; }
            set
            {
                if (step != value)
                {
                    step = value;
                    OnPropertyChanged("Step");
                }
            }
        }

        [Browsable(false)]
        [PropertyOrder(11)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_11")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_11")]
        [DefaultValue(false)]
        public bool Stl
        {
            get { return stl; }
            set
            {
                if (stl != value)
                {
                    stl = value;
                    OnPropertyChanged("Iges");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string[] values = new string[12];

            values[00] = document  ? "01" : "00";
            values[01] = acad      ? "01" : "00";
            values[02] = acis      ? "01" : "00";
            values[03] = bitmap    ? "01" : "00";
            values[04] = bmf       ? "01" : "00";
            values[05] = emf       ? "01" : "00";
            values[06] = iges      ? "01" : "00";
            values[07] = jt        ? "01" : "00";
            values[08] = parasolid ? "01" : "00";
            values[09] = pdf       ? "01" : "00";
            values[10] = step      ? "01" : "00";
            values[11] = stl       ? "01" : "00";

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

    internal class InputExtensionItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { "*.grb", "GRB" }
            };
        }
    }
}