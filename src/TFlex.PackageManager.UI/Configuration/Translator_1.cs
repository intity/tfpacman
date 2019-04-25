using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Collections.Generic;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Acad translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_1, 3)]
    [CustomCategoryOrder(Resource.TRANSLATOR_1, 4)]
    public class Translator_1 : Translator_0
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

        private readonly byte[] objState;
        private readonly int[] i_values;
        private readonly decimal[] m_values;
        private bool isChanged;
        #endregion

        public Translator_1()
        {
            extension                            = 0;
            autocadExportFileVersion             = 3;
            convertAreas                         = 0;
            convertToLines                       = 1;
            convertDimensions                    = 0;
            convertLineText                      = 0;
            convertMultitext                     = 0;
            biarcInterpolationForSplines         = 0;
            biarcInterpolationAccuracyForSplines = 0.1m;

            TargetExtension = "DWG";

            objState        = new byte[8];
            i_values        = new int[7];
            m_values        = new decimal[1];
        }

        #region public properties
        /// <summary>
        /// The AutoCAD file format:
        /// (0) - DWG,
        /// (1) - DXF,
        /// (2) - DXB
        /// </summary>
        [PropertyOrder(15)]
        [CustomCategory(Resource.TRANSLATOR_1, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn3_1")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn3_1")]
        [ItemsSource(typeof(ExtensionItems_1))]
        public int Extension
        {
            get
            {
                return extension;
            }
            set
            {
                if (extension != value)
                {
                    extension = value;
                    switch (extension)
                    {
                        case 0: TargetExtension = "DWG"; break;
                        case 1: TargetExtension = "DXF"; break;
                        case 2: TargetExtension = "DXB"; break;
                    }
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
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_1, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn3_2")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn3_2")]
        [ItemsSource(typeof(AutocadExportFileVersionItems))]
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
        [PropertyOrder(17)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_1")]
        [ItemsSource(typeof(ConvertAreasItems))]
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
        [PropertyOrder(18)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_2")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_2")]
        [ItemsSource(typeof(ConvertToLinesItems))]
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
        [PropertyOrder(19)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_3")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_3")]
        [ItemsSource(typeof(ConvertDimensionsItems))]
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
        [PropertyOrder(20)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_4")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_4")]
        [ItemsSource(typeof(ConvertLineTextItems))]
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
        [PropertyOrder(21)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_5")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_5")]
        [ItemsSource(typeof(ConvertMultitextItems))]
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
        [PropertyOrder(22)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_6")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_6")]
        [ItemsSource(typeof(BiarcInterpolationForSplinesItems))]
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
        [PropertyOrder(23)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_7")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_7")]
        [Editor(typeof(BiarcInterpolationControl), typeof(UITypeEditor))]
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

        #region internal properties
        internal override uint Processing
        {
            get { return (uint)ProcessingType.Export; }
        }

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
            i_values[0] = autocadExportFileVersion;
            i_values[1] = convertAreas;
            i_values[2] = convertToLines;
            i_values[3] = convertDimensions;
            i_values[4] = convertLineText;
            i_values[5] = convertMultitext;
            i_values[6] = biarcInterpolationForSplines;
            m_values[0] = biarcInterpolationAccuracyForSplines;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 16: objState[0] = (byte)(i_values[0] != autocadExportFileVersion             ? 1 : 0); break;
                case 17: objState[1] = (byte)(i_values[1] != convertAreas                         ? 1 : 0); break;
                case 18: objState[2] = (byte)(i_values[2] != convertToLines                       ? 1 : 0); break;
                case 19: objState[3] = (byte)(i_values[3] != convertDimensions                    ? 1 : 0); break;
                case 20: objState[4] = (byte)(i_values[4] != convertLineText                      ? 1 : 0); break;
                case 21: objState[5] = (byte)(i_values[5] != convertMultitext                     ? 1 : 0); break;
                case 22: objState[6] = (byte)(i_values[6] != biarcInterpolationForSplines         ? 1 : 0); break;
                case 23: objState[7] = (byte)(m_values[0] != biarcInterpolationAccuracyForSplines ? 1 : 0); break;
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

        internal override void Export(Document document, Dictionary<Page, string> pages, LogFile logFile)
        {
            ExportToDWG export1;
            ExportToDXF export2;
            ExportToDXB export3;
            bool result = false;

            foreach (var p in pages)
            {
                switch (extension)
                {
                    case 0:
                        export1 = new ExportToDWG(document);
                        ExportTo(export1, p.Key);
                        result = export1.Export(p.Value);
                        break;
                    case 1:
                        export2 = new ExportToDXF(document);
                        ExportTo(export2, p.Key);
                        result = export2.Export(p.Value);
                        break;
                    case 2:
                        export3 = new ExportToDXB(document);
                        ExportTo(export3, p.Key);
                        result = export3.Export(p.Value);
                        break;
                }

                if (result)
                {
                    logFile.AppendLine(string.Format("Export to:\t{0}", p.Value));
                }
            }
        }

        internal override void AppendTranslatorToXml(XElement parent, TranslatorType translator)
        {
            base.AppendTranslatorToXml(parent, translator);

            string value = Enum.GetName(typeof(TranslatorType), translator);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter",
                    new XAttribute("name", "FileNameSuffix"),
                    new XAttribute("value", FileNameSuffix)),
                new XElement("parameter",
                    new XAttribute("name", "TemplateFileName"),
                    new XAttribute("value", TemplateFileName)),
                new XElement("parameter",
                    new XAttribute("name", "TargetExtension"),
                    new XAttribute("value", TargetExtension)),
                new XElement("parameter",
                    new XAttribute("name", "AutocadExportFileVersion"),
                    new XAttribute("value", AutocadExportFileVersion)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertAreas"),
                    new XAttribute("value", ConvertAreas)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertToLines"),
                    new XAttribute("value", ConvertToLines)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertDimensions"),
                    new XAttribute("value", ConvertDimensions)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertLineText"),
                    new XAttribute("value", ConvertLineText)),
                new XElement("parameter",
                    new XAttribute("name", "ConvertMultitext"),
                    new XAttribute("value", ConvertMultitext)),
                new XElement("parameter",
                    new XAttribute("name", "BiarcInterpolationForSplines"),
                    new XAttribute("value", BiarcInterpolationForSplines)),
                new XElement("parameter",
                    new XAttribute("name", "BiarcInterpolationAccuracyForSplines"),
                    new XAttribute("value", BiarcInterpolationAccuracyForSplines)));
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "TargetExtension":
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
                        value = TargetExtension;
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

        #region private methods
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

            export.ConvertToLines    = ConvertToLines > 0;
            export.ConvertAreas      = ConvertAreas > 0;
            export.ConvertDimensions = ConvertDimensions;
            export.ConvertLineText   = ConvertLineText > 0;
            export.ConvertMultitext  = ConvertMultitext;
            // encoding ?
            if (biarcInterpolationForSplines > 0)
            {
                export.BiarcInterpolationForSplines = true;
                export.BiarcInterpolationAccuracyForSplines = (double)biarcInterpolationAccuracyForSplines;
            }
            else
            {
                export.BiarcInterpolationForSplines = false;
                export.BiarcInterpolationAccuracyForSplines = 0;
            }

            export.ExportAllPages = false;
            export.Page = page;
        }
        #endregion
    }

#pragma warning disable CA1812
    internal class ExtensionItems_1 : IItemsSource
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
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_1", 0) }
            };
        }
    }

    internal class ConvertToLinesItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_1", 0) }
            };
        }
    }

    internal class ConvertDimensionsItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_1", 0) },
                { 2, Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_2", 0) }
            };
        }
    }

    internal class ConvertLineTextItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_1", 0) }
            };
        }
    }

    internal class ConvertMultitextItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_1", 0) },
                { 2, Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_2", 0) }
            };
        }
    }

    internal class BiarcInterpolationForSplinesItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_1", 0) }
            };
        }
    }
}