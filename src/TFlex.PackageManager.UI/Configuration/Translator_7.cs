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
    /// The JT-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    public class Translator_7 : Translator3D
    {
        #region private fields
        private int version;

        private readonly int[] i_values;
        private bool isChanged;
        #endregion

        public Translator_7()
        {
            TargetExtension = "JT";
            i_values        = new int[1];
        }

        #region public properties
        /// <summary>
        /// JT format version.
        /// (0) - JT 8.1
        /// (1) - JT 9.5
        /// </summary>
        [PropertyOrder(16)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_7, "dn4_0")]
        [CustomDescription(Resource.TRANSLATOR_7, "dn4_0")]
        [ItemsSource(typeof(JtVersions))]
        public int Version
        {
            get { return version; }
            set
            {
                if (version != value)
                {
                    version = value;
                    OnChanged(16);
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
            i_values[0] = version;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 16: isChanged = i_values[0] != version; break;
            }

            base.OnChanged(index);
        }

        internal override void Export(Document document, string path, LogFile logFile)
        {
            ExportToJtVersion jtVersion = ExportToJtVersion.JT81;
            ExportTo3dMode exportMode = ExportMode == 0
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0
                ? ExportTo3dColorSource.ToneColor
                : ExportTo3dColorSource.MaterialColor;

            switch (version)
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
                logFile.AppendLine(string.Format("Export to:\t\t{0}", path));
            }
        }

        internal override void AppendTranslatorToXml(XElement parent, TranslatorType translator)
        {
            base.AppendTranslatorToXml(parent, translator);

            string value = Enum.GetName(typeof(TranslatorType), translator);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter",
                    new XAttribute("name", "Version"),
                    new XAttribute("value", Version)));
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "Version":
                    if (flag == 0)
                        version = int.Parse(value);
                    else
                        value = version.ToString();
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

#pragma warning disable CA1812
    internal class JtVersions : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, "JT 8.1" },
                { 1, "JT 9.5" }
            };
        }
    }
}
