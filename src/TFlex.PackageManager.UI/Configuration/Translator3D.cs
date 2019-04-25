using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Base class for import/export to some universal 3D formats (STEP, ACIS, JT, PRC, 3MF, IGES).
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 5)]
    public class Translator3D : Category_3
    {
        #region private fields
        private int exportMode;
        private int colorSource;
        private bool export3DPictures;
        private bool exportAnotation;
        private bool exportContours;
        private bool exportCurves;
        private bool exportSheetBodies;
        private bool exportSolidBodies;
        private bool exportWelds;
        private bool exportWireBodies;
        private bool simplifyGeometry;

        private int importMode;
        private int heal;
        private int createAccurateEdges;
        private bool importAnotations;
        private bool importSolidBodies;
        private bool importSheetBodies;
        private bool importMeshBodies;
        private bool importWireBodies;
        private bool importHideBodies;
        private bool importOnlyFromActiveFilter;
        private readonly Sewing sewing;
        private bool checkImportGeomerty;
        private bool updateProductStructure;
        private bool addBodyRecordsInProductStructure;

        private readonly byte[] objState;
        private readonly int[] i_values;
        private readonly bool[] b_values;
        private readonly double[] d_values;
        private bool isChanged;
        #endregion

        public Translator3D()
        {
            colorSource                = 1;
            exportSolidBodies          = true;
            exportSheetBodies          = true;
            exportWireBodies           = true;

            importMode                 = 1;
            createAccurateEdges        = 2;
            importSolidBodies          = true;
            importSheetBodies          = true;
            importMeshBodies           = true;
            importWireBodies           = true;
            importOnlyFromActiveFilter = true;
            sewing                     = new Sewing();
            sewing.PropertyChanged    += Sewing_PropertyChanged;
            checkImportGeomerty        = true;

            objState = new byte[25];
            i_values = new int[6];
            b_values = new bool[20];
            d_values = new double[1];
        }

        private void Sewing_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChanged(41);
        }

        #region public properties
        /// <summary>
        /// Export type.
        /// (0) - Assembly,
        /// (1) - BodySet
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_1")]
        [ItemsSource(typeof(ExportModeItems))]
        public int ExportMode
        {
            get { return exportMode; }
            set
            {
                if (exportMode != value)
                {
                    exportMode = value;
                    OnChanged(20);
                }
            }
        }

        /// <summary>
        /// Source for color:
        /// (0) - ToneColor,
        /// (1) - MaterialColor
        /// </summary>
        [PropertyOrder(21)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_2")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_2")]
        [ItemsSource(typeof(ColorSourceItems))]
        public int ColorSource
        {
            get { return colorSource; }
            set
            {
                if (colorSource != value)
                {
                    colorSource = value;
                    OnChanged(21);
                }
            }
        }

        /// <summary>
        /// Export solid bodies.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_3")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_3")]
        public bool ExportSolidBodies
        {
            get { return exportSolidBodies; }
            set
            {
                if (exportSolidBodies != value)
                {
                    exportSolidBodies = value;
                    OnChanged(22);
                }
            }
        }

        /// <summary>
        /// Export sheet bodies.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_4")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_4")]
        public bool ExportSheetBodies
        {
            get { return exportSheetBodies; }
            set
            {
                if (exportSheetBodies != value)
                {
                    exportSheetBodies = value;
                    OnChanged(23);
                }
            }
        }

        /// <summary>
        /// Export wire bodies.
        /// </summary>
        [PropertyOrder(24)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_5")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_5")]
        public bool ExportWireBodies
        {
            get { return exportWireBodies; }
            set
            {
                if (exportWireBodies != value)
                {
                    exportWireBodies = value;
                    OnChanged(24);
                }
            }
        }

        /// <summary>
        /// Export 3D images.
        /// </summary>
        [PropertyOrder(25)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_6")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_6")]
        public bool Export3DPictures
        {
            get { return export3DPictures; }
            set
            {
                if (export3DPictures != value)
                {
                    export3DPictures = value;
                    OnChanged(25);
                }
            }
        }

        /// <summary>
        /// Export annotations.
        /// </summary>
        [PropertyOrder(26)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_7")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_7")]
        public bool ExportAnotation
        {
            get { return exportAnotation; }
            set
            {
                if (exportAnotation != value)
                {
                    exportAnotation = value;
                    OnChanged(26);
                }
            }
        }

        /// <summary>
        /// Export welds.
        /// </summary>
        [PropertyOrder(27)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_8")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_8")]
        public bool ExportWelds
        {
            get { return exportWelds; }
            set
            {
                if (exportWelds != value)
                {
                    exportWelds = value;
                    OnChanged(27);
                }
            }
        }

        /// <summary>
        /// Export curves.
        /// </summary>
        [PropertyOrder(28)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_9")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_9")]
        public bool ExportCurves
        {
            get { return exportCurves; }
            set
            {
                if (exportCurves != value)
                {
                    exportCurves = value;
                    OnChanged(28);
                }
            }
        }

        /// <summary>
        /// Export profiles.
        /// </summary>
        [PropertyOrder(29)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_10")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_10")]
        public bool ExportContours
        {
            get { return exportContours; }
            set
            {
                if (exportContours != value)
                {
                    exportContours = value;
                    OnChanged(29);
                }
            }
        }

        /// <summary>
        /// Simplify geometry.
        /// </summary>
        [PropertyOrder(30)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_11")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_11")]
        public bool SimplifyGeometry
        {
            get { return simplifyGeometry; }
            set
            {
                if (simplifyGeometry != value)
                {
                    simplifyGeometry = value;
                    OnChanged(30);
                }
            }
        }

        /// <summary>
        /// Import type: 
        /// (0) - Assembly,
        /// (1) - BodySet,
        /// (2) - Operation
        /// </summary>
        [PropertyOrder(31)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_1")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_1")]
        [ItemsSource(typeof(ImportModeItems))]
        public int ImportMode
        {
            get { return importMode; }
            set
            {
                if (importMode != value)
                {
                    importMode = value;
                    OnChanged(31);
                }
            }
        }

        /// <summary>
        /// Geometry heal:
        /// (0) - Auto,
        /// (1) - Yes,
        /// (2) - No
        /// </summary>
        [PropertyOrder(32)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_2")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_2")]
        [ItemsSource(typeof(ImportFrom3dOptionItems))]
        public int Heal
        {
            get { return heal; }
            set
            {
                if (heal != value)
                {
                    heal = value;
                    OnChanged(32);
                }
            }
        }

        /// <summary>
        /// Create accurate edges:
        /// (0) - Auto,
        /// (1) - Yes,
        /// (2) - No
        /// </summary>
        [PropertyOrder(33)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_3")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_3")]
        [ItemsSource(typeof(ImportFrom3dOptionItems))]
        public int CreateAccurateEdges
        {
            get { return createAccurateEdges; }
            set
            {
                if (createAccurateEdges != value)
                {
                    createAccurateEdges = value;
                    OnChanged(33);
                }
            }
        }

        /// <summary>
        /// Import solid bodies.
        /// </summary>
        [PropertyOrder(34)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_4")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_4")]
        public bool ImportSolidBodies
        {
            get { return importSolidBodies; }
            set
            {
                if (importSolidBodies != value)
                {
                    importSolidBodies = value;
                    OnChanged(34);
                }
            }
        }

        /// <summary>
        /// Import sheet bodies.
        /// </summary>
        [PropertyOrder(35)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_5")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_5")]
        public bool ImportSheetBodies
        {
            get { return importSheetBodies; }
            set
            {
                if (importSheetBodies != value)
                {
                    importSheetBodies = value;
                    OnChanged(35);
                }
            }
        }

        /// <summary>
        /// Import wire bodies.
        /// </summary>
        [PropertyOrder(36)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_6")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_6")]
        public bool ImportWireBodies
        {
            get { return importWireBodies; }
            set
            {
                if (importWireBodies != value)
                {
                    importWireBodies = value;
                    OnChanged(36);
                }
            }
        }

        /// <summary>
        /// Import mesh bodies as 3D images.
        /// </summary>
        [PropertyOrder(37)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_7")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_7")]
        public bool ImportMeshBodies
        {
            get { return importMeshBodies; }
            set
            {
                if (importMeshBodies != value)
                {
                    importMeshBodies = value;
                    OnChanged(37);
                }
            }
        }

        /// <summary>
        /// Import hidden bodies.
        /// </summary>
        [PropertyOrder(38)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_8")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_8")]
        public bool ImportHideBodies
        {
            get { return importHideBodies; }
            set
            {
                if (importHideBodies != value)
                {
                    importHideBodies = value;
                    OnChanged(38);
                }
            }
        }

        /// <summary>
        /// Import annotations.
        /// </summary>
        [PropertyOrder(39)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_9")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_9")]
        public bool ImportAnotations
        {
            get { return importAnotations; }
            set
            {
                if (importAnotations != value)
                {
                    importAnotations = value;
                    OnChanged(39);
                }
            }
        }

        /// <summary>
        /// Import bodies from active layer only.
        /// </summary>
        [PropertyOrder(40)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_10")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_10")]
        public bool ImportOnlyFromActiveFilter
        {
            get { return importOnlyFromActiveFilter; }
            set
            {
                if (importOnlyFromActiveFilter != value)
                {
                    importOnlyFromActiveFilter = value;
                    OnChanged(40);
                }
            }
        }

        /// <summary>
        /// Sewing.
        /// </summary>
        [PropertyOrder(41)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_11")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_11")]
        [Editor(typeof(InputSewingControl), typeof(UITypeEditor))]
        public Sewing Sewing
        {
            get { return (sewing); }
        }

        /// <summary>
        /// Check import geomerty.
        /// </summary>
        [PropertyOrder(42)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_12")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_12")]
        public bool CheckImportGeomerty
        {
            get { return checkImportGeomerty; }
            set
            {
                if (checkImportGeomerty != value)
                {
                    checkImportGeomerty = value;
                    OnChanged(42);
                }
            }
        }

        /// <summary>
        /// Update product structure.
        /// </summary>
        [PropertyOrder(43)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_13")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_13")]
        public bool UpdateProductStructure
        {
            get { return updateProductStructure; }
            set
            {
                if (updateProductStructure != value)
                {
                    updateProductStructure = value;
                    OnChanged(43);
                }
            }
        }

        /// <summary>
        /// Add body records in product structure.
        /// </summary>
        [PropertyOrder(44)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_14")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_14")]
        public bool AddBodyRecordsInProductStructure
        {
            get { return addBodyRecordsInProductStructure; }
            set
            {
                if (addBodyRecordsInProductStructure != value)
                {
                    addBodyRecordsInProductStructure = value;
                    OnChanged(44);
                }
            }
        }
        #endregion

        #region internal properties
        internal override uint Processing
        {
            get { return (uint)(ProcessingType.Import | ProcessingType.Export); }
        }

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
            i_values[00] = exportMode;
            i_values[01] = colorSource;
            b_values[00] = exportSolidBodies;
            b_values[01] = exportSheetBodies;
            b_values[02] = exportWireBodies;
            b_values[03] = export3DPictures;
            b_values[04] = exportAnotation;
            b_values[05] = exportWelds;
            b_values[06] = exportCurves;
            b_values[07] = exportContours;
            b_values[08] = simplifyGeometry;

            i_values[03] = importMode;
            i_values[04] = heal;
            i_values[05] = createAccurateEdges;
            b_values[09] = importSolidBodies;
            b_values[10] = importSheetBodies;
            b_values[11] = importWireBodies;
            b_values[12] = importMeshBodies;
            b_values[13] = importHideBodies;
            b_values[14] = importAnotations;
            b_values[15] = importOnlyFromActiveFilter;
            b_values[16] = sewing.IsChecked;
            d_values[00] = sewing.Accuracy;
            b_values[17] = checkImportGeomerty;
            b_values[18] = updateProductStructure;
            b_values[19] = addBodyRecordsInProductStructure;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 20: objState[00] = (byte)(i_values[00] != exportMode                       ? 1 : 0); break;
                case 21: objState[01] = (byte)(i_values[01] != colorSource                      ? 1 : 0); break;
                case 22: objState[02] = (byte)(b_values[00] != exportSolidBodies                ? 1 : 0); break;
                case 23: objState[03] = (byte)(b_values[01] != exportSheetBodies                ? 1 : 0); break;
                case 24: objState[04] = (byte)(b_values[02] != exportWireBodies                 ? 1 : 0); break;
                case 25: objState[05] = (byte)(b_values[03] != export3DPictures                 ? 1 : 0); break;
                case 26: objState[06] = (byte)(b_values[04] != exportAnotation                  ? 1 : 0); break;
                case 27: objState[07] = (byte)(b_values[05] != exportWelds                      ? 1 : 0); break;
                case 28: objState[08] = (byte)(b_values[06] != exportCurves                     ? 1 : 0); break;
                case 29: objState[09] = (byte)(b_values[07] != exportContours                   ? 1 : 0); break;
                case 30: objState[10] = (byte)(b_values[08] != simplifyGeometry                 ? 1 : 0); break;
                case 31: objState[11] = (byte)(i_values[03] != importMode                       ? 1 : 0); break;
                case 32: objState[12] = (byte)(i_values[04] != heal                             ? 1 : 0); break;
                case 33: objState[13] = (byte)(i_values[05] != createAccurateEdges              ? 1 : 0); break;
                case 34: objState[14] = (byte)(b_values[09] != importSolidBodies                ? 1 : 0); break;
                case 35: objState[15] = (byte)(b_values[10] != importSheetBodies                ? 1 : 0); break;
                case 36: objState[16] = (byte)(b_values[11] != importWireBodies                 ? 1 : 0); break;
                case 37: objState[17] = (byte)(b_values[12] != importMeshBodies                 ? 1 : 0); break;
                case 38: objState[18] = (byte)(b_values[13] != importHideBodies                 ? 1 : 0); break;
                case 39: objState[19] = (byte)(b_values[14] != importAnotations                 ? 1 : 0); break;
                case 40: objState[20] = (byte)(b_values[15] != importOnlyFromActiveFilter       ? 1 : 0); break;
                case 41: objState[21] = (byte)(b_values[16] != sewing.IsChecked || 
                                               d_values[00] != sewing.Accuracy                  ? 1 : 0); break;
                case 42: objState[22] = (byte)(b_values[17] != checkImportGeomerty              ? 1 : 0); break;
                case 43: objState[23] = (byte)(b_values[18] != updateProductStructure           ? 1 : 0); break;
                case 44: objState[24] = (byte)(b_values[19] != addBodyRecordsInProductStructure ? 1 : 0); break;
            }

            isChanged = false;

            foreach (var i in objState)
            {
                if (i > 0)
                {
                    isChanged = true;
                    break;
                }
            }

            base.OnChanged(index);
        }

        internal override void Import(Document document, string targetDirectory, string path, LogFile logFile)
        {
            ImportFrom3dOption option = ImportFrom3dOption.Auto;

            switch (createAccurateEdges)
            {
                case 1: option = ImportFrom3dOption.Yes; break;
                case 2: option = ImportFrom3dOption.No;  break;
            }

            ImportFrom3dCommon import = new ImportFrom3dCommon(document)
            {
                CreateAccurateEdges        = option,
                CheckImportGeomerty        = checkImportGeomerty,
                ImportOnlyFromActiveFilter = importOnlyFromActiveFilter,
                PathToAssemblyFolder       = targetDirectory,
                ShowDialog                 = false
            };

            if (importMode == 2)
            {
                import.Mode = ImportFrom3dMode.Operation;
                import.ImportSolidBodies = importSolidBodies;
                import.ImportSheetBodies = importSheetBodies;
                import.ImportHideBodies  = importHideBodies;
            }
            else
            {
                import.Mode = importMode > 0 
                    ? ImportFrom3dMode.BodySet 
                    : ImportFrom3dMode.Assembly;
                import.ImportSolidBodies = importSolidBodies;
                import.ImportSheetBodies = importSheetBodies;
                import.ImportWireBodies  = importWireBodies;
                import.ImportMeshBodies  = importMeshBodies;
                import.ImportHideBodies  = importHideBodies;
                import.ImportAnotations  = importAnotations;
            }

            if (sewing.IsChecked)
            {
                import.Sewing       = sewing.IsChecked;
                import.SewTolerance = sewing.Accuracy;
            }

            if (updateProductStructure)
            {
                import.UpdateProductStructure = updateProductStructure;
                import.AddBodyRecordsInProductStructure = addBodyRecordsInProductStructure;
            }

            if (import.Import(path))
            {
                logFile.AppendLine(string.Format("Import to:\t\t{0}", path));
            }
        }

        internal override XElement NewTranslator(TranslatorType translator)
        {
            XElement element = new XElement("translator",
                new XAttribute("id", translator),
                new XAttribute("processing", Processing),
                new XElement("parameter",
                    new XAttribute("name", "FileNameSuffix"),
                    new XAttribute("value", FileNameSuffix)),
                new XElement("parameter",
                    new XAttribute("name", "TemplateFileName"),
                    new XAttribute("value", TemplateFileName)),
                new XElement("parameter",
                    new XAttribute("name", "TargetExtension"),
                    new XAttribute("value", TargetExtension)),
                new XElement("parameter",
                    new XAttribute("name", "ExportMode"),
                    new XAttribute("value", ExportMode)),
                new XElement("parameter",
                    new XAttribute("name", "ColorSource"),
                    new XAttribute("value", ColorSource)),
                new XElement("parameter",
                    new XAttribute("name", "ExportSolidBodies"),
                    new XAttribute("value", ExportSolidBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportSheetBodies"),
                    new XAttribute("value", ExportSheetBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportWireBodies"),
                    new XAttribute("value", ExportWireBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "Export3DPictures"),
                    new XAttribute("value", Export3DPictures ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportAnotation"),
                    new XAttribute("value", ExportAnotation ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportWelds"),
                    new XAttribute("value", ExportWelds ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportCurves"),
                    new XAttribute("value", ExportCurves ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ExportContours"),
                    new XAttribute("value", ExportContours ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "SimplifyGeometry"),
                    new XAttribute("value", SimplifyGeometry ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportMode"),
                    new XAttribute("value", ImportMode)),
                new XElement("parameter",
                    new XAttribute("name", "Heal"),
                    new XAttribute("value", Heal)),
                new XElement("parameter",
                    new XAttribute("name", "CreateAccurateEdges"),
                    new XAttribute("value", CreateAccurateEdges)),
                new XElement("parameter",
                    new XAttribute("name", "ImportSolidBodies"),
                    new XAttribute("value", ImportSolidBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportSheetBodies"),
                    new XAttribute("value", ImportSheetBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportWireBodies"),
                    new XAttribute("value", ImportWireBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportMeshBodies"),
                    new XAttribute("value", ImportMeshBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportHideBodies"),
                    new XAttribute("value", ImportHideBodies ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportAnotations"),
                    new XAttribute("value", ImportAnotations ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "ImportOnlyFromActiveFilter"),
                    new XAttribute("value", ImportOnlyFromActiveFilter ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "Sewing"),
                    new XAttribute("value", Sewing.ToString())),
                new XElement("parameter",
                    new XAttribute("name", "CheckImportGeomerty"), 
                    new XAttribute("value", CheckImportGeomerty ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "UpdateProductStructure"),
                    new XAttribute("value", UpdateProductStructure ? "1" : "0")),
                new XElement("parameter",
                    new XAttribute("name", "AddBodyRecordsInProductStructure"),
                    new XAttribute("value", AddBodyRecordsInProductStructure ? "1" : "0")));

            return element;
        }

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "ExportMode":
                    if (flag == 0)
                        exportMode = int.Parse(value);
                    else
                        value = exportMode.ToString();
                    break;
                case "ColorSource":
                    if (flag == 0)
                        colorSource = int.Parse(value);
                    else
                        value = colorSource.ToString();
                    break;
                case "ExportSolidBodies":
                    if (flag == 0)
                        exportSolidBodies = value == "1";
                    else
                        value = exportSolidBodies ? "1" : "0";
                    break;
                case "ExportSheetBodies":
                    if (flag == 0)
                        exportSheetBodies = value == "1";
                    else
                        value = exportSheetBodies ? "1" : "0";
                    break;
                case "ExportWireBodies":
                    if (flag == 0)
                        exportWireBodies = value == "1";
                    else
                        value = exportWireBodies ? "1" : "0";
                    break;
                case "Export3DPictures":
                    if (flag == 0)
                        export3DPictures = value == "1";
                    else
                        value = export3DPictures ? "1" : "0";
                    break;
                case "ExportAnotation":
                    if (flag == 0)
                        exportAnotation = value == "1";
                    else
                        value = exportAnotation ? "1" : "0";
                    break;
                case "ExportWelds":
                    if (flag == 0)
                        exportWelds = value == "1";
                    else
                        value = exportWelds ? "1" : "0";
                    break;
                case "ExportCurves":
                    if (flag == 0)
                        exportCurves = value == "1";
                    else
                        value = exportCurves ? "1" : "0";
                    break;
                case "ExportContours":
                    if (flag == 0)
                        exportContours = value == "1";
                    else
                        value = exportContours ? "1" : "0";
                    break;
                case "SimplifyGeometry":
                    if (flag == 0)
                        simplifyGeometry = value == "1";
                    else
                        value = simplifyGeometry ? "1" : "0";
                    break;
                case "ImportMode":
                    if (flag == 0)
                        importMode = int.Parse(value);
                    else
                        value = importMode.ToString();
                    break;
                case "Heal":
                    if (flag == 0)
                        heal = int.Parse(value);
                    else
                        value = heal.ToString();
                    break;
                case "CreateAccurateEdges":
                    if (flag == 0)
                        createAccurateEdges = int.Parse(value);
                    else
                        value = createAccurateEdges.ToString();
                    break;
                case "ImportSolidBodies":
                    if (flag == 0)
                        importSolidBodies = value == "1";
                    else
                        value = importSolidBodies ? "1" : "0";
                    break;
                case "ImportSheetBodies":
                    if (flag == 0)
                        importSheetBodies = value == "1";
                    else
                        value = importSheetBodies ? "1" : "0";
                    break;
                case "ImportWireBodies":
                    if (flag == 0)
                        importWireBodies = value == "1";
                    else
                        value = importWireBodies ? "1" : "0";
                    break;
                case "ImportMeshBodies":
                    if (flag == 0)
                        importMeshBodies = value == "1";
                    else
                        value = importMeshBodies ? "1" : "0";
                    break;
                case "ImportHideBodies":
                    if (flag == 0)
                        importHideBodies = value == "1";
                    else
                        value = importHideBodies ? "1" : "0";
                    break;
                case "ImportAnotations":
                    if (flag == 0)
                        importAnotations = value == "1";
                    else
                        value = importAnotations ? "1" : "0";
                    break;
                case "ImportOnlyFromActiveFilter":
                    if (flag == 0)
                        importOnlyFromActiveFilter = value == "1";
                    else
                        value = importOnlyFromActiveFilter ? "1" : "0";
                    break;
                case "Sewing":
                    if (flag == 0)
                    {
                        string[] values = value.Split(' ');

                        sewing.IsChecked = values[0] == "1";
                        sewing.Accuracy = double.Parse(values[1],
                            NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = sewing.ToString();
                    }
                    break;
                case "CheckImportGeomerty":
                    if (flag == 0)
                        checkImportGeomerty = value == "1";
                    else
                        value = checkImportGeomerty ? "1" : "0";
                    break;
                case "UpdateProductStructure":
                    if (flag == 0)
                        updateProductStructure = value == "1";
                    else
                        value = updateProductStructure ? "1" : "0";
                    break;
                case "AddBodyRecordsInProductStructure":
                    if (flag == 0)
                        addBodyRecordsInProductStructure = value == "1";
                    else
                        value = addBodyRecordsInProductStructure ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

    public class Sewing : INotifyPropertyChanged
    {
        #region private fields
        private bool isChecked;
        private double accuracy;
        #endregion

        public Sewing()
        {
            isChecked = true;
            accuracy  = 0.1;
        }

        #region public properties
        /// <summary>
        /// Sewing.
        /// </summary>
        [Browsable(false)]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        /// <summary>
        /// Sew accuracy in mm.
        /// </summary>
        [Browsable(false)]
        public double Accuracy
        {
            get { return accuracy; }
            set
            {
                if (accuracy != value)
                {
                    accuracy = value;
                    OnPropertyChanged("Accuracy");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string[] values = new string[]
            {
                isChecked ? "1" : "0",
                accuracy.ToString(CultureInfo.InvariantCulture)
            };

            return values.ToString(" ");
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OpPropertyChanged event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

#pragma warning disable CA1812
    internal class ExportModeItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_1", 0) }
            };
        }
    }

    internal class ColorSourceItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                { 0, Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_0", 0) },
                { 1, Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_1", 0) }
            };
        }
    }

    internal class ImportModeItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                {0, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_0", 0) },
                {1, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_1", 0) },
                {2, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_2", 0) }
            };
        }
    }

    internal class ImportFrom3dOptionItems : IItemsSource
    {
        public ItemCollection GetValues()
        {
            return new ItemCollection
            {
                {0, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_0", 0) },
                {1, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_1", 0) },
                {2, Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_2", 0) }
            };
        }
    }
}
