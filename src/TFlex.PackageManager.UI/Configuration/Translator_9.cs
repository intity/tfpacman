using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The PDF translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_9, 4)]
    public class Translator_9 : Translator_0
    {
        #region private field
        private bool export3dModel;
        private bool layers;
        private bool combinePages;

        private readonly byte[] objState;
        private readonly bool[] b_values;
        private bool isChanged;
        #endregion

        public Translator_9()
        {
            TargetExtension = "PDF";

            objState        = new byte[3];
            b_values        = new bool[3];
        }

        #region public properies
        /// <summary>
        /// Export 3D model.
        /// </summary>
        [PropertyOrder(16)]
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
                    OnChanged(16);
                }
            }
        }

        /// <summary>
        /// Export layers.
        /// </summary>
        [PropertyOrder(17)]
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
                    OnChanged(17);
                }
            }
        }

        /// <summary>
        /// Combine pages to one PDF file.
        /// </summary>
        [PropertyOrder(18)]
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
                    OnChanged(18);
                }
            }
        }
        #endregion

        #region internal properties
        internal override uint Processing
        {
            get { return (uint)ProcessingType.Export; }
        }

        internal override bool IsChanged
        {
            get
            {
                return (isChanged | base.IsChanged);
            }
        }
        #endregion

        #region internals methods
        internal override void OnLoaded()
        {
            b_values[0] = export3dModel;
            b_values[1] = layers;
            b_values[2] = combinePages;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 16: objState[0] = (byte)(b_values[0] != export3dModel ? 1 : 0); break;
                case 17: objState[1] = (byte)(b_values[1] != layers        ? 1 : 0); break;
                case 18: objState[2] = (byte)(b_values[2] != combinePages  ? 1 : 0); break;
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
                    logFile.AppendLine(string.Format("Export to:\t\t{0}", path));
                }
            }
            else
            {
                foreach (var p in pages)
                {
                    export.ExportPages.Add(p.Key);

                    if (export.Export(p.Value))
                    {
                        logFile.AppendLine(string.Format("Export to:\t\t{0}", p.Value));
                    }

                    export.ExportPages.Clear();
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