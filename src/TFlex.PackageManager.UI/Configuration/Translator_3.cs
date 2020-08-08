using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using TFlex.Model;
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
    /// The Bitmap translator class.
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_3, 4)]
    [CustomCategoryOrder(Resources.TRANSLATOR_3, 5)]
    public class Translator_3 : Translator_0
    {
        #region private fields
        const double px = 3.7795275590551;
        int extension;
        bool screenLayers;
        bool constructions;

        XAttribute data_4_1;
        XAttribute data_4_2;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_3() { }

        #region public properties
        /// <summary>
        /// Image export formats:
        /// 0 - BMP,
        /// 1 - JPEG,
        /// 2 - GIF,
        /// 3 - TIFF,
        /// 4 - PNG
        /// </summary>
        [PropertyOrder(15)]
        [CustomCategory(Resources.TRANSLATOR_3, "category4")]
        [CustomDisplayName(Resources.TRANSLATOR_3, "dn4_1")]
        [CustomDescription(Resources.TRANSLATOR_3, "dn4_1")]
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
                        case 0: OExtension = ".bmp" ; break;
                        case 1: OExtension = ".jpeg"; break;
                        case 2: OExtension = ".gif" ; break;
                        case 3: OExtension = ".tiff"; break;
                        case 4: OExtension = ".png" ; break;
                    }
                }
            }
        }

        /// <summary>
        /// Export the screen layers of the drawing.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resources.TRANSLATOR_3, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3, "dn5_1")]
        [CustomDescription(Resources.TRANSLATOR_3, "dn5_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ScreenLayers
        {
            get => screenLayers;
            set
            {
                if (screenLayers != value)
                {
                    screenLayers = value;
                    data_4_1.Value = value ? "1" : "0";

                    OnPropertyChanged("ScreenLayers");
                }
            }
        }

        /// <summary>
        /// Export drawing elements.
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resources.TRANSLATOR_3, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3, "dn5_2")]
        [CustomDescription(Resources.TRANSLATOR_3, "dn5_2")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Constructions
        {
            get => constructions;
            set
            {
                if (constructions != value)
                {
                    constructions = value;
                    data_4_2.Value = value ? "1" : "0";

                    OnPropertyChanged("Constructions");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Bitmap;
        #endregion

        #region internal methods
        internal override void Export(Document document, Dictionary<Page, string> pages, Logging logging)
        {
            ExportToBitmap export    = new ExportToBitmap(document);
            ImageExport options      =
                    (ScreenLayers  ? ImageExport.ScreenLayers  : ImageExport.None) |
                    (Constructions ? ImageExport.Constructions : ImageExport.None);
            ImageExportFormat format = ImageExportFormat.Bmp;

            foreach (var p in pages)
            {
                switch (extension)
                {
                    case 0:
                        format = ImageExportFormat.Bmp;
                        break;
                    case 1:
                        format = ImageExportFormat.Jpeg;
                        break;
                    case 2:
                        format = ImageExportFormat.Gif;
                        break;
                    case 3:
                        format = ImageExportFormat.Tiff;
                        break;
                    case 4:
                        format = ImageExportFormat.Png;
                        break;
                }

                export.Page   = p.Key;
                export.Height = Convert.ToInt32((p.Key.Top.Value - p.Key.Bottom.Value) * px);
                export.Width  = Convert.ToInt32((p.Key.Right.Value - p.Key.Left.Value) * px);

                if (export.Export(p.Value, options, format))
                {
                    logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", p.Value));
                }
            }
        }

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_1 = new XAttribute("value", ScreenLayers ? "1" : "0");
            data_4_2 = new XAttribute("value", Constructions ? "1" : "0");

            data.Add(new XElement("parameter",
                new XAttribute("name", "ScreenLayers"),
                data_4_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "Constructions"),
                data_4_2));

            PMode = ProcessingMode.Export;
            OExtension = ".bmp";
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
                        case ".bmp" : extension = 0; break;
                        case ".jpeg": extension = 1; break;
                        case ".gif" : extension = 2; break;
                        case ".tiff": extension = 3; break;
                        case ".png" : extension = 4; break;
                    }
                    break;
                case "ScreenLayers":
                    screenLayers = a.Value == "1";
                    data_4_1 = a;
                    break;
                case "Constructions":
                    constructions = a.Value == "1";
                    data_4_2 = a;
                    break;
            }
        }
        #endregion
    }
}