using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
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
        private const double px = 3.7795275590551;
        private int extension;
        private readonly byte[] imageOptions;
        private bool screenLayers;
        private bool constructions;

        private readonly byte[] objState;
        private readonly bool[] b_values;
        private bool isChanged;
        #endregion

        public Translator_3()
        {
            extension    = 0;
            imageOptions = new byte[2];

            TargetExtension = "BMP";
            objState        = new byte[2];
            b_values        = new bool[2];
        }

        #region public properties
        /// <summary>
        /// Image options definition.
        /// </summary>
        [Browsable(false)]
        public byte[] ImageOptions
        {
            get { return (imageOptions); }
        }

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
        [ItemsSource(typeof(ExtensionItems_3))]
        public int Extension
        {
            get { return extension; }
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
        public bool ScreenLayers
        {
            get { return screenLayers; }
            set
            {
                if (screenLayers != value)
                {
                    screenLayers = value;
                    imageOptions[0] = (byte)(screenLayers ? 1 : 0);
                    OnChanged(16);
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
        public bool Constructions
        {
            get { return constructions; }
            set
            {
                if (constructions != value)
                {
                    constructions = value;
                    imageOptions[1] = (byte)(constructions ? 1 : 0);
                    OnChanged(17);
                }
            }
        }   
        #endregion

        #region internal properties
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
            b_values[0] = screenLayers;
            b_values[1] = constructions;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 16: objState[0] = (byte)(b_values[0] != screenLayers  ? 1 : 0); break;
                case 17: objState[1] = (byte)(b_values[1] != constructions ? 1 : 0); break;
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
            ExportToBitmap export    = new ExportToBitmap(document);
            ImageExport options      =
                    (screenLayers  ? ImageExport.ScreenLayers  : ImageExport.None) |
                    (constructions ? ImageExport.Constructions : ImageExport.None);
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
                    new XAttribute("name", "ImageOptions"),
                    new XAttribute("value", 
                    (imageOptions[0] == 1 ? "01" : "00") + " " + 
                    (imageOptions[1] == 1 ? "01" : "00"))));
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
                            case "BMP" : extension = 0; break;
                            case "JPEG": extension = 1; break;
                            case "GIF" : extension = 2; break;
                            case "TIFF": extension = 3; break;
                            case "PNG" : extension = 4; break;
                        }
                    }
                    else
                        value = TargetExtension;
                    break;
                case "ImageOptions":
                    string[] values = value.Split(' ');

                    if (flag == 0)
                    {
                        screenLayers  = values[0] == "01";
                        constructions = values[1] == "01";
                    }
                    else
                    {
                        values[0] = screenLayers  ? "01" : "00";
                        values[1] = constructions ? "01" : "00";

                        value = values.ToString(" ");
                    }
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

#pragma warning disable CA1812
    internal class ExtensionItems_3 : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, "BMP"  },
                { 1, "JPEG" },
                { 2, "GIF"  },
                { 3, "TIFF" },
                { 4, "PNG"  }
            };
        }
    }
}