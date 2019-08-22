using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Configuration Header class.
    /// </summary>
    public class Header : INotifyPropertyChanged
    {
        #region private fields
        private Translator_0 translator_0;
        private Translator_1 translator_1;
        private Translator_2 translator_2;
        private Translator_3 translator_3;
        private Translator_9 translator_9;
        private Translator_10 translator_10;
        private readonly ObservableDictionary<string, object> translators;

        private string configurationName;
        private string initialCatalog;
        private string targetDirectory;
        private string inputExtension;
        private readonly TranslatorTypes translatorTypes;

        private readonly byte[] objState;
        private readonly string[] s_values;
        private readonly bool[] tr_types;
        private readonly byte[] tchanges;
        private bool isLoaded, isChanged, isInvalid;
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

            objState          = new byte[6];
            s_values          = new string[4];
            tr_types          = new bool[12];
            tchanges          = new byte[6];
        }

        #region event handlers
        private void Translator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender.Equals(translator_0))
            {
                tchanges[0] = (byte)(translator_0.IsChanged ? 1 : 0);
            }
            else if (sender.Equals(translator_1))
            {
                tchanges[1] = (byte)(translator_1.IsChanged ? 1 : 0);
            }
            else if (sender.Equals(translator_2))
            {
                tchanges[2] = (byte)(translator_2.IsChanged ? 1 : 0);
            }
            else if (sender.Equals(translator_3))
            {
                tchanges[3] = (byte)(translator_3.IsChanged ? 1 : 0);
            }
            else if (sender.Equals(translator_9))
            {
                tchanges[4] = (byte)(translator_9.IsChanged ? 1 : 0);
            }
            else if (sender.Equals(translator_10))
            {
                tchanges[5] = (byte)(translator_10.IsChanged ? 1 : 0);
            }

            objState[5] = 0;

            foreach (var i in tchanges)
            {
                if (i > 0)
                {
                    objState[5] = 1;
                    break;
                }
            }

            OnChanged(5);

            //Debug.WriteLine(string.Format("IsChanged [value: {0}]", isChanged));
        }

        private void Translator_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            isInvalid = (sender as Category_3).HasErrors;
        }

        private void TranslatorTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool t_value = false;

            switch (e.PropertyName)
            {
                case "Document":
                    if (t_value = (sender as TranslatorTypes).Document)
                    {
                        if (translator_0 == null)
                        {
                            translator_0 = new Translator_0();
                            translator_0.PropertyChanged += Translator_PropertyChanged;
                            translator_0.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_0);
                    }
                    break;
                case "Acad":
                    if (t_value = (sender as TranslatorTypes).Acad)
                    {
                        if (translator_1 == null)
                        {
                            translator_1 = new Translator_1();
                            translator_1.PropertyChanged += Translator_PropertyChanged;
                            translator_1.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_1);
                    }   
                    break;
                case "Acis":
                    if (t_value = (sender as TranslatorTypes).Acis)
                    {
                        if (translator_2 == null)
                        {
                            translator_2 = new Translator_2();
                            translator_2.PropertyChanged += Translator_PropertyChanged;
                            translator_2.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_2);
                    }
                    break;
                case "Bitmap":
                    if (t_value = (sender as TranslatorTypes).Bitmap)
                    {
                        if (translator_3 == null)
                        {
                            translator_3 = new Translator_3();
                            translator_3.PropertyChanged += Translator_PropertyChanged;
                            translator_3.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_3);
                    }
                    break;
                case "Pdf":
                    if (t_value = (sender as TranslatorTypes).Pdf)
                    {
                        if (translator_9 == null)
                        {
                            translator_9 = new Translator_9();
                            translator_9.PropertyChanged += Translator_PropertyChanged;
                            translator_9.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_9);
                    }
                    break;
                case "Step":
                    if (t_value = (sender as TranslatorTypes).Step)
                    {
                        if (translator_10 == null)
                        {
                            translator_10 = new Translator_10();
                            translator_10.PropertyChanged += Translator_PropertyChanged;
                            translator_10.ErrorsChanged   += Translator_ErrorsChanged;
                        }

                        translators.Add(e.PropertyName, translator_10);
                    }
                    break;
            }

            if (t_value)
            {
                if (targetDirectory.Length > 0)
                {
                    string path = Path.Combine(targetDirectory, e.PropertyName);
                    if (Directory.Exists(path) == false)
                        Directory.CreateDirectory(path);
                }
            }
            else
                translators.Remove(e.PropertyName);
            //Debug.WriteLine(string.Format("TranslatorTypes_PropertyChanged [name: {0}, value: {1}]", 
            //    e.PropertyName, t_value));

            OnChanged(4);
        }
        #endregion

        #region internal properties
        /// <summary>
        /// Translator list.
        /// </summary>
        internal ObservableDictionary<string, object> Translators
        {
            get { return (translators); }
        }

        /// <summary>
        /// The configuration is changed.
        /// </summary>
        internal bool IsChanged { get { return (isChanged); } }

        /// <summary>
        /// The configuration is loaded.
        /// </summary>
        internal bool IsLoaded { get { return (isLoaded); } }

        /// <summary>
        /// Validating this configuration properties.
        /// </summary>
        internal bool IsInvalid { get { return (isInvalid); } }

        /// <summary>
        /// Configurations directory.
        /// </summary>
        internal string UserDirectory { get; set; }
        #endregion

        #region properties
        /// <summary>
        /// The configuration name definition.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_1")]
        [CustomDescription(Resource.HEADER_UI, "dn1_1")]
        [ReadOnly(true)]
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
                    OnPropertyChanged("InitialCatalog");
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
                    OnPropertyChanged("TargetDirectory");
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
        /// <summary>
        /// Cloneable values this object on loaded.
        /// </summary>
        private void OnLoaded()
        {
            s_values[00] = configurationName;
            s_values[01] = initialCatalog;
            s_values[02] = targetDirectory;
            s_values[03] = inputExtension;

            tr_types[00] = translatorTypes.Document;
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

            for (int i = 0; i < tchanges.Length; i++)
                tchanges[i] = 0;

            isLoaded = true;

            if (isChanged)
            {
                isChanged = false;
                OnPropertyChanged("IsChanged");
            }
        }

        /// <summary>
        /// Verified this object a change and calls event for 'IsChanged' property.
        /// </summary>
        /// <param name="index">Property index.</param>
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
                    if (tr_types[00] != translatorTypes.Document || 
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

            OnPropertyChanged("IsChanged");
        }

        /// <summary>
        /// Extension method to get translators.
        /// </summary>
        /// <param name="element"></param>
        internal void GetTranslators(XElement element)
        {
            foreach (XElement i in element.Element("translators").Elements())
            {
                switch (i.Attribute("id").Value)
                {
                    case "Document":
                        translatorTypes.Document = true;
                        translator_0.ConfigurationTask(i, 0);
                        break;
                    case "Acad":
                        translatorTypes.Acad = true;
                        translator_1.ConfigurationTask(i, 0);
                        break;
                    case "Acis":
                        translatorTypes.Acis = true;
                        translator_2.ConfigurationTask(i, 0);
                        break;
                    case "Bitmap":
                        translatorTypes.Bitmap = true;
                        translator_3.ConfigurationTask(i, 0);
                        break;
                    case "Pdf":
                        translatorTypes.Pdf = true;
                        translator_9.ConfigurationTask(i, 0);
                        break;
                    case "Step":
                        translatorTypes.Step = true;
                        translator_10.ConfigurationTask(i, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Extension method to set translators.
        /// </summary>
        /// <param name="element"></param>
        internal void SetTranslators(XElement element)
        {
            bool t_value = false;
            XElement parent = element.Element("translators");
            List<XElement> elements = new List<XElement>();

            foreach (XElement e in parent.Elements())
            {
                elements.Add(e);
            }

            foreach (XElement i in elements)
            {
                switch (i.Attribute("id").Value)
                {
                    case "Document":
                        if (t_value = translatorTypes.Document)
                        {
                            translator_0.ConfigurationTask(i, 1);
                        }
                        break;
                    case "Acad":
                        if (t_value = translatorTypes.Acad)
                        {
                            translator_1.ConfigurationTask(i, 1);
                        }
                        break;
                    case "Acis":
                        if (t_value = translatorTypes.Acis)
                        {
                            translator_2.ConfigurationTask(i, 1);
                        }
                        break;
                    case "Bitmap":
                        if (t_value = translatorTypes.Bitmap)
                        {
                            translator_3.ConfigurationTask(i, 1);
                        }
                        break;
                    case "Pdf":
                        if (t_value = translatorTypes.Pdf)
                        {
                            translator_9.ConfigurationTask(i, 1);
                        }
                        break;
                    case "Step":
                        if (t_value = translatorTypes.Step)
                        {
                            translator_10.ConfigurationTask(i, 1);
                        }
                        break;
                }

                if (!t_value) i.Remove();
            }

            foreach (var i in translators)
            {
                switch (i.Key)
                {
                    case "Document":
                        if (translator_0.IsLoaded == false)
                        {
                            translator_0.AppendTranslatorToXml(parent, TranslatorType.Document);
                        }
                        break;
                    case "Acad":
                        if (translator_1.IsLoaded == false)
                        {
                            translator_1.AppendTranslatorToXml(parent, TranslatorType.Acad);
                        }
                        break;
                    case "Acis":
                        if (translator_2.IsLoaded == false)
                        {
                            translator_2.AppendTranslatorToXml(parent, TranslatorType.Acis);
                        }
                        break;
                    case "Bitmap":
                        if (translator_3.IsLoaded == false)
                        {
                            translator_3.AppendTranslatorToXml(parent, TranslatorType.Bitmap);
                        }
                        break;
                    case "Pdf":
                        if (translator_9.IsLoaded == false)
                        {
                            translator_9.AppendTranslatorToXml(parent, TranslatorType.Pdf);
                        }
                        break;
                    case "Step":
                        if (translator_10.IsLoaded == false)
                        {
                            translator_10.AppendTranslatorToXml(parent, TranslatorType.Step);
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
                    new XElement("translators")));

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
            string path = UserDirectory + "\\" + configurationName + ".config";

            if (flag != 2)
            {
                XDocument document;
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
                    GetTranslators(element);
                }
                else
                {
                    SetTranslators(element);
                    element.Save(path);
                }

                OnLoaded();
            }
            else
            {
                File.Delete(path);
                isLoaded = false;
            }

            //Debug.WriteLine(string.Format("ConfigurationTask [flag: {0}, path: {1}]", flag, path));
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

                        translatorTypes.Document  = values[00] == "01";
                        translatorTypes.Acad      = values[01] == "01";
                        translatorTypes.Acis      = values[02] == "01";
                        translatorTypes.Bitmap    = values[03] == "01";
                        translatorTypes.Bmf       = values[04] == "01";
                        translatorTypes.Emf       = values[05] == "01";
                        translatorTypes.Iges      = values[06] == "01";
                        translatorTypes.Jt        = values[07] == "01";
                        translatorTypes.Parasolid = values[08] == "01";
                        translatorTypes.Pdf       = values[09] == "01";
                        translatorTypes.Step      = values[10] == "01";
                        translatorTypes.Stl       = values[11] == "01";
                    }
                    else
                        value = translatorTypes.ToString();
                    break;
            }
            element.Attribute("value").Value = value;
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
        public bool Document
        {
            get { return document; }
            set
            {
                if (document != value)
                {
                    document = value;
                    OnPropertyChanged("Document");
                }
            }
        }

        [PropertyOrder(1)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_1")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_1")]
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

        [PropertyOrder(2)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_2")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_2")]
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

        [PropertyOrder(10)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_10")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_10")]
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
        public bool Stl
        {
            get { return stl; }
            set
            {
                if (stl != value)
                {
                    stl = value;
                    OnPropertyChanged("Stl");
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
    }

#pragma warning disable CA1812
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