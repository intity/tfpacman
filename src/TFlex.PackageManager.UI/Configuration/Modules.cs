using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// The Modules base class definition.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "parameter")]
    public class Modules : IXmlSerializable, INotifyPropertyChanged
    {
        #region private fields
        bool pages;
        bool projections;
        bool variables;
        bool links;
        #endregion

        public Modules() { }
        public Modules(int index)
        {
            Index = index;
        }

        #region public properies
        /// <summary>
        /// Modules Index.
        /// </summary>
        [Browsable(false)]
        public int Index { get; private set; }

        /// <summary>
        /// Module for processing document links.
        /// </summary>
        [PropertyOrder(0)]
        [CustomDisplayName(Resources.MODULES_UI, "dn1_5_0")]
        [CustomDescription(Resources.MODULES_UI, "dn1_5_0")]
        public virtual bool Links
        {
            get => links;
            set
            {
                if (links != value)
                {
                    links = value;
                    OnPropertyChanged("Links");
                }
            }
        }

        /// <summary>
        /// Module for processing document pages.
        /// </summary>
        [PropertyOrder(1)]
        [CustomDisplayName(Resources.MODULES_UI, "dn1_5_1")]
        [CustomDescription(Resources.MODULES_UI, "dn1_5_1")]
        public virtual bool Pages
        {
            get => pages;
            set
            {
                if (pages != value)
                {
                    pages = value;
                    OnPropertyChanged("Pages");
                }
            }
        }

        /// <summary>
        /// Module for processing document projections.
        /// </summary>
        [PropertyOrder(2)]
        [CustomDisplayName(Resources.MODULES_UI, "dn1_5_2")]
        [CustomDescription(Resources.MODULES_UI, "dn1_5_2")]
        public virtual bool Projections
        {
            get => projections;
            set
            {
                if (projections != value)
                {
                    projections = value;
                    OnPropertyChanged("Projections");
                }
            }
        }

        /// <summary>
        /// Module for processing document variables.
        /// </summary>
        [PropertyOrder(3)]
        [CustomDisplayName(Resources.MODULES_UI, "dn1_5_3")]
        [CustomDescription(Resources.MODULES_UI, "dn1_5_3")]
        public virtual bool Variables
        {
            get => variables;
            set
            {
                if (variables != value)
                {
                    variables = value;
                    OnPropertyChanged("Variables");
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string[] values;
            var data = reader.GetAttribute("value");
            if (data != null && (values = data.Split(' ')).Length >= 4)
            {
                links       = values[0] == "1";
                pages       = values[1] == "1";
                projections = values[2] == "1";
                variables   = values[3] == "1";
            }
            Index = int.Parse(reader.GetAttribute("index"));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", "Modules");
            string[] values = new string[] {
                Links       ? "1" : "0",
                Pages       ? "1" : "0",
                Projections ? "1" : "0",
                Variables   ? "1" : "0"
            };
            writer.WriteAttributeString("value", values.ToString(" "));
            writer.WriteAttributeString("index", Index.ToString());
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

    /// <summary>
    /// Modules for Document translator.
    /// </summary>
    public class M0 : Modules
    {
        public M0(int index) : base (index)
        {
            Links = true;
        }
    }

    /// <summary>
    /// Modules for Acad, Bitmap and PDF translators.
    /// </summary>
    public class M1 : Modules
    {
        public M1(int index) : base (index)
        {
            Links = true;
            Pages = true;
        }

        [Browsable(false)]
        public override bool Variables
        {
            get => base.Variables;
            set => base.Variables = value;
        }
    }

    /// <summary>
    /// Modules for ACIS, IGES, JT and STEP translators.
    /// </summary>
    public class M2 : Modules
    {
        public M2(int index) : base (index)
        {
            // ...
        }

        [Browsable(false)]
        public override bool Pages
        {
            get => base.Pages;
            set => base.Pages = value;
        }

        [Browsable(false)]
        public override bool Projections
        {
            get => base.Projections;
            set => base.Projections = value;
        }

        [Browsable(false)]
        public override bool Variables
        {
            get => base.Variables;
            set => base.Variables = value;
        }

        [Browsable(false)]
        public override bool Links
        {
            get => base.Links;
            set => base.Links = value;
        }
    }
}
