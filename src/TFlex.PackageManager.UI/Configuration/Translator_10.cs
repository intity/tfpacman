using System;
using System.ComponentModel;
using System.Drawing.Design;
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
    /// The STEP-translator class.
    /// </summary>
    [Serializable]
    public class Translator_10 : Translator3D
    {
        #region private fields
        int protocol;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_10()
        {
            PMode = ProcessingMode.Export;
        }

        #region public properties
        /// <summary>
        /// Protocol to be used for export to Step.
        /// (0) - AP203,
        /// (1) - AP214,
        /// (2) - AP242.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_10, "dn5_0")]
        [CustomDescription(Resources.TRANSLATOR_10, "dn5_0")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Protocol
        {
            get => protocol;
            set
            {
                if (protocol != value)
                {
                    protocol = value;
                    OnPropertyChanged("Protocol");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Step;
        internal override ProcessingMode PMode
        {
            get => base.PMode;
            set
            {
                base.PMode = value;
                switch (base.PMode)
                {
                    case ProcessingMode.Export:
                        IExtension = ".grb";
                        OExtension = ".stp";
                        break;
                    case ProcessingMode.Import:
                        IExtension = ".stp";
                        OExtension = ".grb";
                        break;
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 1 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "Protocol":
                        protocol = int.Parse(reader.GetAttribute(1));
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Protocol");
            writer.WriteAttributeString("value", Protocol.ToString());
            writer.WriteEndElement();
        }
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
        #endregion
    }
}