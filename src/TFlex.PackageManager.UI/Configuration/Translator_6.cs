using System;
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
    /// The IGES-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    public class Translator_6 : Translator3D
    {
        #region private fields
        private bool convertAnalyticGeometryToNurbs;
        private bool saveSolidBodiesAsFaceSet;

        private readonly byte[] objState;
        private readonly bool[] b_values;
        private bool isChanged;
        #endregion

        public Translator_6()
        {
            TargetExtension = "IGS";

            objState        = new byte[2];
            b_values        = new bool[2];
        }

        #region public properties
        /// <summary>
        /// Convert Analytic Geometry to Nurgs.
        /// </summary>
        [PropertyOrder(50)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_6, "dn4_0")]
        [CustomDescription(Resource.TRANSLATOR_6, "dn4_0")]
        public bool ConvertAnalyticGeometryToNurbs
        {
            get { return convertAnalyticGeometryToNurbs; }
            set
            {
                if (convertAnalyticGeometryToNurbs != value)
                {
                    convertAnalyticGeometryToNurbs = value;
                    OnChanged(50);
                }
            }
        }

        /// <summary>
        /// Save solid bodies as face set.
        /// </summary>
        [PropertyOrder(51)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_6, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_6, "dn4_1")]
        public bool SaveSolidBodiesAsFaceSet
        {
            get { return saveSolidBodiesAsFaceSet; }
            set
            {
                if (saveSolidBodiesAsFaceSet != value)
                {
                    saveSolidBodiesAsFaceSet = value;
                    OnChanged(51);
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
            b_values[0] = convertAnalyticGeometryToNurbs;
            b_values[1] = saveSolidBodiesAsFaceSet;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 50: objState[0] = (byte)(b_values[0] != convertAnalyticGeometryToNurbs ? 1 : 0); break;
                case 51: objState[1] = (byte)(b_values[1] != saveSolidBodiesAsFaceSet       ? 1 : 0); break;
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

        internal override void Export(Document document, string path, LogFile logFile)
        {
            ExportTo3dMode exportMode = ExportMode == 0
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0
                ? ExportTo3dColorSource.ToneColor
                : ExportTo3dColorSource.MaterialColor;

            ExportToIges export = new ExportToIges(document)
            {
                Mode              = exportMode,
                ColorSource       = colorSource,
                Export3DPictures  = Export3DPictures,
                ExportAnotation   = ExportAnotation,
                ExportContours    = ExportContours,
                ExportCurves      = ExportCurves,
                ExportSheetBodies = ExportSheetBodies,
                ExportSolidBodies = ExportSolidBodies,
                ExportWelds       = ExportWelds,
                ExportWireBodies  = ExportWireBodies,
                SimplifyGeometry  = SimplifyGeometry,
                ShowDialog        = false,
                ConvertAnaliticGeometryToNurbs = convertAnalyticGeometryToNurbs,
                SaveSolidBodiesAsFaceSet       = saveSolidBodiesAsFaceSet
            };

            if (export.Export(path))
            {
                logFile.AppendLine(string.Format("Export to:\t\t{0}", path));
            }
        }

        internal override void AppendTranslatorToXml(XElement parent, TranslatorType translator)
        {
            base.AppendTranslatorToXml(parent, translator);

            string value = Enum.GetName(typeof(TranslatorType), translator);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter",
                    new XAttribute("name", "ConvertAnalyticGeometryToNurbs"),
                    new XAttribute("value", ConvertAnalyticGeometryToNurbs ? "1" : "0")),
                new XElement("parameter", 
                    new XAttribute("name", "SaveSolidBodiesAsFaceSet"), 
                    new XAttribute("value", SaveSolidBodiesAsFaceSet ? "1" : "0")));
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "ConvertAnalyticGeometryToNurbs":
                    if (flag == 0)
                        convertAnalyticGeometryToNurbs = value == "1";
                    else
                        value = convertAnalyticGeometryToNurbs ? "1" : "0";
                    break;
                case "SaveSolidBodiesAsFaceSet":
                    if (flag == 0)
                        saveSolidBodiesAsFaceSet = value == "1";
                    else
                        value = saveSolidBodiesAsFaceSet ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }
}