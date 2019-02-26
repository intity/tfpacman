using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Export
{
    [CustomCategoryOrder(Resource.PACKAGE_1, 3)]
    [CustomCategoryOrder(Resource.PACKAGE_1, 4)]
    public class ExportToPackage1 : ExportTo
    {
        #region private fields
        private int extension;
        private int autocadExportFileVersion;
        private int convertAreas;
        private int convertToLines;
        private int convertDimensions;
        private int convertLineText;
        private int convertMultitext;
        private int biarcInterpolationForSplines;
        private decimal biarcInterpolationAccuracyForSplines;

        private readonly byte[] objState = new byte[9];
        private readonly int[] i_values = new int[8];
        private readonly decimal[] m_values = new decimal[1];
        private bool isChanged;
        #endregion

        public ExportToPackage1(Header header) : base(header)
        {
            extension = 0;
            autocadExportFileVersion = 3;
            convertAreas = 0;
            convertToLines = 1;
            convertDimensions = 0;
            convertLineText = 0;
            convertMultitext = 0;
            biarcInterpolationForSplines = 0;
            biarcInterpolationAccuracyForSplines = 0.1m;

            OutputExtension = "DWG";
        }

        #region internal properties
        internal override bool IsChanged
        {
            get
            {
                return (isChanged ? true : base.IsChanged);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// The AutoCAD file format:
        /// (0) - DWG,
        /// (1) - DXF,
        /// (2) - DXB
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.PACKAGE_1, "category3")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn3_1")]
        [CustomDescription(Resource.PACKAGE_1, "dn3_1")]
        [ItemsSource(typeof(ExtensionItems0))]
        [DefaultValue(0)]
        public int Extension
        {
            get
            {
                return extension;
            }
            set
            {
                if (value != extension)
                {
                    extension = value;
                    switch (extension)
                    {
                        case 0: OutputExtension = "DWG"; break;
                        case 1: OutputExtension = "DXF"; break;
                        case 2: OutputExtension = "DXB"; break;
                    }
                    OnChanged(15);
                }
            }
        }

        /// <summary>
        /// The AutoCAD file version definition:
        /// (0) - 12,
        /// (1) - 13,
        /// (2) - 14,
        /// (3) - 2000,
        /// (4) - 2004,
        /// (5) - 2007,
        /// (6) - 2010,
        /// (7) - 2013
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resource.PACKAGE_1, "category3")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn3_2")]
        [CustomDescription(Resource.PACKAGE_1, "dn3_2")]
        [ItemsSource(typeof(AutocadExportFileVersionItems))]
        [DefaultValue(3)]
        public int AutocadExportFileVersion
        {
            get { return autocadExportFileVersion; }
            set
            {
                if (autocadExportFileVersion != value)
                {
                    autocadExportFileVersion = value;
                    OnChanged(16);
                }
            }
        }

        /// <summary>
        /// Convert hatching to: 
        /// (0) - to line, 
        /// (1) - to hatching.
        /// </summary>
        [PropertyOrder(18)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_1")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_1")]
        [ItemsSource(typeof(ConvertAreasItems))]
        [DefaultValue(0)]
        public int ConvertAreas
        {
            get { return convertAreas; }
            set
            {
                if (convertAreas != value)
                {
                    convertAreas = value;
                    OnChanged(17);
                }
            }
        }

        /// <summary>
        /// Convert line to: 
        /// (0) - in a polyline, 
        /// (1) - in a line.
        /// </summary>
        [PropertyOrder(19)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_2")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_2")]
        [ItemsSource(typeof(ConvertToLinesItems))]
        [DefaultValue(1)]
        public int ConvertToLines
        {
            get { return convertToLines; }
            set
            {
                if (convertToLines != value)
                {
                    convertToLines = value;
                    OnChanged(18);
                }
            }
        }

        /// <summary>
        /// Convert dimension to: 
        /// (0) - in the dimensions, 
        /// (1) - in the dimensions and texts, 
        /// (2) - in lines and texts.
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_3")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_3")]
        [ItemsSource(typeof(ConvertDimensionsItems))]
        [DefaultValue(0)]
        public int ConvertDimensions
        {
            get { return convertDimensions; }
            set
            {
                if (convertDimensions != value)
                {
                    convertDimensions = value;
                    OnChanged(19);
                }
            }
        }

        /// <summary>
        /// Convert lowercase texts: 
        /// (0) - to text, 
        /// (1) - to line.
        /// </summary>
        [PropertyOrder(21)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_4")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_4")]
        [ItemsSource(typeof(ConvertLineTextItems))]
        [DefaultValue(0)]
        public int ConvertLineText
        {
            get { return convertLineText; }
            set
            {
                if (convertLineText != value)
                {
                    convertLineText = value;
                    OnChanged(20);
                }
            }
        }

        /// <summary>
        /// Convert multitexts: 
        /// (0) - selectively, 
        /// (1) - in multitexts, 
        /// (2) - in texts in a line.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_5")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_5")]
        [ItemsSource(typeof(ConvertMultitextItems))]
        [DefaultValue(0)]
        public int ConvertMultitext
        {
            get { return convertMultitext; }
            set
            {
                if (convertMultitext != value)
                {
                    convertMultitext = value;
                    OnChanged(21);
                }
            }
        }

        /// <summary>
        /// Export splines using circular interpolation: 
        /// (0) - Polylines, 
        /// (1) - Polylines with interpolation.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_6")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_6")]
        [ItemsSource(typeof(BiarcInterpolationForSplinesItems))]
        [DefaultValue(0)]
        public int BiarcInterpolationForSplines
        {
            get { return biarcInterpolationForSplines; }
            set
            {
                if (biarcInterpolationForSplines != value)
                {
                    biarcInterpolationForSplines = value;
                    OnChanged(22);
                }
            }
        }

        /// <summary>
        /// Accuracy of circular interpolation.
        /// </summary>
        [PropertyOrder(24)]
        [CustomCategory(Resource.PACKAGE_1, "category4")]
        [CustomDisplayName(Resource.PACKAGE_1, "dn4_7")]
        [CustomDescription(Resource.PACKAGE_1, "dn4_7")]
        [Editor(typeof(BiarcInterpolationControl), typeof(UITypeEditor))]
        [DefaultValue(typeof(decimal), "0.1")]
        public decimal BiarcInterpolationAccuracyForSplines
        {
            get { return biarcInterpolationAccuracyForSplines; }
            set
            {
                if (biarcInterpolationAccuracyForSplines != value)
                {
                    biarcInterpolationAccuracyForSplines = value;
                    OnChanged(23);
                }
            }
        }
        #endregion

        #region methods
        private void ExportTo(ExportToACAD export, Page page)
        {
            switch (autocadExportFileVersion)
            {
                case 0:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD12;
                    break;
                case 1:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD13;
                    break;
                case 2:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD14;
                    break;
                case 3:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD2000;
                    break;
                case 4:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD2004;
                    break;
                case 5:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD2007;
                    break;
                case 6:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD2010;
                    break;
                case 7:
                    export.AutocadExportFileVersion = AutocadExportFileVersionType.efACAD2013;
                    break;
            }

            export.ConvertToLines    = ConvertToLines > 0 ? true : false;
            export.ConvertAreas      = ConvertAreas > 0 ? true : false;
            export.ConvertDimensions = ConvertDimensions;
            export.ConvertLineText   = ConvertLineText > 0 ? true : false;
            export.ConvertMultitext  = ConvertMultitext;
            // encoding ?
            if (BiarcInterpolationForSplines == 0)
            {
                export.BiarcInterpolationForSplines = BiarcInterpolationForSplines > 0 ? true : false;
                export.BiarcInterpolationAccuracyForSplines = (double)BiarcInterpolationAccuracyForSplines;
            }
            export.ExportAllPages = false;
            export.Page = page;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            i_values[0] = extension;
            i_values[1] = autocadExportFileVersion;
            i_values[2] = convertAreas;
            i_values[3] = convertToLines;
            i_values[4] = convertDimensions;
            i_values[5] = convertLineText;
            i_values[6] = convertMultitext;
            i_values[7] = biarcInterpolationForSplines;
            m_values[0] = biarcInterpolationAccuracyForSplines;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
        }

        public override void OnChanged(int index)
        {
            bool result = false;
            if (!IsLoaded) return;

            switch (index)
            {
                case 15:
                    if (i_values[0] != extension)
                        objState[0] = 1;
                    else
                        objState[0] = 0;
                    break;
                case 16:
                    if (i_values[1] != autocadExportFileVersion)
                        objState[1] = 1;
                    else
                        objState[1] = 0;
                    break;
                case 17:
                    if (i_values[2] != convertAreas)
                        objState[2] = 1;
                    else
                        objState[2] = 0;
                    break;
                case 18:
                    if (i_values[3] != convertToLines)
                        objState[3] = 1;
                    else
                        objState[3] = 0;
                    break;
                case 19:
                    if (i_values[4] != convertDimensions)
                        objState[4] = 1;
                    else
                        objState[4] = 0;
                    break;
                case 20:
                    if (i_values[5] != convertLineText)
                        objState[5] = 1;
                    else
                        objState[5] = 0;
                    break;
                case 21:
                    if (i_values[6] != convertMultitext)
                        objState[6] = 1;
                    else
                        objState[6] = 0;
                    break;
                case 22:
                    if (i_values[7] != biarcInterpolationForSplines)
                        objState[7] = 1;
                    else
                        objState[7] = 0;
                    break;
                case 23:
                    if (m_values[0] != biarcInterpolationAccuracyForSplines)
                        objState[8] = 1;
                    else
                        objState[8] = 0;
                    break;
            }

            foreach (var i in objState)
            {
                if (i > 0)
                {
                    result = true;
                    break;
                }
            }

            isChanged = result;
            base.OnChanged(index);
        }

        public override bool Export(Document document, Page page, string outputPath)
        {
            ExportToDWG export1;
            ExportToDXF export2;
            ExportToDXB export3;

            string fullPathName = ReplaceFullPathName(outputPath);

            switch (Extension)
            {
                case 0:
                    export1 = new ExportToDWG(document);
                    ExportTo(export1, page);
                    return export1.Export(fullPathName);
                case 1:
                    export2 = new ExportToDXF(document);
                    ExportTo(export2, page);
                    return export2.Export(fullPathName);
                case 2:
                    export3 = new ExportToDXB(document);
                    ExportTo(export3, page);
                    return export3.Export(fullPathName);
            }
            return false;
        }

        internal override void AppendPackageToXml(XElement parent, PackageType package)
        {
            base.AppendPackageToXml(parent, package);

            string value = Enum.GetName(typeof(PackageType), package);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter", 
                    new XAttribute("name", "OutputExtension"), 
                    new XAttribute("value", 
                    extension == 0 ? "DWG" : 
                    extension == 1 ? "DXF" : "DXB")),
                new XElement("parameter",
                    new XAttribute("name", "AutocadExportFileVersion"),
                    new XAttribute("value", autocadExportFileVersion)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertAreas"),
                    new XAttribute("value", convertAreas)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertToLines"),
                    new XAttribute("value", convertToLines)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertDimensions"),
                    new XAttribute("value", convertDimensions)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertLineText"),
                    new XAttribute("value", convertLineText)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertMultitext"),
                    new XAttribute("value", convertMultitext)),
                new XElement("parameter",
                    new XAttribute("name", "BiarcInterpolationForSplines"),
                    new XAttribute("value", biarcInterpolationForSplines)),
                new XElement("parameter",
                    new XAttribute("name", "BiarcInterpolationAccuracyForSplines"),
                    new XAttribute("value", biarcInterpolationAccuracyForSplines)));
        }

        internal override void PackageTask(XElement element, int flag)
        {
            base.PackageTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "OutputExtension":
                    if (flag == 0)
                    {
                        switch (value)
                        {
                            case "DWG": extension = 0; break;
                            case "DXF": extension = 1; break;
                            case "DXB": extension = 2; break;
                        }
                    }
                    else
                        value = OutputExtension;
                    break;
                case "AutocadExportFileVersion":
                    if (flag == 0)
                        autocadExportFileVersion = int.Parse(value);
                    else
                        value = autocadExportFileVersion.ToString();
                    break;
                case "ConvertAreas":
                    if (flag == 0)
                        convertAreas = int.Parse(value);
                    else
                        value = convertAreas.ToString();
                    break;
                case "ConvertToLines":
                    if (flag == 0)
                        convertToLines = int.Parse(value);
                    else
                        value = convertToLines.ToString();
                    break;
                case "ConvertDimensions":
                    if (flag == 0)
                        convertDimensions = int.Parse(value);
                    else
                        value = convertDimensions.ToString();
                    break;
                case "ConvertLineText":
                    if (flag == 0)
                        convertLineText = int.Parse(value);
                    else
                        value = convertLineText.ToString();
                    break;
                case "ConvertMultitext":
                    if (flag == 0)
                        convertMultitext = int.Parse(value);
                    else
                        value = convertMultitext.ToString();
                    break;
                case "BiarcInterpolationForSplines":
                    if (flag == 0)
                        biarcInterpolationForSplines = int.Parse(value);
                    else
                        value = biarcInterpolationForSplines.ToString();
                    break;
                case "BiarcInterpolationAccuracyForSplines":
                    if (flag == 0)
                        biarcInterpolationAccuracyForSplines = decimal.Parse(value,
                            NumberStyles.Float, CultureInfo.InvariantCulture);
                    else
                        value = biarcInterpolationAccuracyForSplines.ToString(CultureInfo.InvariantCulture);
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

    internal class ExtensionItems0 : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, "DWG" },
                { 1, "DXF" },
                { 2, "DXB" }
            };
        }
    }

    internal class AutocadExportFileVersionItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0,   "12" }, 
                { 1,   "13" }, 
                { 2,   "14" }, 
                { 3, "2000" }, 
                { 4, "2004" }, 
                { 5, "2007" }, 
                { 6, "2010" }, 
                { 7, "2013" }
            };
        }
    }

    internal class ConvertAreasItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_1_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_1_1", 0) }
            };
        }
    }

    internal class ConvertToLinesItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_2_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_2_1", 0) }
            };
        }
    }

    internal class ConvertDimensionsItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_3_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_3_1", 0) },
                { 2, Resource.GetString(Resource.PACKAGE_1, "dn4_3_2", 0) }
            };
        }
    }

    internal class ConvertLineTextItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_4_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_4_1", 0) }
            };
        }
    }

    internal class ConvertMultitextItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_5_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_5_1", 0) },
                { 2, Resource.GetString(Resource.PACKAGE_1, "dn4_5_2", 0) }
            };
        }
    }

    internal class BiarcInterpolationForSplinesItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.PACKAGE_1, "dn4_6_0", 0) },
                { 1, Resource.GetString(Resource.PACKAGE_1, "dn4_6_1", 0) }
            };
        }
    }
}