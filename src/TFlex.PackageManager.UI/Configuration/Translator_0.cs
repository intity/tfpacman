using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707
#pragma warning disable CA1819

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The document translator class.
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_0, 2)]
    [CustomCategoryOrder(Resources.TRANSLATOR_0, 3)]
    public class Translator_0 : Links
    {
        #region private fields
        string[] projectionNames;
        bool excludeProjection;
        decimal projectionScale;
        XAttribute data_2_1;
        XAttribute data_2_2;
        XAttribute data_2_3;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_0(string ext = "GRB") : base (ext)
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
        [CustomCategory(Resources.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn2_1")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn2_1")]
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
        [CustomCategory(Resources.TRANSLATOR_0, "category2")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn2_3")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn2_3")]
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
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_1")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_1")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public Variables AddVariables { get; } = new Variables("AddVariables");

        /// <summary>
        /// Edit variables.
        /// </summary>
        [PropertyOrder(10)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_2")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_2")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public Variables EditVariables { get; } = new Variables("EditVariables");

        /// <summary>
        /// Rename variables.
        /// </summary>
        [PropertyOrder(11)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_3")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_3")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public Variables RenameVariables { get; } = new Variables("RenameVariables");

        /// <summary>
        /// Remove variables.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_4")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_4")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public Variables RemoveVariables { get; } = new Variables("RemoveVariables");
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Document;
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

            data.Add(AddVariables.Data);
            data.Add(EditVariables.Data);
            data.Add(RenameVariables.Data);
            data.Add(RemoveVariables.Data);

            PMode = ProcessingMode.SaveAs;
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
                case "AddVariables":
                    AddVariables.LoadData(element);
                    break;
                case "EditVariables":
                    EditVariables.LoadData(element);
                    break;
                case "RenameVariables":
                    RenameVariables.LoadData(element);
                    break;
                case "RemoveVariables":
                    RemoveVariables.LoadData(element);
                    break;
            }
        }
        #endregion
    }
}