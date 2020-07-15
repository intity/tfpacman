using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The PDF translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_9, 5)]
    public class Translator_9 : Translator_0
    {
        #region private field
        bool export3dModel;
        bool layers;
        bool combinePages;

        XAttribute data_4_1;
        XAttribute data_4_2;
        XAttribute data_4_3;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_9(string ext = "PDF") : base (ext)
        {
            
        }

        #region public properies
        /// <summary>
        /// Export 3D model.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn5_1")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn5_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Export3dModel
        {
            get => export3dModel;
            set
            {
                if (export3dModel != value)
                {
                    export3dModel = value;
                    data_4_1.Value = value ? "1" : "0";

                    OnPropertyChanged("Export3dModel");
                }
            }
        }

        /// <summary>
        /// Export layers.
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resource.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn5_2")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn5_2")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Layers
        {
            get => layers;
            set
            {
                if (layers != value)
                {
                    layers = value;
                    data_4_2.Value = value ? "1" : "0";

                    OnPropertyChanged("Layers");
                }
            }
        }

        /// <summary>
        /// Combine pages to one PDF file.
        /// </summary>
        [PropertyOrder(18)]
        [CustomCategory(Resource.TRANSLATOR_9, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn5_3")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn5_3")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool CombinePages
        {
            get => combinePages;
            set
            {
                if (combinePages != value)
                {
                    combinePages = value;
                    data_4_3.Value = value ? "1" : "0";

                    OnPropertyChanged("CombinePages");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Pdf;
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

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_1 = new XAttribute("value", Export3dModel ? "1" : "0");
            data_4_2 = new XAttribute("value", Layers        ? "1" : "0");
            data_4_3 = new XAttribute("value", CombinePages  ? "1" : "0");

            data.Add(new XElement("parameter",
                new XAttribute("name", "Export3dModel"),
                data_4_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "Layers"),
                data_4_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "CombinePages"),
                data_4_3));

            PMode = ProcessingMode.Export;
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "Export3dModel":
                    export3dModel = a.Value == "1";
                    data_4_1 = a;
                    break;
                case "Layers":
                    layers = a.Value == "1";
                    data_4_2 = a;
                    break;
                case "CombinePages":
                    combinePages = a.Value == "1";
                    data_4_3 = a;
                    break;
            }
        }
        #endregion
    }
}