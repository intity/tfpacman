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
    /// The JT-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 5)]
    public class Translator_7 : Translator3D
    {
        #region private fields
        int version;

        XAttribute data_4_0;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_7(string ext = "JT") : base (ext) { }

        #region public properties
        /// <summary>
        /// JT format version.
        /// (0) - JT 8.1
        /// (1) - JT 9.5
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_7, "dn5_0")]
        [CustomDescription(Resource.TRANSLATOR_7, "dn5_0")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Version
        {
            get => version;
            set
            {
                if (version != value)
                {
                    version = value;
                    data_4_0.Value = value.ToString();

                    OnPropertyChanged("Version");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType Mode => TranslatorType.Jt;
        #endregion

        #region internal methods
        internal override void Export(Document document, string path, Logging logging)
        {
            ExportToJtVersion jtVersion = ExportToJtVersion.JT81;
            ExportTo3dMode exportMode = ExportMode == 0
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0
                ? ExportTo3dColorSource.ToneColor
                : ExportTo3dColorSource.MaterialColor;

            switch (Version)
            {
                case 0: jtVersion = ExportToJtVersion.JT81; break;
                case 1: jtVersion = ExportToJtVersion.JT95; break;
            }

            ExportToJt export = new ExportToJt(document)
            {
                Version           = jtVersion,
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
                ShowDialog        = false
            };

            if (export.Export(path))
            {
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", path));
            }
        }

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_0 = new XAttribute("value", Version.ToString());
            data.Add(new XElement("parameter",
                new XAttribute("name", "Version"),
                data_4_0));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "Version":
                    version = int.Parse(a.Value);
                    data_4_0 = a;
                    break;
            }
        }
        #endregion
    }
}