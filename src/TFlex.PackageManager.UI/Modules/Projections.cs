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
    /// Projections extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.PROJECTIONS, 2)]
    public class Projections : Variables
    {
        #region private fields
        string[] projectionNames;
        bool excludeProjection;
        decimal projectionScale;
        XAttribute data_2_1;
        XAttribute data_2_2;
        XAttribute data_2_3;
        #endregion

        public Projections(string ext) : base(ext)
        {
            projectionNames   = new string[] { };
            excludeProjection = false;
            projectionScale   = 99999;
        }

        #region public properties
        /// <summary>
        /// The projection name list.
        /// </summary>
        [PropertyOrder(6)]
        [CustomCategory(Resources.PROJECTIONS, "category2")]
        [CustomDisplayName(Resources.PROJECTIONS, "dn2_1")]
        [CustomDescription(Resources.PROJECTIONS, "dn2_1")]
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
        [CustomCategory(Resources.PROJECTIONS, "category2")]
        [CustomDisplayName(Resources.PROJECTIONS, "dn2_3")]
        [CustomDescription(Resources.PROJECTIONS, "dn2_3")]
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
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();
            data_2_1 = new XAttribute("value", ProjectionNames.ToString("\r\n"));
            data_2_2 = new XAttribute("value", ExcludeProjection ? "1" : "0");
            data_2_3 = new XAttribute("value", ProjectionScale.ToString(CultureInfo.InvariantCulture));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ProjectionNames"),
                data_2_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExcludeProjection"),
                data_2_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ProjectionScale"),
                data_2_3));
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
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
            }
        }
        #endregion
    }
}
