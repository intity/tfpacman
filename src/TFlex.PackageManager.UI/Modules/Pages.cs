using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Pages extension modules.
    /// </summary>
    [CustomCategoryOrder(Resources.PAGES, 1)]
    public class Pages : Files
    {
        #region private fields
        string[] pageNames;
        bool excludePage;
        decimal pageScale;
        bool checkDrawingTemplate;
        XAttribute data_1_1;
        XAttribute data_1_2;
        XAttribute data_1_3;
        XAttribute data_1_4;
        XAttribute data_1_5;
        #endregion

        public Pages(string ext) : base(ext)
        {
            pageNames = new string[] { };
            excludePage = false;
            pageScale = 99999;
            PageTypes = new PageTypes();
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
                    data_1_1.Value = value.ToString("\r\n");
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
                    data_1_2.Value = value ? "1" : "0";
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
                    data_1_3.Value = value
                        .ToString(CultureInfo.InvariantCulture);
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
                    data_1_5.Value = value ? "1" : "0";
                    OnPropertyChanged("CheckDrawingTemplate");
                }
            }
        }
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();
            data_1_1 = new XAttribute("value", PageNames.ToString("\r\n"));
            data_1_2 = new XAttribute("value", ExcludePage ? "1" : "0");
            data_1_3 = new XAttribute("value", PageScale.ToString(CultureInfo.InvariantCulture));
            data_1_4 = new XAttribute("value", PageTypes.ToString());
            data_1_5 = new XAttribute("value", CheckDrawingTemplate ? "1" : "0");
            data.Add(new XElement("parameter",
                new XAttribute("name", "PageNames"),
                data_1_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExcludePage"),
                data_1_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "PageScale"),
                data_1_3));
            data.Add(new XElement("parameter",
                new XAttribute("name", "PageTypes"),
                data_1_4));
            data.Add(new XElement("parameter",
                new XAttribute("name", "CheckDrawingTemplate"),
                data_1_5));
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "PageNames":
                    pageNames = a.Value.Length > 0
                        ? a.Value.Replace("\r", "").Split('\n')
                        : new string[] { };
                    data_1_1 = a;
                    break;
                case "ExcludePage":
                    excludePage = a.Value == "1";
                    data_1_2 = a;
                    break;
                case "PageScale":
                    pageScale = decimal.Parse(a.Value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture);
                    data_1_3 = a;
                    break;
                case "PageTypes":
                    PageTypes.SetValue(a.Value);
                    data_1_4 = a;
                    break;
                case "CheckDrawingTemplate":
                    checkDrawingTemplate = a.Value == "1";
                    data_1_5 = a;
                    break;
            }
        }
        #endregion

        #region events
        void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            data_1_4.Value = (sender as PageTypes).ToString();
        }
        #endregion
    }

    /// <summary>
    /// The page types class.
    /// </summary>
    public class PageTypes : INotifyPropertyChanged
    {
        #region private fields
        bool normal;
        bool workplane;
        bool auxiliary;
        bool text;
        bool billOfMaterials;
        bool circuit;
        #endregion

        public PageTypes()
        {
            normal = true;
        }

        internal bool IsChanged { get; private set; }

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
        #endregion

        #region methods
        public void SetValue(string value)
        {
            string[] values = value.Split(' ');

            normal          = values[0] == "1";
            workplane       = values[1] == "1";
            auxiliary       = values[2] == "1";
            text            = values[3] == "1";
            billOfMaterials = values[4] == "1";
            circuit         = values[5] == "1";
        }

        public override string ToString()
        {
            string[] values = new string[6];

            values[0] = normal          ? "1" : "0";
            values[1] = workplane       ? "1" : "0";
            values[2] = auxiliary       ? "1" : "0";
            values[3] = text            ? "1" : "0";
            values[4] = billOfMaterials ? "1" : "0";
            values[5] = circuit         ? "1" : "0";

            return values.ToString(" ");
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
