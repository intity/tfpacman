using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The JT-translator class.
    /// </summary>
    [Serializable]
    public class Translator_7 : Translator3D
    {
        #region private fields
        int version;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_7()
        {
            PMode = ProcessingMode.Export;
        }

        #region public properties
        /// <summary>
        /// JT format version.
        /// (0) - JT 8.1
        /// (1) - JT 9.5
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_7, "dn5_0")]
        [CustomDescription(Resources.TRANSLATOR_7, "dn5_0")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Version
        {
            get => version;
            set
            {
                if (version != value)
                {
                    version = value;
                    OnPropertyChanged("Version");
                }
            }
        }
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Jt;
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
                        OExtension = ".jt";
                        break;
                    case ProcessingMode.Import:
                        IExtension = ".jt";
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
                    case "Version":
                        version = int.Parse(reader.GetAttribute(1));
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Version");
            writer.WriteAttributeString("value", Version.ToString());
            writer.WriteEndElement();
        }
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
        #endregion
    }
}