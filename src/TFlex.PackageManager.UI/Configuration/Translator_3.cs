using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707
#pragma warning disable CA1819

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Bitmap translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3, 3)]
    [CustomCategoryOrder(Resource.TRANSLATOR_3, 4)]
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
        /// <param name="ext">Target extension the file.</param>
        public Translator_3(string ext = "BMP") : base (ext) { }

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
        [CustomCategory(Resource.TRANSLATOR_3, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_3, "dn3_1")]
        [CustomDescription(Resource.TRANSLATOR_3, "dn3_1")]
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
                        case 0: TargetExtension = "BMP";  break;
                        case 1: TargetExtension = "JPEG"; break;
                        case 2: TargetExtension = "GIF";  break;
                        case 3: TargetExtension = "TIFF"; break;
                        case 4: TargetExtension = "PNG";  break;
                    }
                }
            }
        }

        /// <summary>
        /// Export the screen layers of the drawing.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_3, "dn4_1")]
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
        [CustomCategory(Resource.TRANSLATOR_3, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3, "dn4_2")]
        [CustomDescription(Resource.TRANSLATOR_3, "dn4_2")]
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
        internal override TranslatorType Mode => TranslatorType.Bitmap;

        internal override uint Processing
        {
            get => (uint)ProcessingMode.Export;
        }
        #endregion

        #region internal methods
        internal override void Export(Document document, Dictionary<Page, string> pages, LogFile logFile)
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
                    logFile.AppendLine(string.Format("Export to:\t\t{0}", p.Value));
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
                        case "BMP" : extension = 0; break;
                        case "JPEG": extension = 1; break;
                        case "GIF" : extension = 2; break;
                        case "TIFF": extension = 3; break;
                        case "PNG" : extension = 4; break;
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