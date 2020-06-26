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
    /// The STEP-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 5)]
    public class Translator_10 : Translator3D
    {
        #region private fields
        int protocol;

        XAttribute data_4_0;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_10(string ext = "STP") : base (ext) { }

        #region public properties
        /// <summary>
        /// Protocol to be used for export to Step.
        /// (0) - AP203,
        /// (1) - AP214,
        /// (2) - AP242.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_10, "dn5_0")]
        [CustomDescription(Resource.TRANSLATOR_10, "dn5_0")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Protocol
        {
            get => protocol;
            set
            {
                if (protocol != value)
                {
                    protocol = value;
                    data_4_0.Value = value.ToString();

                    OnPropertyChanged("Protocol");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Step;
        #endregion

        #region internal methods
        internal override void Export(Document document, string path, Logging logging)
        {
            ExportToStepProtocol stepProtocol = ExportToStepProtocol.AP203;
            ExportTo3dMode exportMode = ExportMode == 0 
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0 
                ? ExportTo3dColorSource.ToneColor 
                : ExportTo3dColorSource.MaterialColor;

            switch (Protocol)
            {
                case 0: stepProtocol = ExportToStepProtocol.AP203; break;
                case 1: stepProtocol = ExportToStepProtocol.AP214; break;
                case 2: stepProtocol = ExportToStepProtocol.AP242; break;
            }

            ExportToStep export = new ExportToStep(document)
            {
                Protocol          = stepProtocol,
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

            data_4_0 = new XAttribute("value", Protocol.ToString());
            data.Add(new XElement("parameter",
                new XAttribute("name", "Protocol"),
                data_4_0));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "Protocol":
                    protocol = int.Parse(a.Value);
                    data_4_0 = a;
                    break;
            }
        }
        #endregion
    }
}