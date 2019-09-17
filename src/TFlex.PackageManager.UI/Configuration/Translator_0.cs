using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707
#pragma warning disable CA1819

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The document translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 1)]
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 2)]
    [CustomCategoryOrder(Resource.TRANSLATOR_0, 3)]
    public class Translator_0 : Category_3
    {
        #region private fields
        string [] pageNames;
        bool excludePage;
        decimal pageScale;
        bool checkDrawingTemplate;
        string[] projectionNames;
        bool excludeProjection;
        decimal projectionScale;

        XAttribute data_1_1;
        XAttribute data_1_2;
        XAttribute data_1_3;
        XAttribute data_1_4;
        XAttribute data_1_5;
        XAttribute data_2_1;
        XAttribute data_2_2;
        XAttribute data_2_3;
        XElement data_3_1;
        XElement data_3_2;
        XElement data_3_3;
        XElement data_3_4;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_0(string ext = "GRB") : base (ext)
        {
            pageNames            = new string[] { };
            excludePage          = false;
            pageScale            = 99999;
            PageTypes            = new PageTypes();
            PageTypes.PropertyChanged += PageTypes_PropertyChanged;
            checkDrawingTemplate = false;
            projectionNames      = new string[] { };
            excludeProjection    = false;
            projectionScale      = 99999;
        }

        private void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            data_1_4.Value = (sender as PageTypes).ToString();
        }

        #region public properties
        /// <summary>
        /// The page name list.
        /// </summary>
        [PropertyOrder(1)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_1")]
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
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_3")]
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
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4")]
        [ExpandableObject]
        [Editor(typeof(PageTypesEditor), typeof(UITypeEditor))]
        public PageTypes PageTypes { get; }

        /// <summary>
        /// Check the drawing template.
        /// </summary>
        [PropertyOrder(5)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5")]
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

        /// <summary>
        /// The projection name list.
        /// </summary>
        [PropertyOrder(6)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_1")]
        [ExpandableObject]
        [Editor(typeof(StringArrayEditor), typeof(UITypeEditor))]
        public string[] ProjectionNames
        {
            get => projectionNames;
            set
            {
                if (projectionNames != value)
                {
                    projectionNames = value;
                    data_2_1.Value = value.ToString("\r\n");

                    OnPropertyChanged("ProjectionNames");
                }
            }
        }

        /// <summary>
        /// Exclude the projection names from search.
        /// </summary>
        [Browsable(false)]
        public bool ExcludeProjection
        {
            get => excludeProjection;
            set
            {
                if (excludeProjection != value)
                {
                    excludeProjection = value;
                    data_2_2.Value = value ? "1" : "0";

                    OnPropertyChanged("ExcludeProjection");
                }
            }
        }

        /// <summary>
        /// The projection scale value.
        /// </summary>
        [PropertyOrder(8)]
        [CustomCategory(Resource.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn2_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn2_3")]
        [Editor(typeof(ScaleEditor), typeof(UITypeEditor))]
        public decimal ProjectionScale
        {
            get => projectionScale;
            set
            {
                if (projectionScale != value)
                {
                    projectionScale = value;
                    data_2_3.Value = value
                        .ToString(CultureInfo.InvariantCulture);

                    OnPropertyChanged("ProjectionScale");
                }
            }
        }

        /// <summary>
        /// Add variables.
        /// </summary>
        [PropertyOrder(9)]
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_1")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public XElement AddVariables { get => data_3_1; }

        /// <summary>
        /// Edit variables.
        /// </summary>
        [PropertyOrder(10)]
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_2")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public XElement EditVariables { get => data_3_2; }

        /// <summary>
        /// Rename variables.
        /// </summary>
        [PropertyOrder(11)]
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_3")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public XElement RenameVariables { get => data_3_3; }

        /// <summary>
        /// Remove variables.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resource.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn3_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn3_4")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public XElement RemoveVariables { get => data_3_4; }
        #endregion

        #region internal properties
        internal override TranslatorType Mode => TranslatorType.Document;

        internal override uint Processing
        {
            get => (uint)ProcessingMode.SaveAs;
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
            data_2_1 = new XAttribute("value", ProjectionNames.ToString("\r\n"));
            data_2_2 = new XAttribute("value", ExcludeProjection ? "1" : "0");
            data_2_3 = new XAttribute("value", ProjectionScale.ToString(CultureInfo.InvariantCulture));

            data_3_1 = new XElement("parameter", new XAttribute("name", "AddVariables"));
            data_3_2 = new XElement("parameter", new XAttribute("name", "EditVariables"));
            data_3_3 = new XElement("parameter", new XAttribute("name", "RenameVariables"));
            data_3_4 = new XElement("parameter", new XAttribute("name", "RemoveVariables"));

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
            data.Add(new XElement("parameter",
                new XAttribute("name", "ProjectionNames"),
                data_2_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExcludeProjection"),
                data_2_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ProjectionScale"),
                data_2_3));

            data.Add(data_3_1);
            data.Add(data_3_2);
            data.Add(data_3_3);
            data.Add(data_3_4);

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
                case "ProjectionNames":
                    projectionNames = a.Value.Length > 0 
                        ? a.Value.Replace("\r", "").Split('\n') 
                        : new string[] { };
                    data_2_1 = a;
                    break;
                case "ExcludeProjection":
                    excludeProjection = a.Value == "1";
                    data_2_2 = a;
                    break;
                case "ProjectionScale":
                    projectionScale = decimal.Parse(a.Value, 
                        NumberStyles.Float, 
                        CultureInfo.InvariantCulture);
                    data_2_3 = a;
                    break;
                case "AddVariables":
                    data_3_1 = element;
                    break;
                case "EditVariables":
                    data_3_2 = element;
                    break;
                case "RenameVariables":
                    data_3_3 = element;
                    break;
                case "RemoveVariables":
                    data_3_4 = element;
                    break;
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_1")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_2")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_3")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_4")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_5")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_6")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_6")]
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

            normal          = values[0] == "01";
            workplane       = values[1] == "01";
            auxiliary       = values[2] == "01";
            text            = values[3] == "01";
            billOfMaterials = values[4] == "01";
            circuit         = values[5] == "01";
        }

        public override string ToString()
        {
            string[] values = new string[6];

            values[0] = normal          ? "01" : "00";
            values[1] = workplane       ? "01" : "00";
            values[2] = auxiliary       ? "01" : "00";
            values[3] = text            ? "01" : "00";
            values[4] = billOfMaterials ? "01" : "00";
            values[5] = circuit         ? "01" : "00";

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
}