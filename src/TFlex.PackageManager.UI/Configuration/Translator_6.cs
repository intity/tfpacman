using System.ComponentModel;
using System.Drawing.Design;
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
    /// The IGES-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 5)]
    public class Translator_6 : Translator3D
    {
        #region private fields
        bool convertAnalyticGeometryToNurbs;
        bool saveSolidBodiesAsFaceSet;

        XAttribute data_4_0;
        XAttribute data_4_1;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_6(string ext = "IGS") : base (ext)
        {
            PMode = ProcessingMode.Export; // Export | Import
        }

        #region public properties
        /// <summary>
        /// Convert Analytic Geometry to Nurgs.
        /// </summary>
        [PropertyOrder(50)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_6, "dn5_0")]
        [CustomDescription(Resource.TRANSLATOR_6, "dn5_0")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ConvertAnalyticGeometryToNurbs
        {
            get => convertAnalyticGeometryToNurbs;
            set
            {
                if (convertAnalyticGeometryToNurbs != value)
                {
                    convertAnalyticGeometryToNurbs = value;
                    data_4_0.Value = value ? "1" : "0";

                    OnPropertyChanged("ConvertAnalyticGeometryToNurbs");
                }
            }
        }

        /// <summary>
        /// Save solid bodies as face set.
        /// </summary>
        [PropertyOrder(51)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_6, "dn5_1")]
        [CustomDescription(Resource.TRANSLATOR_6, "dn5_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool SaveSolidBodiesAsFaceSet
        {
            get => saveSolidBodiesAsFaceSet;
            set
            {
                if (saveSolidBodiesAsFaceSet != value)
                {
                    saveSolidBodiesAsFaceSet = value;
                    data_4_1.Value = value ? "1" : "0";

                    OnPropertyChanged("SaveSolidBodiesAsFaceSet");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Iges;
        internal override ProcessingMode PMode
        {
            get => base.PMode;
            set => base.PMode = value;
        }
        #endregion

        #region internal methods
        internal override void Export(Document document, string path, Logging logging)
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
                ConvertAnaliticGeometryToNurbs = ConvertAnalyticGeometryToNurbs,
                SaveSolidBodiesAsFaceSet       = SaveSolidBodiesAsFaceSet
            };

            if (export.Export(path))
            {
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", path));
            }
        }

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_0 = new XAttribute("value", ConvertAnalyticGeometryToNurbs ? "1" : "0");
            data_4_1 = new XAttribute("value", SaveSolidBodiesAsFaceSet ? "1" : "0");

            data.Add(new XElement("parameter",
                new XAttribute("name", "ConvertAnalyticGeometryToNurbs"),
                data_4_0));
            data.Add(new XElement("parameter",
                new XAttribute("name", "SaveSolidBodiesAsFaceSet"),
                data_4_1));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "ConvertAnalyticGeometryToNurbs":
                    convertAnalyticGeometryToNurbs = a.Value == "1";
                    data_4_0 = a;
                    break;
                case "SaveSolidBodiesAsFaceSet":
                    saveSolidBodiesAsFaceSet = a.Value == "1";
                    data_4_1 = a;
                    break;
            }
        }
        #endregion
    }
}