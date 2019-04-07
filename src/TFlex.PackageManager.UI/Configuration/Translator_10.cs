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
    /// The STEP-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    public class Translator_10 : Translator3D
    {
        #region private fields
        private int protocol;
        private readonly int[] i_values;
        private bool isChanged;
        #endregion

        public Translator_10()
        {
            protocol        = 1;
            OutputExtension = "STP";
            i_values        = new int[1];
        }

        #region internal properties
        internal override bool IsChanged
        {
            get
            {
                return (isChanged | base.IsChanged);
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Protocol to be used for export to Step.
        /// (0) - AP203,
        /// (1) - AP214,
        /// (2) - AP242.
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_10, "dn4_0")]
        [CustomDescription(Resource.TRANSLATOR_10, "dn4_0")]
        [ItemsSource(typeof(ProtocolItems))]
        public int Protocol
        {
            get { return protocol; }
            set
            {
                if (protocol != value)
                {
                    protocol = value;
                    OnChanged(16);
                }
            }
        }
        #endregion

        #region internal methods
        internal override void OnLoaded()
        {
            i_values[0] = protocol;
            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            isChanged = i_values[0] != protocol;
            base.OnChanged(index);
        }

        internal override void Export(Document document, string path, LogFile logFile)
        {
            ExportToStepProtocol stepProtocol = ExportToStepProtocol.AP203;
            ExportTo3dMode exportMode = Mode == 0 
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0 
                ? ExportTo3dColorSource.ToneColor 
                : ExportTo3dColorSource.MaterialColor;

            switch (protocol)
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
                logFile.AppendLine(string.Format("Export to:\t{0}", path));
            }
        }

        internal override XElement NewTranslator(TranslatorType translator)
        {
            XElement element = new XElement("translator", new XAttribute("id", translator),
                new XElement("parameter",
                    new XAttribute("name", "FileNameSuffix"),
                    new XAttribute("value", FileNameSuffix)),
                new XElement("parameter",
                    new XAttribute("name", "TemplateFileName"),
                    new XAttribute("value", TemplateFileName)),
                new XElement("parameter",
                    new XAttribute("name", "OutputExtension"),
                    new XAttribute("value", OutputExtension)),
                new XElement("parameter",
                    new XAttribute("name", "Protocol"),
                    new XAttribute("value", Protocol)),
                new XElement("parameter",
                    new XAttribute("name", "Mode"),
                    new XAttribute("value", Mode)),
                new XElement("parameter",
                    new XAttribute("name", "ColorSource"),
                    new XAttribute("value", ColorSource)),
                new XElement("parameter",
                    new XAttribute("name", "Export3DPictures"),
                    new XAttribute("value", Export3DPictures ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportAnotation"),
                    new XAttribute("value", ExportAnotation ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportContours"),
                    new XAttribute("value", ExportContours ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportCurves"),
                    new XAttribute("value", ExportCurves ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportSheetBodies"),
                    new XAttribute("value", ExportSheetBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportSolidBodies"),
                    new XAttribute("value", ExportSolidBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportWelds"),
                    new XAttribute("value", ExportWelds ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportWireBodies"),
                    new XAttribute("value", ExportWireBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "SimplifyGeometry"),
                    new XAttribute("value", SimplifyGeometry ? "1" : "0")));

            return element;
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "Protocol":
                    if (flag == 0)
                        protocol = int.Parse(value);
                    else
                        value = protocol.ToString();
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

#pragma warning disable CA1812
    internal class ProtocolItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, "AP203" },
                { 1, "AP214" },
                { 2, "AP242" }
            };
        }
    }
}