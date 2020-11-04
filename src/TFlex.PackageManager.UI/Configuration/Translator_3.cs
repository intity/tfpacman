using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using TFlex.Model;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707
#pragma warning disable CA1819

namespace TFlex.PackageManager.UI.Configuration
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
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_3()
        {
            IExtension = ".grb";
            OExtension = ".bmp";
        }

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
                    OnPropertyChanged("Constructions");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Bitmap;
        internal override ProcessingMode PMode => ProcessingMode.Export;
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 2 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "ScreenLayers":
                        screenLayers = reader.GetAttribute(1) == "1";
                        break;
                    case "Constructions":
                        constructions = reader.GetAttribute(1) == "1";
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ScreenLayers");
            writer.WriteAttributeString("value", ScreenLayers ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Constructions");
            writer.WriteAttributeString("value", Constructions ? "1" : "0");
            writer.WriteEndElement();
        }
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
        #endregion
    }
}