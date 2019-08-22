using TFlex.Configuration.Attributes;
using TFlex.Model;
using TFlex.PackageManager.Common;

#pragma warning disable CA1707

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The ACIS-translator class.
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    public class Translator_2 : Translator3D
    {
        public Translator_2()
        {
            TargetExtension = "SAT"; // SAT | SAB
        }

        #region internal methods
        internal override void Export(Document document, string path, LogFile logFile)
        {
            ExportTo3dMode exportMode = ExportMode == 0
                ? ExportTo3dMode.Assembly
                : ExportTo3dMode.BodySet;

            ExportTo3dColorSource colorSource = ColorSource == 0
                ? ExportTo3dColorSource.ToneColor
                : ExportTo3dColorSource.MaterialColor;

            ExportToAcis export = new ExportToAcis(document)
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
                ShowDialog        = false
            };

            if (export.Export(path))
            {
                logFile.AppendLine(string.Format("Export to:\t\t{0}", path));
            }
        }
        #endregion
    }
}
