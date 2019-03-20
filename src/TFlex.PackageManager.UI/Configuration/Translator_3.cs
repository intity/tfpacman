using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
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

        private readonly byte[] objState = new byte[2];
        private readonly bool[] b_values = new bool[2];
        private bool isChanged;
        #endregion

        public Translator_3(Header header) : base(header)
        {
            extension    = 0;
            imageOptions = new byte[2];

            OutputExtension = "BMP";
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
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3, "category3")]
        [CustomDisplayName(Resource.TRANSLATOR_3, "dn3_1")]
        [CustomDescription(Resource.TRANSLATOR_3, "dn3_1")]
        [ItemsSource(typeof(ExtensionItems2))]
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
                        case 0: OutputExtension = "BMP";  break;
                        case 1: OutputExtension = "JPEG"; break;
                        case 2: OutputExtension = "GIF";  break;
                        case 3: OutputExtension = "TIFF"; break;
                        case 4: OutputExtension = "PNG";  break;
                    }
                }
            }
        }

        /// <summary>
        /// Export the screen layers of the drawing.
        /// </summary>
        [PropertyOrder(17)]
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
        [PropertyOrder(18)]
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

        #region methods
        internal override void OnLoaded()
        {
            base.OnLoaded();

            b_values[0] = screenLayers;
            b_values[1] = constructions;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 16:
                    if (b_values[0] != screenLayers)
                        objState[0] = 1;
                    else
                        objState[0] = 0;
                    break;
                case 17:
                    if (b_values[1] != constructions)
                        objState[1] = 1;
                    else
                        objState[1] = 0;
                    break;
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
            ImageExport options      = ImageExport.None;
            ImageExportFormat format = ImageExportFormat.Bmp;

            foreach (var p in pages)
            {
                options =
                    (ScreenLayers  ? ImageExport.ScreenLayers  : ImageExport.None) |
                    (Constructions ? ImageExport.Constructions : ImageExport.None);

                switch (Extension)
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

            logFile.AppendLine(string.Format("Total pages:\t{0}", pages.Count));
        }

        internal override void AppendTranslatorToXml(XElement parent, TranslatorType package)
        {
            base.AppendTranslatorToXml(parent, package);

            string value = Enum.GetName(typeof(TranslatorType), package);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter", 
                    new XAttribute("name", "OutputExtension"), 
                    new XAttribute("value", OutputExtension)),
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
                case "OutputExtension":
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
                        value = OutputExtension;
                    break;
                case "ImageOptions":
                    string[] values = value.Split(' ');

                    if (flag == 0)
                    {
                        screenLayers  = values[0] == "01" ? true : false;
                        constructions = values[1] == "01" ? true : false;
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

    internal class ExtensionItems2 : IItemsSource
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
