using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Xml;
using TFlex.Model;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// The Acad translator class.
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_1, 5), Serializable]
    public class Translator_1 : Translator_0
    {
        #region private fields
        int extension;
        int autocadExportFileVersion;
        int convertAreas;
        int convertToLines;
        int convertDimensions;
        int convertLineText;
        int convertMultitext;
        int biarcInterpolationForSplines;
        decimal biarcInterpolationAccuracyForSplines;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_1()
        {
            autocadExportFileVersion             = 3;
            convertAreas                         = 0;
            convertToLines                       = 1;
            convertDimensions                    = 0;
            convertLineText                      = 0;
            convertMultitext                     = 0;
            biarcInterpolationForSplines         = 0;
            biarcInterpolationAccuracyForSplines = 0.1m;

            IExtension = ".grb";
            OExtension = ".dwg";
        }

        #region public properties
        /// <summary>
        /// The AutoCAD file format:
        /// (0) - DWG,
        /// (1) - DXF,
        /// (2) - DXB
        /// </summary>
        [PropertyOrder(15)]
        [CustomCategory(Resources.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn4_1")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn4_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Extension
        {
            get => extension;
            set
            {
                if (extension != value)
                {
                    extension = value;
                    switch (value)
                    {
                        case 0: OExtension = ".dwg"; break;
                        case 1: OExtension = ".dxf"; break;
                        case 2: OExtension = ".dxb"; break;
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
        [CustomCategory(Resources.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn4_2")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn4_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int AutocadExportFileVersion
        {
            get => autocadExportFileVersion;
            set
            {
                if (autocadExportFileVersion != value)
                {
                    autocadExportFileVersion = value;
                    OnPropertyChanged("AutocadExportFileVersion");
                }
            }
        }

        /// <summary>
        /// Convert hatching to: 
        /// (0) - to line, 
        /// (1) - to hatching.
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_1")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertAreas
        {
            get => convertAreas;
            set
            {
                if (convertAreas != value)
                {
                    convertAreas = value;
                    OnPropertyChanged("ConvertAreas");
                }
            }
        }

        /// <summary>
        /// Convert line to: 
        /// (0) - in a polyline, 
        /// (1) - in a line.
        /// </summary>
        [PropertyOrder(18)]
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_2")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertToLines
        {
            get => convertToLines;
            set
            {
                if (convertToLines != value)
                {
                    convertToLines = value;
                    OnPropertyChanged("ConvertToLines");
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
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_3")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_3")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertDimensions
        {
            get => convertDimensions;
            set
            {
                if (convertDimensions != value)
                {
                    convertDimensions = value;
                    OnPropertyChanged("ConvertDimensions");
                }
            }
        }

        /// <summary>
        /// Convert lowercase texts: 
        /// (0) - to text, 
        /// (1) - to line.
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_4")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_4")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertLineText
        {
            get => convertLineText;
            set
            {
                if (convertLineText != value)
                {
                    convertLineText = value;
                    OnPropertyChanged("ConvertLineText");
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
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_5")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_5")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertMultitext
        {
            get => convertMultitext;
            set
            {
                if (convertMultitext != value)
                {
                    convertMultitext = value;
                    OnPropertyChanged("ConvertMultitext");
                }
            }
        }

        /// <summary>
        /// Export splines using circular interpolation: 
        /// (0) - Polylines, 
        /// (1) - Polylines with interpolation.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_6")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_6")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int BiarcInterpolationForSplines
        {
            get => biarcInterpolationForSplines;
            set
            {
                if (biarcInterpolationForSplines != value)
                {
                    biarcInterpolationForSplines = value;
                    OnPropertyChanged("BiarcInterpolationForSplines");
                }
            }
        }

        /// <summary>
        /// Accuracy of circular interpolation.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resources.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_1, "dn5_7")]
        [CustomDescription(Resources.TRANSLATOR_1, "dn5_7")]
        [Editor(typeof(BiarcInterpolationEditor), typeof(UITypeEditor))]
        public decimal BiarcInterpolationAccuracyForSplines
        {
            get => biarcInterpolationAccuracyForSplines;
            set
            {
                if (biarcInterpolationAccuracyForSplines != value)
                {
                    biarcInterpolationAccuracyForSplines = value;
                    OnPropertyChanged("BiarcInterpolationAccuracyForSplines");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Acad;
        internal override ProcessingMode PMode => ProcessingMode.Export;
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 8 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "AutocadExportFileVersion":
                        autocadExportFileVersion = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ConvertAreas":
                        convertAreas = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ConvertToLines":
                        convertToLines = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ConvertDimensions":
                        convertDimensions = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ConvertLineText":
                        convertLineText = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ConvertMultitext":
                        convertMultitext = int.Parse(reader.GetAttribute(1));
                        break;
                    case "BiarcInterpolationForSplines":
                        biarcInterpolationForSplines = int.Parse(reader.GetAttribute(1));
                        break;
                    case "BiarcInterpolationAccuracyForSplines":
                        biarcInterpolationAccuracyForSplines = decimal
                            .Parse(reader.GetAttribute(1), CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "AutocadExportFileVersion");
            writer.WriteAttributeString("value", AutocadExportFileVersion.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertAreas");
            writer.WriteAttributeString("value", ConvertAreas.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertToLines");
            writer.WriteAttributeString("value", ConvertToLines.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertDimensions");
            writer.WriteAttributeString("value", ConvertDimensions.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertLineText");
            writer.WriteAttributeString("value", ConvertLineText.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertMultitext");
            writer.WriteAttributeString("value", ConvertMultitext.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "BiarcInterpolationForSplines");
            writer.WriteAttributeString("value", BiarcInterpolationForSplines.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "BiarcInterpolationAccuracyForSplines");
            writer.WriteAttributeString("value", BiarcInterpolationAccuracyForSplines
                .ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }
        #endregion

        #region internal methods
        internal override void Export(Document document, Dictionary<Page, string> pages, Logging logging)
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
                    logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", p.Value));
                }
            }
        }
        #endregion

        #region private methods
        private void ExportTo(ExportToACAD export, Page page)
        {
            switch (AutocadExportFileVersion)
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
            if (BiarcInterpolationForSplines > 0)
            {
                export.BiarcInterpolationForSplines = true;
                export.BiarcInterpolationAccuracyForSplines = (double)BiarcInterpolationAccuracyForSplines;
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
}