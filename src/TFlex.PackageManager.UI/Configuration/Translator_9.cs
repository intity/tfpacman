using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Xml;
using TFlex.Model;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// The PDF translator class.
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_9, 5), Serializable]
    public class Translator_9 : Translator_0
    {
        #region private field
        bool export3dModel;
        bool layers;
        bool combinePages;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_9()
        {
            IExtension = ".grb";
            OExtension = ".pdf";
        }

        #region public properies
        /// <summary>
        /// Export 3D model.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resources.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_9, "dn5_1")]
        [CustomDescription(Resources.TRANSLATOR_9, "dn5_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Export3dModel
        {
            get => export3dModel;
            set
            {
                if (export3dModel != value)
                {
                    export3dModel = value;
                    OnPropertyChanged("Export3dModel");
                }
            }
        }

        /// <summary>
        /// Export layers.
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resources.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_9, "dn5_2")]
        [CustomDescription(Resources.TRANSLATOR_9, "dn5_2")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Layers
        {
            get => layers;
            set
            {
                if (layers != value)
                {
                    layers = value;
                    OnPropertyChanged("Layers");
                }
            }
        }

        /// <summary>
        /// Combine pages to one PDF file.
        /// </summary>
        [PropertyOrder(18)]
        [CustomCategory(Resources.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_9, "dn5_3")]
        [CustomDescription(Resources.TRANSLATOR_9, "dn5_3")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool CombinePages
        {
            get => combinePages;
            set
            {
                if (combinePages != value)
                {
                    combinePages = value;
                    OnPropertyChanged("CombinePages");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Pdf;
        internal override ProcessingMode PMode => ProcessingMode.Export;
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 3 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "Export3dModel":
                        export3dModel = reader.GetAttribute(1) == "1";
                        break;
                    case "Layers":
                        layers = reader.GetAttribute(1) == "1";
                        break;
                    case "CombinePages":
                        combinePages = reader.GetAttribute(1) == "1";
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Export3dModel");
            writer.WriteAttributeString("value", Export3dModel ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Layers");
            writer.WriteAttributeString("value", Layers ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "CombinePages");
            writer.WriteAttributeString("value", CombinePages ? "1" : "0");
            writer.WriteEndElement();
        }
        #endregion

        #region internals methods
        internal override void Export(Document document, Dictionary<Page, string> pages, Logging logging)
        {
            ExportToPDF export = new ExportToPDF(document)
            {
                IsSelectPagesDialogEnabled = false,
                OpenExportFile             = false,
                Export3DModel              = Export3dModel,
                Layers                     = Layers
            };

            if (CombinePages)
            {
                string path = pages.Values.First();

                if (path.Contains("_1.pdf"))
                {
                    path = path.Replace("_1.pdf", ".pdf");
                }

                export.ExportPages.AddRange(pages.Keys);

                if (export.Export(path))
                {
                    logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", path));
                }
            }
            else
            {
                foreach (var p in pages)
                {
                    export.ExportPages.Add(p.Key);

                    if (export.Export(p.Value))
                    {
                        logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", p.Value));
                    }

                    export.ExportPages.Clear();
                }
            }
        }
        #endregion
    }
}