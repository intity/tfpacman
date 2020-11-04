﻿using System;
using System.Diagnostics;
using TFlex.Model;
using TFlex.PackageManager.UI.Common;

#pragma warning disable CA1707

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// The ACIS-translator class.
    /// </summary>
    [Serializable]
    public class Translator_2 : Translator3D
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_2()
        {
            PMode = ProcessingMode.Export;
        }

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Acis;
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
                        OExtension = ".sat";
                        break;
                    case ProcessingMode.Import:
                        IExtension = ".sat";
                        OExtension = ".grb";
                        break;
                }
            }
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
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Export to [path: {0}]", path));
            }
        }
        #endregion
    }
}
