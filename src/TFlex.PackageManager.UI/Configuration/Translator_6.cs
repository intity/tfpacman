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
    /// The IGES-translator class.
    /// </summary>
    [Serializable]
    public class Translator_6 : Translator3D
    {
        #region private fields
        bool convertAnalyticGeometryToNurbs;
        bool saveSolidBodiesAsFaceSet;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_6()
        {
            PMode = ProcessingMode.Export;
        }

        #region public properties
        /// <summary>
        /// Convert Analytic Geometry to Nurgs.
        /// </summary>
        [PropertyOrder(50)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_6, "dn5_0")]
        [CustomDescription(Resources.TRANSLATOR_6, "dn5_0")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ConvertAnalyticGeometryToNurbs
        {
            get => convertAnalyticGeometryToNurbs;
            set
            {
                if (convertAnalyticGeometryToNurbs != value)
                {
                    convertAnalyticGeometryToNurbs = value;
                    OnPropertyChanged("ConvertAnalyticGeometryToNurbs");
                }
            }
        }

        /// <summary>
        /// Save solid bodies as face set.
        /// </summary>
        [PropertyOrder(51)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_6, "dn5_1")]
        [CustomDescription(Resources.TRANSLATOR_6, "dn5_1")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool SaveSolidBodiesAsFaceSet
        {
            get => saveSolidBodiesAsFaceSet;
            set
            {
                if (saveSolidBodiesAsFaceSet != value)
                {
                    saveSolidBodiesAsFaceSet = value;
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
            set
            {
                base.PMode = value;
                switch (base.PMode)
                {
                    case ProcessingMode.Export:
                        IExtension = ".grb";
                        OExtension = ".igs";
                        break;
                    case ProcessingMode.Import:
                        IExtension = ".igs";
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
            for (int i = 0; i < 2 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "ConvertAnalyticGeometryToNurbs":
                        convertAnalyticGeometryToNurbs = reader.GetAttribute(1) == "1";
                        break;
                    case "SaveSolidBodiesAsFaceSet":
                        saveSolidBodiesAsFaceSet = reader.GetAttribute(1) == "1";
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ConvertAnalyticGeometryToNurbs");
            writer.WriteAttributeString("value", ConvertAnalyticGeometryToNurbs ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "SaveSolidBodiesAsFaceSet");
            writer.WriteAttributeString("value", SaveSolidBodiesAsFaceSet ? "1" : "0");
            writer.WriteEndElement();
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
        #endregion
    }
}