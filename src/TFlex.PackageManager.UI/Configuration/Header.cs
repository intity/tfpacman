using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
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
        Translator_0 translator_0;
        Translator_1 translator_1;
        Translator_2 translator_2;
        Translator_3 translator_3;
        Translator_6 translator_6;
        Translator_7 translator_7;
        Translator_9 translator_9;
        Translator_10 translator_10;

        string configurationName;
        string initialCatalog;
        string targetDirectory;
        string inputExtension;
        XElement translators;
        XAttribute data_1_1;
        XAttribute data_1_2;
        XAttribute data_1_3;
        XAttribute data_1_4;
        XAttribute data_1_5;

        bool isChanged;
        #endregion

        public Header()
        {
            configurationName = string.Empty;
            initialCatalog    = string.Empty;
            targetDirectory   = string.Empty;
            inputExtension    = "*.grb";
            Translators       = new ObservableDictionary<string, object>();
            TranslatorTypes   = new TranslatorTypes();
            TranslatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;
        }

        #region event handlers
        private void DataContext_Changed(object sender, XObjectChangeEventArgs e)
        {
            IsChanged = true;

            Debug.WriteLine(string.Format("DataContext_Changed: [{0}]", e.ObjectChange));
        }

        private void Translator_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            IsInvalid = (sender as Category_3).HasErrors;
        }

        private void TranslatorTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool value = false;
            var t_mode = sender as TranslatorTypes;
            InitTranslator(e.PropertyName);
            data_1_5.Value = t_mode.ToString();

            switch (e.PropertyName)
            {
                case "Document":
                    if (value = t_mode.Document)
                    {
                        if (translator_0.Data == null)
                            translators.Add(translator_0.NewTranslator());
                        else
                            translators.Add(translator_0.Data);
                    }
                    break;
                case "Acad":
                    if (value = t_mode.Acad)
                    {
                        if (translator_1.Data == null)
                            translators.Add(translator_1.NewTranslator());
                        else
                            translators.Add(translator_1.Data);
                    }
                    break;
                case "Acis":
                    if (value = t_mode.Acis)
                    {
                        if (translator_2.Data == null)
                            translators.Add(translator_2.NewTranslator());
                        else
                            translators.Add(translator_2.Data);
                    }
                    break;
                case "Bitmap":
                    if (value = t_mode.Bitmap)
                    {
                        if (translator_3.Data == null)
                            translators.Add(translator_3.NewTranslator());
                        else
                            translators.Add(translator_3.Data);
                    }
                    break;
                case "Iges":
                    if (value = t_mode.Iges)
                    {
                        if (translator_6.Data == null)
                            translators.Add(translator_6.NewTranslator());
                        else
                            translators.Add(translator_6.Data);
                    }
                    break;
                case "Jt":
                    if (value = t_mode.Jt)
                    {
                        if (translator_7.Data == null)
                            translators.Add(translator_7.NewTranslator());
                        else
                            translators.Add(translator_7.Data);
                    }
                    break;
                case "Pdf":
                    if (value = t_mode.Pdf)
                    {
                        if (translator_9.Data == null)
                            translators.Add(translator_9.NewTranslator());
                        else
                            translators.Add(translator_9.Data);
                    }
                    break;
                case "Step":
                    if (value = t_mode.Step)
                    {
                        if (translator_10.Data == null)
                            translators.Add(translator_10.NewTranslator());
                        else
                            translators.Add(translator_10.Data);
                    }
                    break;
            }

            if (!value)
            {
                Translators.Remove(e.PropertyName);

                foreach (var i in translators.Elements())
                {
                    if (i.Attribute("id").Value == e.PropertyName)
                    {
                        i.Remove();
                        break;
                    }
                }
            }

            //Debug.WriteLine(string.Format("TranslatorTypes_PropertyChanged [name: {0}, value: {1}]",
            //    e.PropertyName, value));
        }
        #endregion

        #region internal properties
        /// <summary>
        /// XML data context.
        /// </summary>
        internal XDocument DataContext { get; private set; }

        /// <summary>
        /// Translator list.
        /// </summary>
        internal ObservableDictionary<string, object> Translators { get; }

        /// <summary>
        /// The configuration is changed.
        /// </summary>
        internal bool IsChanged
        {
            get => isChanged;
            private set
            {
                if (isChanged != value)
                {
                    isChanged = value;
                    OnPropertyChanged("IsChanged");
                }
            }
        }

        /// <summary>
        /// Validating this configuration properties.
        /// </summary>
        internal bool IsInvalid { get; private set; }

        /// <summary>
        /// Configurations directory.
        /// </summary>
        internal string UserDirectory { get; set; }
        #endregion

        #region public properties
        /// <summary>
        /// The configuration name definition.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_1")]
        [CustomDescription(Resource.HEADER_UI, "dn1_1")]
        [ReadOnly(true)]
        public string ConfigurationName
        {
            get => configurationName;
            set
            {
                if (configurationName != value)
                {
                    configurationName = value;
                }
            }
        }

        /// <summary>
        /// The initial catalog definition.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_2")]
        [CustomDescription(Resource.HEADER_UI, "dn1_2")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        public string InitialCatalog
        {
            get { return initialCatalog; }
            set
            {
                if (initialCatalog != value)
                {
                    initialCatalog = value;
                    data_1_2.Value = value;

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
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        public string TargetDirectory
        {
            get => targetDirectory;
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;
                    data_1_3.Value = value;

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
                }
            }
        }

        /// <summary>
        /// The translator types.
        /// </summary>
        [PropertyOrder(5)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5")]
        [ExpandableObject]
        [Editor(typeof(TranslatorTypesEditor), typeof(UITypeEditor))]
        public TranslatorTypes TranslatorTypes { get; }
        #endregion

        #region private methods
        /// <summary>
        /// Initialize the translator.
        /// </summary>
        /// <param name="name">The translator name.</param>
        private void InitTranslator(string name)
        {
            switch (name)
            {
                case "Document":
                    if (translator_0 == null)
                    {
                        translator_0 = new Translator_0();
                        translator_0.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_0);
                    break;
                case "Acad":
                    if (translator_1 == null)
                    {
                        translator_1 = new Translator_1();
                        translator_1.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_1);
                    break;
                case "Acis":
                    if (translator_2 == null)
                    {
                        translator_2 = new Translator_2();
                        translator_2.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_2);
                    break;
                case "Bitmap":
                    if (translator_3 == null)
                    {
                        translator_3 = new Translator_3();
                        translator_3.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_3);
                    break;
                case "Iges":
                    if (translator_6 == null)
                    {
                        translator_6 = new Translator_6();
                        translator_6.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_6);
                    break;
                case "Jt":
                    if (translator_7 == null)
                    {
                        translator_7 = new Translator_7();
                        translator_7.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_7);
                    break;
                case "Pdf":
                    if (translator_9 == null)
                    {
                        translator_9 = new Translator_9();
                        translator_9.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_9);
                    break;
                case "Step":
                    if (translator_10 == null)
                    {
                        translator_10 = new Translator_10();
                        translator_10.ErrorsChanged += Translator_ErrorsChanged;
                    }
                    if (Translators.ContainsKey(name) == false)
                        Translators.Add(name, translator_10);
                    break;
            }

            if (TargetDirectory.Length > 0)
            {
                string path = Path.Combine(TargetDirectory, name);
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Load translators data.
        /// </summary>
        internal void LoadTranslators()
        {
            foreach (XElement i in translators.Elements())
            {
                string name = i.Attribute("id").Value;
                InitTranslator(name);

                switch (name)
                {
                    case "Document":
                        translator_0.LoadTranslator(i);
                        break;
                    case "Acad":
                        translator_1.LoadTranslator(i);
                        break;
                    case "Acis":
                        translator_2.LoadTranslator(i);
                        break;
                    case "Bitmap":
                        translator_3.LoadTranslator(i);
                        break;
                    case "Iges":
                        translator_6.LoadTranslator(i);
                        break;
                    case "Jt":
                        translator_7.LoadTranslator(i);
                        break;
                    case "Pdf":
                        translator_9.LoadTranslator(i);
                        break;
                    case "Step":
                        translator_10.LoadTranslator(i);
                        break;
                }
            }
        }

        /// <summary>
        /// Create new configuration.
        /// </summary>
        /// <returns>The XML-data for new configuration.</returns>
        internal XDocument NewConfiguration()
        {
            data_1_1 = new XAttribute("value", ConfigurationName);
            data_1_2 = new XAttribute("value", InitialCatalog);
            data_1_3 = new XAttribute("value", TargetDirectory);
            data_1_4 = new XAttribute("value", InputExtension);
            data_1_5 = new XAttribute("value", TranslatorTypes.ToString());

            translators = new XElement("translators");

            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("configuration", 
                    new XElement("header",
                        new XElement("parameter",
                            new XAttribute("name", "ConfigurationName"),
                            data_1_1),
                        new XElement("parameter",
                            new XAttribute("name", "InitialCatalog"),
                            data_1_2),
                        new XElement("parameter",
                            new XAttribute("name", "TargetDirectory"),
                            data_1_3),
                        new XElement("parameter",
                            new XAttribute("name", "InputExtension"),
                            data_1_4),
                        new XElement("parameter",
                            new XAttribute("name", "TranslatorTypes"),
                            data_1_5)), translators));

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
            var path = UserDirectory + "\\" + ConfigurationName + ".config";

            if (flag == 0 && File.Exists(path))
            {
                DataContext = XDocument.Load(path);
                translators = DataContext
                    .Element("configuration")
                    .Element("translators");

                var header = DataContext
                    .Element("configuration")
                    .Element("header");

                foreach (var p in header.Elements())
                {
                    LoadHeader(p);
                }

                LoadTranslators();
                DataContext.Changed += DataContext_Changed;
                IsChanged = false;
            }

            if (flag != 2)
            {
                if (DataContext == null)
                {
                    DataContext = NewConfiguration();
                    DataContext.Changed += DataContext_Changed;
                    IsChanged = false;
                }

                if (flag == 1)
                {
                    DataContext.Save(path);
                    IsChanged = false;
                }
            }
            else
            {
                File.Delete(path);
            }

            //Debug.WriteLine(string.Format("ConfigurationTask [flag: {0}, path: {1}]", flag, path));
        }

        /// <summary>
        /// Extension method for processing header parameters.
        /// </summary>
        /// <param name="element">Source header element.</param>
        private void LoadHeader(XElement element)
        {
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "ConfigurationName":
                    configurationName = a.Value;
                    data_1_1 = a;
                    break;
                case "InitialCatalog":
                    initialCatalog = a.Value;
                    data_1_2 = a;
                    break;
                case "TargetDirectory":
                    targetDirectory = a.Value;
                    data_1_3 = a;
                    break;
                case "InputExtension":
                    inputExtension = a.Value;
                    data_1_4 = a;
                    break;
                case "TranslatorTypes":
                    TranslatorTypes.SetValue(a.Value);
                    data_1_5 = a;
                    break;
            }
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
        bool document;
        bool acad;
        bool acis;
        bool bitmap;
        bool bmf;
        bool emf;
        bool iges;
        bool jt;
        bool parasolid;
        bool pdf;
        bool step;
        bool stl;
        #endregion

        #region public properties
        [PropertyOrder(0)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_0")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_0")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Document
        {
            get => document;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Acad
        {
            get => acad;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Acis
        {
            get => acis;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Bitmap
        {
            get => bitmap;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Bmf
        {
            get => bmf;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Emf
        {
            get => emf;
            set
            {
                if (emf != value)
                {
                    emf = value;
                    OnPropertyChanged("Emf");
                }
            }
        }

        [PropertyOrder(6)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_6")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Iges
        {
            get => iges;
            set
            {
                if (iges != value)
                {
                    iges = value;
                    OnPropertyChanged("Iges");
                }
            }
        }

        [PropertyOrder(7)]
        [CustomDisplayName(Resource.HEADER_UI, "dn1_5_7")]
        [CustomDescription(Resource.HEADER_UI, "dn1_5_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Jt
        {
            get => jt;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Parasolid
        {
            get => parasolid;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Pdf
        {
            get => pdf;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Step
        {
            get => step;
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
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Stl
        {
            get => stl;
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
        public void SetValue(string value)
        {
            string[] values = value.Split(' ');

            document  = values[00] == "01";
            acad      = values[01] == "01";
            acis      = values[02] == "01";
            bitmap    = values[03] == "01";
            bmf       = values[04] == "01";
            emf       = values[05] == "01";
            iges      = values[06] == "01";
            jt        = values[07] == "01";
            parasolid = values[08] == "01";
            pdf       = values[09] == "01";
            step      = values[10] == "01";
            stl       = values[11] == "01";
        }

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