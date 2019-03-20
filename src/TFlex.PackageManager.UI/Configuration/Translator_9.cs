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
    [CustomCategoryOrder(Resource.TRANSLATOR_9, 4)]
    public class Translator_9 : Translator_0
    {
        #region private field
        private bool export3dModel;
        private bool layers;
        private bool combinePages;

        private readonly byte[] objState = new byte[3];
        private readonly bool[] b_values = new bool[3];
        private bool isChanged;
        #endregion

        public Translator_9(Header header) : base (header)
        {
            OutputExtension = "PDF";
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

        #region public properies
        /// <summary>
        /// Export 3D model.
        /// </summary>
        [PropertyOrder(18)]
        [CustomCategory(Resource.TRANSLATOR_9, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn4_1")]
        public bool Export3dModel
        {
            get { return export3dModel; }
            set
            {
                if (export3dModel != value)
                {
                    export3dModel = value;
                    OnChanged(15);
                }
            }
        }

        /// <summary>
        /// Export layers.
        /// </summary>
        [PropertyOrder(19)]
        [CustomCategory(Resource.TRANSLATOR_9, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn4_2")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn4_2")]
        public bool Layers
        {
            get { return layers; }
            set
            {
                if (layers != value)
                {
                    layers = value;
                    OnChanged(16);
                }
            }
        }

        /// <summary>
        /// Combine pages to one PDF file.
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resource.TRANSLATOR_9, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_9, "dn4_3")]
        [CustomDescription(Resource.TRANSLATOR_9, "dn4_3")]
        public bool CombinePages
        {
            get { return combinePages; }
            set
            {
                if (combinePages != value)
                {
                    combinePages = value;
                    OnChanged(17);
                }
            }
        }
        #endregion

        #region methods
        internal override void OnLoaded()
        {
            base.OnLoaded();

            b_values[0] = export3dModel;
            b_values[1] = layers;
            b_values[2] = combinePages;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 15:
                    if (b_values[0] != export3dModel)
                        objState[0] = 1;
                    else
                        objState[0] = 0;
                    break;
                case 16:
                    if (b_values[1] != layers)
                        objState[1] = 1;
                    else
                        objState[1] = 0;
                    break;
                case 17:
                    if (b_values[2] != combinePages)
                        objState[2] = 1;
                    else
                        objState[2] = 0;
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
            ExportToPDF export = new ExportToPDF(document)
            {
                IsSelectPagesDialogEnabled = false,
                OpenExportFile             = false,
                Export3DModel              = export3dModel,
                Layers                     = layers
            };

            if (combinePages)
            {
                string path = pages.Values.First();

                if (path.Contains("_1.pdf"))
                {
                    path = path.Replace("_1.pdf", ".pdf");
                }

                export.ExportPages.AddRange(pages.Keys);

                if (export.Export(path))
                {
                    logFile.AppendLine(string.Format("Export to:\t{0}", path));
                }
            }
            else
            {
                foreach (var p in pages)
                {
                    export.ExportPages.Add(p.Key);

                    if (export.Export(p.Value))
                    {
                        logFile.AppendLine(string.Format("Export to:\t{0}", p.Value));
                    }

                    export.ExportPages.Clear();
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
                    new XAttribute("name", "Export3dModel"),
                    new XAttribute("value", export3dModel ? "1" : "0")),
                new XElement("parameter", 
                    new XAttribute("name", "Layers"), 
                    new XAttribute("value", layers ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "CombinePages"),
                    new XAttribute("value", combinePages ? "1" : "0")));
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "Export3dModel":
                    if (flag == 0)
                        export3dModel = value == "1" ? true : false;
                    else
                        value = export3dModel ? "1" : "0";
                    break;
                case "Layers":
                    if (flag == 0)
                        layers = value == "1" ? true : false;
                    else
                        value = layers ? "1" : "0";
                    break;
                case "CombinePages":
                    if (flag == 0)
                        combinePages = value == "1" ? true : false;
                    else
                        value = combinePages ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }
}