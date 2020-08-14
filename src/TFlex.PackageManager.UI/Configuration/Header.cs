using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Configuration class.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "configuration")]
    public class Header : IXmlSerializable, INotifyPropertyChanged
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

        string initialCatalog;
        string targetDirectory;
        object modules;
        object translator;

        int m_index;
        bool isChanged;
        Dictionary<int, object> translators;
        List<object> opt_modules;
        #endregion

        public Header()
        {
            InitModules();
        }

        public Header(string name)
        {
            ConfigName = name;
            InitModules();
            Modules = opt_modules[0];
        }

        #region event handlers
        private void Translator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }

        private void Translator_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            IsInvalid = (sender as Translator).HasErrors;
        }
        
        private void Modules_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }
        #endregion

        #region internal properties
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
        /// The Translator type Index.
        /// </summary>
        internal int TIndex
        {
            get => m_index;
            set
            {
                if (m_index != value)
                {
                    m_index = value;
                    Modules = opt_modules[value];
                }
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// The configuration name definition.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resources.HEADER_UI, "dn1_1")]
        [CustomDescription(Resources.HEADER_UI, "dn1_1")]
        [ReadOnly(true)]
        public string ConfigName { get; private set; }

        /// <summary>
        /// The initial catalog definition.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resources.HEADER_UI, "dn1_2")]
        [CustomDescription(Resources.HEADER_UI, "dn1_2")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        public string InitialCatalog
        {
            get => initialCatalog ?? string.Empty;
            set
            {
                if (initialCatalog != value)
                {
                    initialCatalog = value;
                    IsChanged = true;
                    OnPropertyChanged("InitialCatalog");
                }
            }
        }

        /// <summary>
        /// The target directory definition.
        /// </summary>
        [PropertyOrder(3)]
        [CustomDisplayName(Resources.HEADER_UI, "dn1_3")]
        [CustomDescription(Resources.HEADER_UI, "dn1_3")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        public string TargetDirectory
        {
            get => targetDirectory ?? string.Empty;
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;
                    IsChanged = true;
                    OnPropertyChanged("TargetDirectory");
                }
            }
        }

        /// <summary>
        /// The translator modules for current selector.
        /// </summary>
        [PropertyOrder(5)]
        [CustomDisplayName(Resources.HEADER_UI, "dn1_4")]
        [CustomDescription(Resources.HEADER_UI, "dn1_4")]
        [ExpandableObject]
        [Editor(typeof(ModulesEditor), typeof(UITypeEditor))]
        [XmlElement(Type = typeof(Modules))]
        public object Modules
        {
            get => modules;
            set
            {
                if (modules == value)
                    return;

                modules = value;
                InitTranslator();
                OnPropertyChanged("Modules");
            }
        }

        /// <summary>
        /// Reference on current translator.
        /// </summary>
        [Browsable(false)]
        [XmlElement(Type = typeof(Translator))]
        public object Translator
        {
            get => translator;
            set
            {
                if (translator == value)
                    return;

                translator = value;
                IsChanged = true;
                OnPropertyChanged("Translator");
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Initialize the modules container.
        /// </summary>
        private void InitModules()
        {
            translators = new Dictionary<int, object>();
            opt_modules = new List<object>
            {
                new M0(0), // Document
                new M1(1), // Acad
                new M2(2), // Acis
                new M1(3), // Bitmap
                new M2(4), // Iges
                new M2(5), // Jt
                new M1(6), // Pdf
                new M2(7), // Step
            };
            foreach (var m in opt_modules)
            {
                (m as Modules).PropertyChanged += Modules_PropertyChanged;
            }
        }

        /// <summary>
        /// Initialize the translator.
        /// </summary>
        private void InitTranslator()
        {
            switch (TIndex)
            {
                case 0: // Document
                    if (translator_0 == null)
                    {
                        translator_0 = new Translator_0();
                        translator_0.PropertyChanged += Translator_PropertyChanged;
                        translator_0.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_0);
                    }
                    break;
                case 1: // Acad
                    if (translator_1 == null)
                    {
                        translator_1 = new Translator_1();
                        translator_1.PropertyChanged += Translator_PropertyChanged;
                        translator_1.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_1);
                    }
                    break;
                case 2: // ACIS
                    if (translator_2 == null)
                    {
                        translator_2 = new Translator_2();
                        translator_2.PropertyChanged += Translator_PropertyChanged;
                        translator_2.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_2);
                    }
                    break;
                case 3: // Bitmap
                    if (translator_3 == null)
                    {
                        translator_3 = new Translator_3();
                        translator_3.PropertyChanged += Translator_PropertyChanged;
                        translator_3.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_3);
                    }
                    break;
                case 4: // IGES
                    if (translator_6 == null)
                    {
                        translator_6 = new Translator_6();
                        translator_6.PropertyChanged += Translator_PropertyChanged;
                        translator_6.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_6);
                    }
                    break;
                case 5: // JT
                    if (translator_7 == null)
                    {
                        translator_7 = new Translator_7();
                        translator_7.PropertyChanged += Translator_PropertyChanged;
                        translator_7.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_7);
                    }
                    break;
                case 6: // PDF
                    if (translator_9 == null)
                    {
                        translator_9 = new Translator_9();
                        translator_9.PropertyChanged += Translator_PropertyChanged;
                        translator_9.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_9);
                    }
                    break;
                case 7: // STEP
                    if (translator_10 == null)
                    {
                        translator_10 = new Translator_10();
                        translator_10.PropertyChanged += Translator_PropertyChanged;
                        translator_10.ErrorsChanged   += Translator_ErrorsChanged;
                        translators.Add(TIndex, translator_10);
                    }
                    break;
            }
            Translator = translators[TIndex];
            //Debug.WriteLine(string.Format("InitTranslator [index: {0}]", TIndex));
        }
        #endregion

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            ConfigName = reader.GetAttribute(0);

            while (reader.Read())
            {
                if (reader.Name == "parameter")
                {
                    switch (reader.GetAttribute(0))
                    {
                        case "InitialCatalog":
                            initialCatalog = reader.GetAttribute(1);
                            break;
                        case "TargetDirectory":
                            targetDirectory = reader.GetAttribute(1);
                            break;
                        case "Modules":
                            var obj = new XmlSerializer(typeof(Modules)).Deserialize(reader);
                            if (obj != null)
                            {
                                m_index = (obj as Modules).Index;
                                Modules = opt_modules[m_index];
                            }
                            (Modules as Modules).ReadXml(reader);
                            break;
                    }
                }
                if (reader.Name == "translator" && reader.IsStartElement())
                {
                    (Translator as Translator).ReadXml(reader);
                }
            }
            IsChanged = false;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", ConfigName);
            writer.WriteStartElement("header");
            {
                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", "InitialCatalog");
                writer.WriteAttributeString("value", InitialCatalog);
                writer.WriteEndElement();

                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", "TargetDirectory");
                writer.WriteAttributeString("value", TargetDirectory);
                writer.WriteEndElement();

                writer.WriteStartElement("parameter");
                (Modules as Modules).WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("translator");
            (Translator as Translator).WriteXml(writer);
            writer.WriteEndElement();
            
            IsChanged = false;
        }
        #endregion

        #region INotifyPropertyChanged Members
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
}