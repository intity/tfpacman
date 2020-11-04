using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// Projections extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.PROJECTIONS, 2), Serializable]
    public class Projections : Pages
    {
        #region private fields
        string[] projectionNames;
        bool excludeProjection;
        decimal projectionScale;
        #endregion

        public Projections()
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
                    OnPropertyChanged("ProjectionScale");
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 3 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "ProjectionNames":
                        var value = reader.GetAttribute(1);
                        projectionNames = value.Length > 0 
                            ? value.Replace("\r", "").Split('\n') 
                            : new string[] { };
                        break;
                    case "ExcludeProjection":
                        excludeProjection = reader.GetAttribute(1) == "1";
                        break;
                    case "ProjectionScale":
                        projectionScale = decimal.Parse(reader.GetAttribute(1), 
                            CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ProjectionNames");
            writer.WriteAttributeString("value", ProjectionNames.ToString("\r\n"));
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExcludeProjection");
            writer.WriteAttributeString("value", ExcludeProjection ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ProjectionScale");
            writer.WriteAttributeString("value", ProjectionScale
                .ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }
        #endregion
    }
}
