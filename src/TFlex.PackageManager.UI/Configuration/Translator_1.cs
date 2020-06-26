using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Acad translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_1, 4)]
    [CustomCategoryOrder(Resource.TRANSLATOR_1, 5)]
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

        XAttribute data_3_2;
        XAttribute data_4_1;
        XAttribute data_4_2;
        XAttribute data_4_3;
        XAttribute data_4_4;
        XAttribute data_4_5;
        XAttribute data_4_6;
        XAttribute data_4_7;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_1(string ext = "DWG") : base (ext)
        {
            autocadExportFileVersion             = 3;
            convertAreas                         = 0;
            convertToLines                       = 1;
            convertDimensions                    = 0;
            convertLineText                      = 0;
            convertMultitext                     = 0;
            biarcInterpolationForSplines         = 0;
            biarcInterpolationAccuracyForSplines = 0.1m;
        }

        #region public properties
        /// <summary>
        /// The AutoCAD file format:
        /// (0) - DWG,
        /// (1) - DXF,
        /// (2) - DXB
        /// </summary>
        [PropertyOrder(15)]
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Extension
        {
            get => extension;
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
        [CustomCategory(Resource.TRANSLATOR_1, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn4_2")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn4_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int AutocadExportFileVersion
        {
            get => autocadExportFileVersion;
            set
            {
                if (autocadExportFileVersion != value)
                {
                    autocadExportFileVersion = value;
                    data_3_2.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_1")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertAreas
        {
            get => convertAreas;
            set
            {
                if (convertAreas != value)
                {
                    convertAreas = value;
                    data_4_1.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_2")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertToLines
        {
            get => convertToLines;
            set
            {
                if (convertToLines != value)
                {
                    convertToLines = value;
                    data_4_2.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_3")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_3")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertDimensions
        {
            get => convertDimensions;
            set
            {
                if (convertDimensions != value)
                {
                    convertDimensions = value;
                    data_4_3.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_4")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_4")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertLineText
        {
            get => convertLineText;
            set
            {
                if (convertLineText != value)
                {
                    convertLineText = value;
                    data_4_4.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_5")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_5")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ConvertMultitext
        {
            get => convertMultitext;
            set
            {
                if (convertMultitext != value)
                {
                    convertMultitext = value;
                    data_4_5.Value = value.ToString();

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
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_6")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_6")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int BiarcInterpolationForSplines
        {
            get => biarcInterpolationForSplines;
            set
            {
                if (biarcInterpolationForSplines != value)
                {
                    biarcInterpolationForSplines = value;
                    data_4_6.Value = value.ToString();

                    OnPropertyChanged("BiarcInterpolationForSplines");
                }
            }
        }

        /// <summary>
        /// Accuracy of circular interpolation.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resource.TRANSLATOR_1, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_1, "dn5_7")]
        [CustomDescription(Resource.TRANSLATOR_1, "dn5_7")]
        [Editor(typeof(BiarcInterpolationEditor), typeof(UITypeEditor))]
        public decimal BiarcInterpolationAccuracyForSplines
        {
            get => biarcInterpolationAccuracyForSplines;
            set
            {
                if (biarcInterpolationAccuracyForSplines != value)
                {
                    biarcInterpolationAccuracyForSplines = value;
                    data_4_7.Value = value
                        .ToString(CultureInfo.InvariantCulture);

                    OnPropertyChanged("BiarcInterpolationAccuracyForSplines");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Acad;
        internal override ProcessingMode PMode => ProcessingMode.Export;
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

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_3_2 = new XAttribute("value", AutocadExportFileVersion.ToString());
            data_4_1 = new XAttribute("value", ConvertAreas.ToString());
            data_4_2 = new XAttribute("value", ConvertToLines.ToString());
            data_4_3 = new XAttribute("value", ConvertDimensions.ToString());
            data_4_4 = new XAttribute("value", ConvertLineText.ToString());
            data_4_5 = new XAttribute("value", ConvertMultitext.ToString());
            data_4_6 = new XAttribute("value", BiarcInterpolationForSplines.ToString());
            data_4_7 = new XAttribute("value", BiarcInterpolationAccuracyForSplines
                .ToString(CultureInfo.InvariantCulture));

            data.Add(new XElement("parameter",
                new XAttribute("name", "AutocadExportFileVersion"),
                data_3_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertAreas"),
                data_4_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertToLines"),
                data_4_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertDimensions"),
                data_4_3));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertLineText"),
                data_4_4));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertMultitext"),
                data_4_5));
            data.Add(new XElement("parameter",
                new XAttribute("name", "BiarcInterpolationForSplines"),
                data_4_6));
            data.Add(new XElement("parameter",
                new XAttribute("name", "BiarcInterpolationAccuracyForSplines"),
                data_4_7));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "TargetExtension":
                    switch (a.Value)
                    {
                        case "DWG": extension = 0; break;
                        case "DXF": extension = 1; break;
                        case "DXB": extension = 2; break;
                    }
                    break;
                case "AutocadExportFileVersion":
                    autocadExportFileVersion = int.Parse(a.Value);
                    data_3_2 = a;
                    break;
                case "ConvertAreas":
                    convertAreas = int.Parse(a.Value);
                    data_4_1 = a;
                    break;
                case "ConvertToLines":
                    convertToLines = int.Parse(a.Value);
                    data_4_2 = a;
                    break;
                case "ConvertDimensions":
                    convertDimensions = int.Parse(a.Value);
                    data_4_3 = a;
                    break;
                case "ConvertLineText":
                    convertLineText = int.Parse(a.Value);
                    data_4_4 = a;
                    break;
                case "ConvertMultitext":
                    convertMultitext = int.Parse(a.Value);
                    data_4_5 = a;
                    break;
                case "BiarcInterpolationForSplines":
                    biarcInterpolationForSplines = int.Parse(a.Value);
                    data_4_6 = a;
                    break;
                case "BiarcInterpolationAccuracyForSplines":
                    biarcInterpolationAccuracyForSplines = decimal
                        .Parse(a.Value, NumberStyles.Float, 
                        CultureInfo.InvariantCulture);
                    data_4_7 = a;
                    break;
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