using System.ComponentModel;
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
        private string [] pageNames;
        private bool excludePage;
        private decimal pageScale;
        private readonly PageTypes pageTypes;
        private bool checkDrawingTemplate;
        private string [] projectionNames;
        private bool excludeProjection;
        private decimal projectionScale;

        private string[] pg_names, pj_names;
        private readonly byte[] objState;
        private readonly bool[] pg_types;
        private readonly bool[] b_values;
        private readonly decimal[] m_values;
        
        private bool isChanged;
        #endregion

        public Translator_0()
        {
            pageNames        = new string[] { };
            pageScale        = 99999;
            pageTypes        = new PageTypes { Normal = true };
            pageTypes.PropertyChanged += PageTypes_PropertyChanged;
            projectionNames  = new string[] { };
            projectionScale  = 99999;

            objState         = new byte[8];
            pg_types         = new bool[6];
            b_values         = new bool[3];
            m_values         = new decimal[2];
        }

        #region event handlers
        private void PageTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChanged(4);
        }
        #endregion

        #region public properties
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
        /// Exclude the page names from search.
        /// </summary>
        [Browsable(false)]
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
        /// The page type list.
        /// </summary>
        [PropertyOrder(4)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4")]
        [ExpandableObject]
        [Editor(typeof(PageTypesEditor), typeof(UITypeEditor))]
        public PageTypes PageTypes
        {
            get { return (pageTypes); }
        }

        /// <summary>
        /// Check the drawing template.
        /// </summary>
        [PropertyOrder(5)]
        [CustomCategory(Resource.TRANSLATOR_0, "category1")]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_5")]
        public bool CheckDrawingTemplate
        {
            get { return checkDrawingTemplate; }
            set
            {
                if (checkDrawingTemplate != value)
                {
                    checkDrawingTemplate = value;
                    OnChanged(5);
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
        [Editor(typeof(InputCollectionControl), typeof(UITypeEditor))]
        public string[] ProjectionNames
        {
            get { return projectionNames; }
            set
            {
                if (projectionNames != value)
                {
                    projectionNames = value;
                    OnChanged(6);
                }
            }
        }

        /// <summary>
        /// Exclude the projection names from search.
        /// </summary>
        [Browsable(false)]
        public bool ExcludeProjection
        {
            get { return excludeProjection; }
            set
            {
                if (excludeProjection != value)
                {
                    excludeProjection = value;
                    OnChanged(7);
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
        [Editor(typeof(InputScaleControl), typeof(UITypeEditor))]
        public decimal ProjectionScale
        {
            get { return projectionScale; }
            set
            {
                if (projectionScale != value)
                {
                    projectionScale = value;
                    OnChanged(8);
                }
            }
        }
        #endregion

        #region internal properties
        internal override bool IsChanged
        {
            get
            {
                return (isChanged | base.IsChanged);
            }
        }
        #endregion

        #region internal methods
        internal override void OnLoaded()
        {
            pg_names = (string[])pageNames.Clone();
            pj_names = (string[])projectionNames.Clone();

            b_values[0] = excludePage;
            m_values[0] = pageScale;
            b_values[1] = checkDrawingTemplate;
            b_values[2] = excludeProjection;
            m_values[1] = projectionScale;

            pg_types[0] = pageTypes.Normal;
            pg_types[1] = pageTypes.Workplane;
            pg_types[2] = pageTypes.Auxiliary;
            pg_types[3] = pageTypes.Text;
            pg_types[4] = pageTypes.BillOfMaterials;
            pg_types[5] = pageTypes.Circuit;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 1:
                    if (!Enumerable.SequenceEqual(pg_names, pageNames))
                        objState[0] = 1;
                    else
                        objState[0] = 0;
                    break;
                case 2:
                    if (b_values[0] != excludePage)
                        objState[1] = 1;
                    else
                        objState[1] = 0;
                    break;
                case 3:
                    if (m_values[0] != pageScale)
                        objState[2] = 1;
                    else
                        objState[2] = 0;
                    break;
                case 4:
                    if (pg_types[0] != pageTypes.Normal ||
                        pg_types[1] != pageTypes.Workplane ||
                        pg_types[2] != pageTypes.Auxiliary ||
                        pg_types[3] != pageTypes.Text ||
                        pg_types[4] != pageTypes.BillOfMaterials || 
                        pg_types[5] != pageTypes.Circuit)
                        objState[3] = 1;
                    else
                        objState[3] = 0;
                    break;
                case 5:
                    if (b_values[1] != checkDrawingTemplate)
                        objState[4] = 1;
                    else
                        objState[4] = 0;
                    break;
                case 6:
                    if (!Enumerable.SequenceEqual(pj_names, projectionNames))
                        objState[5] = 1;
                    else
                        objState[5] = 0;
                    break;
                case 7:
                    if (b_values[2] != excludeProjection)
                        objState[6] = 1;
                    else
                        objState[6] = 0;
                    break;
                case 8:
                    if (m_values[1] != projectionScale)
                        objState[7] = 1;
                    else
                        objState[7] = 0;
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

            base.OnChanged(index);
        }

        internal override XElement NewTranslator(TranslatorType translator)
        {
            XElement element = new XElement("translator", 
                new XAttribute("id", translator), 
                new XAttribute("processing", Processing),
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
                    new XAttribute("name", "FileNameSuffix"),
                    new XAttribute("value", FileNameSuffix)),
                new XElement("parameter",
                    new XAttribute("name", "TemplateFileName"),
                    new XAttribute("value", TemplateFileName)));

            return element;
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

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
                        excludePage = value == "1";
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
                case "PageTypes":
                    if (flag == 0)
                    {
                        string[] values = value.Split(' ');

                        pageTypes.Normal          = values[0] == "01";
                        pageTypes.Workplane       = values[1] == "01";
                        pageTypes.Auxiliary       = values[2] == "01";
                        pageTypes.Text            = values[3] == "01";
                        pageTypes.BillOfMaterials = values[4] == "01";
                        pageTypes.Circuit         = values[5] == "01";
                    }
                    else
                        value = pageTypes.ToString();
                    break;
                case "CheckDrawingTemplate":
                    if (flag == 0)
                        checkDrawingTemplate = value == "1";
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
                        excludeProjection = value == "1";
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
            }
            element.Attribute("value").Value = value;
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
        private bool circuit;
        #endregion

        #region properties
        [PropertyOrder(1)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_1")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_1")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_2")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_2")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_3")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_3")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_4")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_4")]
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
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_5")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_5")]
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

        [PropertyOrder(6)]
        [CustomDisplayName(Resource.TRANSLATOR_0, "dn1_4_6")]
        [CustomDescription(Resource.TRANSLATOR_0, "dn1_4_6")]
        public bool Circuit
        {
            get { return circuit; }
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}