using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TFlex.Model;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// Pages extension modules.
    /// </summary>
    [CustomCategoryOrder(Resources.PAGES, 1), Serializable]
    public class Pages : Links
    {
        #region private fields
        string[] pageNames;
        bool excludePage;
        decimal pageScale;
        bool checkDrawingTemplate;
        #endregion

        public Pages()
        {
            pageNames   = new string[] { };
            excludePage = false;
            pageScale   = 99999;
            PageTypes   = new PageTypes();
            PageTypes.PropertyChanged += PageTypes_PropertyChanged;
        }

        #region public properties
        /// <summary>
        /// The page name list.
        /// </summary>
        [PropertyOrder(1)]
        [CustomCategory(Resources.PAGES, "category1")]
        [CustomDisplayName(Resources.PAGES, "dn1_1")]
        [CustomDescription(Resources.PAGES, "dn1_1")]
        [ExpandableObject]
        [Editor(typeof(StringArrayEditor), typeof(UITypeEditor))]
        public string[] PageNames
        {
            get => pageNames;
            set
            {
                if (pageNames != value)
                {
                    pageNames = value;
                    OnPropertyChanged("PageNames");
                }
            }
        }

        /// <summary>
        /// Exclude the page names from search.
        /// </summary>
        [Browsable(false)]
        public bool ExcludePage
        {
            get => excludePage;
            set
            {
                if (excludePage != value)
                {
                    excludePage = value;
                    OnPropertyChanged("ExcludePage");
                }
            }
        }

        /// <summary>
        /// The page scale value.
        /// </summary>
        [PropertyOrder(3)]
        [CustomCategory(Resources.PAGES, "category1")]
        [CustomDisplayName(Resources.PAGES, "dn1_3")]
        [CustomDescription(Resources.PAGES, "dn1_3")]
        [Editor(typeof(ScaleEditor), typeof(UITypeEditor))]
        public decimal PageScale
        {
            get => pageScale;
            set
            {
                if (pageScale != value)
                {
                    pageScale = value;
                    OnPropertyChanged("PageScale");
                }
            }
        }

        /// <summary>
        /// The page type list.
        /// </summary>
        [PropertyOrder(4)]
        [CustomCategory(Resources.PAGES, "category1")]
        [CustomDisplayName(Resources.PAGES, "dn1_4")]
        [CustomDescription(Resources.PAGES, "dn1_4")]
        [ExpandableObject]
        [Editor(typeof(PageTypesEditor), typeof(UITypeEditor))]
        public PageTypes PageTypes { get; }

        /// <summary>
        /// Check the drawing template.
        /// </summary>
        [PropertyOrder(5)]
        [CustomCategory(Resources.PAGES, "category1")]
        [CustomDisplayName(Resources.PAGES, "dn1_5")]
        [CustomDescription(Resources.PAGES, "dn1_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool CheckDrawingTemplate
        {
            get => checkDrawingTemplate;
            set
            {
                if (checkDrawingTemplate != value)
                {
                    checkDrawingTemplate = value;
                    OnPropertyChanged("CheckDrawingTemplate");
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 5 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "PageNames":
                        var value = reader.GetAttribute(1);
                        pageNames = value.Length > 0 
                            ? value.Replace("\r", "").Split('\n') 
                            : new string[] { };
                        break;
                    case "ExcludePage":
                        excludePage = reader.GetAttribute(1) == "1";
                        break;
                    case "PageScale":
                        pageScale = decimal.Parse(reader.GetAttribute(1), 
                            CultureInfo.InvariantCulture);
                        break;
                    case "PageTypes":
                        PageTypes.ReadXml(reader);
                        break;
                    case "CheckDrawingTemplate":
                        checkDrawingTemplate = reader.GetAttribute(1) == "1";
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "PageNames");
            writer.WriteAttributeString("value", PageNames.ToString("\r\n"));
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExcludePage");
            writer.WriteAttributeString("value", ExcludePage ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "PageScale");
            writer.WriteAttributeString("value", PageScale
                .ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();


            writer.WriteStartElement("parameter");
            PageTypes.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "CheckDrawingTemplate");
            writer.WriteAttributeString("value", CheckDrawingTemplate ? "1" : "0");
            writer.WriteEndElement();
        }
        #endregion

        #region internal methods
        /// <summary>
        /// The Page type exists.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal bool PageTypeExists(Page page)
        {
            Dictionary<PageType, bool> types = new Dictionary<PageType, bool>
            {
                { PageType.Normal,          PageTypes.Normal },
                { PageType.Workplane,       PageTypes.Workplane },
                { PageType.Auxiliary,       PageTypes.Auxiliary },
                { PageType.Text,            PageTypes.Text },
                { PageType.BillOfMaterials, PageTypes.BillOfMaterials },
                { PageType.Circuit,         PageTypes.Circuit },
                { PageType.Projection,      PageTypes.Projection }
            };
            return page.PageType != PageType.Dialog && types[page.PageType];
        }

        /// <summary>
        /// The Drawing Template exists.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal bool DrawingTemplateExists(Document document, Page page)
        {
            return document.GetFragments()
                .Where(f => f.GroupType == ObjectType.Fragment && f.Page == page && 
                f.DisplayName.Contains("<Форматки>")).FirstOrDefault() != null;
        }
        #endregion

        #region events
        void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        #endregion
    }

    /// <summary>
    /// The page types class.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "parameter")]
    public class PageTypes : IXmlSerializable, INotifyPropertyChanged
    {
        #region private fields
        bool normal;
        bool workplane;
        bool auxiliary;
        bool text;
        bool billOfMaterials;
        bool circuit;
        bool projection;
        #endregion

        public PageTypes()
        {
            normal = true;
        }

        #region public properties
        [PropertyOrder(1)]
        [CustomDisplayName(Resources.PAGES, "dn1_4_1")]
        [CustomDescription(Resources.PAGES, "dn1_4_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Normal
        {
            get => normal;
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
        [CustomDisplayName(Resources.PAGES, "dn1_4_2")]
        [CustomDescription(Resources.PAGES, "dn1_4_2")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Workplane
        {
            get => workplane;
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
        [CustomDisplayName(Resources.PAGES, "dn1_4_3")]
        [CustomDescription(Resources.PAGES, "dn1_4_3")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Auxiliary
        {
            get => auxiliary;
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
        [CustomDisplayName(Resources.PAGES, "dn1_4_4")]
        [CustomDescription(Resources.PAGES, "dn1_4_4")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Text
        {
            get => text;
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
        [CustomDisplayName(Resources.PAGES, "dn1_4_5")]
        [CustomDescription(Resources.PAGES, "dn1_4_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool BillOfMaterials
        {
            get => billOfMaterials;
            set
            {
                if (billOfMaterials != value)
                {
                    billOfMaterials = value;
                    OnPropertyChanged("BillOfMaterials");
                }
            }
        }

        [PropertyOrder(6)]
        [CustomDisplayName(Resources.PAGES, "dn1_4_6")]
        [CustomDescription(Resources.PAGES, "dn1_4_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Circuit
        {
            get => circuit;
            set
            {
                if (circuit != value)
                {
                    circuit = value;
                    OnPropertyChanged("Circuit");
                }
            }
        }

        [PropertyOrder(7)]
        [CustomDisplayName(Resources.PAGES, "dn1_4_7")]
        [CustomDescription(Resources.PAGES, "dn1_4_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Projection
        {
            get => projection;
            set
            {
                if (projection != value)
                {
                    projection = value;
                    OnPropertyChanged("Projection");
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
            if (data != null && (values = data.Split(' ')).Length == 7)
            {
                normal          = values[0] == "1";
                workplane       = values[1] == "1";
                auxiliary       = values[2] == "1";
                text            = values[3] == "1";
                billOfMaterials = values[4] == "1";
                circuit         = values[5] == "1";
                projection      = values[6] == "1";
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", "PageTypes");
            string[] values = new string[]
            {
                Normal          ? "1" : "0",
                Workplane       ? "1" : "0",
                Auxiliary       ? "1" : "0",
                Text            ? "1" : "0",
                BillOfMaterials ? "1" : "0",
                Circuit         ? "1" : "0",
                Projection      ? "1" : "0"
            };
            writer.WriteAttributeString("value", values.ToString(" "));
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
