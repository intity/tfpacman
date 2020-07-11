using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Base class for import/export to some universal 3D formats (STEP, ACIS, JT, PRC, 3MF, IGES).
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 5)]
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 6)]
    public class Translator3D : OutputFiles
    {
        #region private fields
        int exportMode;
        int colorSource;
        bool export3DPictures;
        bool exportAnotation;
        bool exportContours;
        bool exportCurves;
        bool exportSheetBodies;
        bool exportSolidBodies;
        bool exportWelds;
        bool exportWireBodies;
        bool simplifyGeometry;
        int importMode;
        int heal;
        int createAccurateEdges;
        bool importAnotations;
        bool importSolidBodies;
        bool importSheetBodies;
        bool importMeshBodies;
        bool importWireBodies;
        bool importHideBodies;
        bool importOnlyFromActiveFilter;
        bool sewing;
        double sewTolerance;
        bool checkImportGeomerty;
        bool updateProductStructure;
        bool addBodyRecordsInProductStructure;

        XAttribute data_4_1;
        XAttribute data_4_2;
        XAttribute data_4_3;
        XAttribute data_4_4;
        XAttribute data_4_5;
        XAttribute data_4_6;
        XAttribute data_4_7;
        XAttribute data_4_8;
        XAttribute data_4_9;
        XAttribute data_4_A;
        XAttribute data_4_B;
        XAttribute data_5_1;
        XAttribute data_5_2;
        XAttribute data_5_3;
        XAttribute data_5_4;
        XAttribute data_5_5;
        XAttribute data_5_6;
        XAttribute data_5_7;
        XAttribute data_5_8;
        XAttribute data_5_9;
        XAttribute data_5_A;
        XAttribute data_5_B;
        XAttribute data_5_C;
        XAttribute data_5_D;
        XAttribute data_5_E;
        XAttribute data_5_F;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator3D(string ext) : base (ext)
        {
            colorSource                = 1;
            exportSheetBodies          = true;
            exportSolidBodies          = true;
            exportWireBodies           = true;
            importMode                 = 1;
            createAccurateEdges        = 2;
            importSolidBodies          = true;
            importSheetBodies          = true;
            importMeshBodies           = true;
            importWireBodies           = true;
            importOnlyFromActiveFilter = true;
            sewing                     = true;
            sewTolerance               = 0.1;
            checkImportGeomerty        = true;
        }

        #region public properties
        /// <summary>
        /// Export type.
        /// (0) - Assembly,
        /// (1) - BodySet
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_1")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ExportMode
        {
            get => exportMode;
            set
            {
                if (exportMode != value)
                {
                    exportMode = value;
                    data_4_1.Value = value.ToString();

                    OnPropertyChanged("ExportMode");
                }
            }
        }

        /// <summary>
        /// Source for color:
        /// (0) - ToneColor,
        /// (1) - MaterialColor
        /// </summary>
        [PropertyOrder(21)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_2")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ColorSource
        {
            get => colorSource;
            set
            {
                if (colorSource != value)
                {
                    colorSource = value;
                    data_4_2.Value = value.ToString();

                    OnPropertyChanged("ColorSource");
                }
            }
        }

        /// <summary>
        /// Export solid bodies.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_3")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_3")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportSolidBodies
        {
            get => exportSolidBodies;
            set
            {
                if (exportSolidBodies != value)
                {
                    exportSolidBodies = value;
                    data_4_3.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportSolidBodies");
                }
            }
        }

        /// <summary>
        /// Export sheet bodies.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_4")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_4")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportSheetBodies
        {
            get => exportSheetBodies;
            set
            {
                if (exportSheetBodies != value)
                {
                    exportSheetBodies = value;
                    data_4_4.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportSheetBodies");
                }
            }
        }

        /// <summary>
        /// Export wire bodies.
        /// </summary>
        [PropertyOrder(24)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_5")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportWireBodies
        {
            get => exportWireBodies;
            set
            {
                if (exportWireBodies != value)
                {
                    exportWireBodies = value;
                    data_4_5.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportWireBodies");
                }
            }
        }

        /// <summary>
        /// Export 3D images.
        /// </summary>
        [PropertyOrder(25)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_6")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Export3DPictures
        {
            get => export3DPictures;
            set
            {
                if (export3DPictures != value)
                {
                    export3DPictures = value;
                    data_4_6.Value = value ? "1" : "0";

                    OnPropertyChanged("Export3DPictures");
                }
            }
        }

        /// <summary>
        /// Export annotations.
        /// </summary>
        [PropertyOrder(26)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_7")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportAnotation
        {
            get => exportAnotation;
            set
            {
                if (exportAnotation != value)
                {
                    exportAnotation = value;
                    data_4_7.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportAnotation");
                }
            }
        }

        /// <summary>
        /// Export welds.
        /// </summary>
        [PropertyOrder(27)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_8")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_8")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportWelds
        {
            get => exportWelds;
            set
            {
                if (exportWelds != value)
                {
                    exportWelds = value;
                    data_4_8.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportWelds");
                }
            }
        }

        /// <summary>
        /// Export curves.
        /// </summary>
        [PropertyOrder(28)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_9")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_9")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportCurves
        {
            get => exportCurves;
            set
            {
                if (exportCurves != value)
                {
                    exportCurves = value;
                    data_4_9.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportCurves");
                }
            }
        }

        /// <summary>
        /// Export profiles.
        /// </summary>
        [PropertyOrder(29)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_10")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_10")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportContours
        {
            get => exportContours;
            set
            {
                if (exportContours != value)
                {
                    exportContours = value;
                    data_4_A.Value = value ? "1" : "0";

                    OnPropertyChanged("ExportContours");
                }
            }
        }

        /// <summary>
        /// Simplify geometry.
        /// </summary>
        [PropertyOrder(30)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn5_11")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn5_11")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool SimplifyGeometry
        {
            get => simplifyGeometry;
            set
            {
                if (simplifyGeometry != value)
                {
                    simplifyGeometry = value;
                    data_4_B.Value = value ? "1" : "0";

                    OnPropertyChanged("SimplifyGeometry");
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
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_1")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ImportMode
        {
            get => importMode;
            set
            {
                if (importMode != value)
                {
                    importMode = value;
                    data_5_1.Value = value.ToString();

                    OnPropertyChanged("ImportMode");
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
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_2")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Heal
        {
            get => heal;
            set
            {
                if (heal != value)
                {
                    heal = value;
                    data_5_2.Value = value.ToString();

                    OnPropertyChanged("Heal");
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
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_3")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_3")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int CreateAccurateEdges
        {
            get => createAccurateEdges;
            set
            {
                if (createAccurateEdges != value)
                {
                    createAccurateEdges = value;
                    data_5_3.Value = value.ToString();

                    OnPropertyChanged("CreateAccurateEdges");
                }
            }
        }

        /// <summary>
        /// Import solid bodies.
        /// </summary>
        [PropertyOrder(34)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_4")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_4")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportSolidBodies
        {
            get => importSolidBodies;
            set
            {
                if (importSolidBodies != value)
                {
                    importSolidBodies = value;
                    data_5_4.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportSolidBodies");
                }
            }
        }

        /// <summary>
        /// Import sheet bodies.
        /// </summary>
        [PropertyOrder(35)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_5")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportSheetBodies
        {
            get => importSheetBodies;
            set
            {
                if (importSheetBodies != value)
                {
                    importSheetBodies = value;
                    data_5_5.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportSheetBodies");
                }
            }
        }

        /// <summary>
        /// Import wire bodies.
        /// </summary>
        [PropertyOrder(36)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_6")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportWireBodies
        {
            get => importWireBodies;
            set
            {
                if (importWireBodies != value)
                {
                    importWireBodies = value;
                    data_5_6.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportWireBodies");
                }
            }
        }

        /// <summary>
        /// Import mesh bodies as 3D images.
        /// </summary>
        [PropertyOrder(37)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_7")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportMeshBodies
        {
            get => importMeshBodies;
            set
            {
                if (importMeshBodies != value)
                {
                    importMeshBodies = value;
                    data_5_7.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportMeshBodies");
                }
            }
        }

        /// <summary>
        /// Import hidden bodies.
        /// </summary>
        [PropertyOrder(38)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_8")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_8")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportHideBodies
        {
            get => importHideBodies;
            set
            {
                if (importHideBodies != value)
                {
                    importHideBodies = value;
                    data_5_8.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportHideBodies");
                }
            }
        }

        /// <summary>
        /// Import annotations.
        /// </summary>
        [PropertyOrder(39)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_9")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_9")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportAnotations
        {
            get => importAnotations;
            set
            {
                if (importAnotations != value)
                {
                    importAnotations = value;
                    data_5_9.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportAnotations");
                }
            }
        }

        /// <summary>
        /// Import bodies from active layer only.
        /// </summary>
        [PropertyOrder(40)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_10")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_10")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportOnlyFromActiveFilter
        {
            get => importOnlyFromActiveFilter;
            set
            {
                if (importOnlyFromActiveFilter != value)
                {
                    importOnlyFromActiveFilter = value;
                    data_5_A.Value = value ? "1" : "0";

                    OnPropertyChanged("ImportOnlyFromActiveFilter");
                }
            }
        }

        [Browsable(false)]
        public bool Sewing
        {
            get => sewing;
            set
            {
                if (sewing != value)
                {
                    sewing = value;
                    data_5_B.Value = value ? "1" : "0";

                    OnPropertyChanged("Sewing");
                }
            }
        }

        /// <summary>
        /// Accuracy sewing.
        /// </summary>
        [PropertyOrder(41)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_11")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_11")]
        [Editor(typeof(SewingEditor), typeof(UITypeEditor))]
        public double SewTolerance
        {
            get => sewTolerance;
            set
            {
                if (sewTolerance != value)
                {
                    sewTolerance = value;
                    data_5_C.Value = value
                        .ToString(CultureInfo.InvariantCulture);

                    OnPropertyChanged("SewTolerance");
                }
            }
        }

        /// <summary>
        /// Check import geomerty.
        /// </summary>
        [PropertyOrder(42)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_12")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_12")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool CheckImportGeomerty
        {
            get => checkImportGeomerty;
            set
            {
                if (checkImportGeomerty != value)
                {
                    checkImportGeomerty = value;
                    data_5_D.Value = value ? "1" : "0";

                    OnPropertyChanged("CheckImportGeomerty");
                }
            }
        }

        /// <summary>
        /// Update product structure.
        /// </summary>
        [PropertyOrder(43)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_13")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_13")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool UpdateProductStructure
        {
            get => updateProductStructure;
            set
            {
                if (updateProductStructure != value)
                {
                    updateProductStructure = value;
                    data_5_E.Value = value ? "1" : "0";

                    OnPropertyChanged("UpdateProductStructure");
                }
            }
        }

        /// <summary>
        /// Add body records in product structure.
        /// </summary>
        [PropertyOrder(44)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn6_14")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn6_14")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool AddBodyRecordsInProductStructure
        {
            get => addBodyRecordsInProductStructure;
            set
            {
                if (addBodyRecordsInProductStructure != value)
                {
                    addBodyRecordsInProductStructure = value;
                    data_5_F.Value = value ? "1" : "0";

                    OnPropertyChanged("AddBodyRecordsInProductStructure");
                }
            }
        }
        #endregion

        #region internal methods
        internal override void Import(Document document, string targetDirectory, string path, Logging logging)
        {
            ImportFrom3dOption option = ImportFrom3dOption.Auto;

            switch (CreateAccurateEdges)
            {
                case 1: option = ImportFrom3dOption.Yes; break;
                case 2: option = ImportFrom3dOption.No;  break;
            }

            ImportFrom3dCommon import = new ImportFrom3dCommon(document)
            {
                CreateAccurateEdges        = option,
                CheckImportGeomerty        = CheckImportGeomerty,
                ImportOnlyFromActiveFilter = ImportOnlyFromActiveFilter,
                PathToAssemblyFolder       = targetDirectory,
                ShowDialog                 = false
            };

            if (ImportMode == 2)
            {
                import.Mode = ImportFrom3dMode.Operation;
                import.ImportSolidBodies = ImportSolidBodies;
                import.ImportSheetBodies = ImportSheetBodies;
                import.ImportHideBodies  = ImportHideBodies;
            }
            else
            {
                import.Mode = ImportMode > 0 
                    ? ImportFrom3dMode.BodySet 
                    : ImportFrom3dMode.Assembly;
                import.ImportSolidBodies = ImportSolidBodies;
                import.ImportSheetBodies = ImportSheetBodies;
                import.ImportWireBodies  = ImportWireBodies;
                import.ImportMeshBodies  = ImportMeshBodies;
                import.ImportHideBodies  = ImportHideBodies;
                import.ImportAnotations  = ImportAnotations;
            }

            if (Sewing)
            {
                import.Sewing       = Sewing;
                import.SewTolerance = SewTolerance;
            }

            if (UpdateProductStructure)
            {
                import.UpdateProductStructure = UpdateProductStructure;
                import.AddBodyRecordsInProductStructure = AddBodyRecordsInProductStructure;
            }

            if (import.Import(path))
            {
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Import from [path: {0}]", path));
            }
        }

        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data_4_1 = new XAttribute("value", ExportMode.ToString());
            data_4_2 = new XAttribute("value", ColorSource.ToString());
            data_4_3 = new XAttribute("value", ExportSolidBodies      ? "1" : "0");
            data_4_4 = new XAttribute("value", ExportSheetBodies      ? "1" : "0");
            data_4_5 = new XAttribute("value", ExportWireBodies       ? "1" : "0");
            data_4_6 = new XAttribute("value", Export3DPictures       ? "1" : "0");
            data_4_7 = new XAttribute("value", ExportAnotation        ? "1" : "0");
            data_4_8 = new XAttribute("value", ExportWelds            ? "1" : "0");
            data_4_9 = new XAttribute("value", ExportCurves           ? "1" : "0");
            data_4_A = new XAttribute("value", ExportContours         ? "1" : "0");
            data_4_B = new XAttribute("value", SimplifyGeometry       ? "1" : "0");
            data_5_1 = new XAttribute("value", ImportMode.ToString());
            data_5_2 = new XAttribute("value", Heal.ToString());
            data_5_3 = new XAttribute("value", CreateAccurateEdges.ToString());
            data_5_4 = new XAttribute("value", ImportSolidBodies      ? "1" : "0");
            data_5_5 = new XAttribute("value", ImportSheetBodies      ? "1" : "0");
            data_5_6 = new XAttribute("value", ImportWireBodies       ? "1" : "0");
            data_5_7 = new XAttribute("value", ImportMeshBodies       ? "1" : "0");
            data_5_8 = new XAttribute("value", ImportHideBodies       ? "1" : "0");
            data_5_9 = new XAttribute("value", ImportAnotations       ? "1" : "0");
            data_5_A = new XAttribute("value", ImportOnlyFromActiveFilter ? "1" : "0");
            data_5_B = new XAttribute("value", Sewing                 ? "1" : "0");
            data_5_C = new XAttribute("value", SewTolerance.ToString(CultureInfo.InvariantCulture));
            data_5_D = new XAttribute("value", CheckImportGeomerty    ? "1" : "0");
            data_5_E = new XAttribute("value", UpdateProductStructure ? "1" : "0");
            data_5_F = new XAttribute("value", AddBodyRecordsInProductStructure ? "1" : "0");

            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportMode"),
                data_4_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ColorSource"),
                data_4_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportSolidBodies"),
                data_4_3));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportSheetBodies"),
                data_4_4));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportWireBodies"),
                data_4_5));
            data.Add(new XElement("parameter",
                new XAttribute("name", "Export3DPictures"),
                data_4_6));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportAnotation"),
                data_4_7));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportWelds"),
                data_4_8));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportCurves"),
                data_4_9));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ExportContours"),
                data_4_A));
            data.Add(new XElement("parameter",
                new XAttribute("name", "SimplifyGeometry"),
                data_4_B));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportMode"),
                data_5_1));
            data.Add(new XElement("parameter",
                new XAttribute("name", "Heal"),
                data_5_2));
            data.Add(new XElement("parameter",
                new XAttribute("name", "CreateAccurateEdges"),
                data_5_3));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportSolidBodies"),
                data_5_4));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportSheetBodies"),
                data_5_5));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportWireBodies"),
                data_5_6));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportMeshBodies"),
                data_5_7));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportHideBodies"),
                data_5_8));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportAnotations"),
                data_5_9));
            data.Add(new XElement("parameter",
                new XAttribute("name", "ImportOnlyFromActiveFilter"),
                data_5_A));
            data.Add(new XElement("parameter",
                new XAttribute("name", "Sewing"),
                data_5_B));
            data.Add(new XElement("parameter",
                new XAttribute("name", "SewTolerance"),
                data_5_C));
            data.Add(new XElement("parameter",
                new XAttribute("name", "CheckImportGeomerty"),
                data_5_D));
            data.Add(new XElement("parameter",
                new XAttribute("name", "UpdateProductStructure"),
                data_5_E));
            data.Add(new XElement("parameter",
                new XAttribute("name", "AddBodyRecordsInProductStructure"),
                data_5_F));

            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);

            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "ExportMode":
                    exportMode = int.Parse(a.Value);
                    data_4_1 = a;
                    break;
                case "ColorSource":
                    colorSource = int.Parse(a.Value);
                    data_4_2 = a;
                    break;
                case "ExportSolidBodies":
                    exportSolidBodies = a.Value == "1";
                    data_4_3 = a;
                    break;
                case "ExportSheetBodies":
                    exportSheetBodies = a.Value == "1";
                    data_4_4 = a;
                    break;
                case "ExportWireBodies":
                    exportWireBodies = a.Value == "1";
                    data_4_5 = a;
                    break;
                case "Export3DPictures":
                    export3DPictures = a.Value == "1";
                    data_4_6 = a;
                    break;
                case "ExportAnotation":
                    exportAnotation = a.Value == "1";
                    data_4_7 = a;
                    break;
                case "ExportWelds":
                    exportWelds = a.Value == "1";
                    data_4_8 = a;
                    break;
                case "ExportCurves":
                    exportCurves = a.Value == "1";
                    data_4_9 = a;
                    break;
                case "ExportContours":
                    exportContours = a.Value == "1";
                    data_4_A = a;
                    break;
                case "SimplifyGeometry":
                    simplifyGeometry = a.Value == "1";
                    data_4_B = a;
                    break;
                case "ImportMode":
                    importMode = int.Parse(a.Value);
                    data_5_1 = a;
                    break;
                case "Heal":
                    heal = int.Parse(a.Value);
                    data_5_2 = a;
                    break;
                case "CreateAccurateEdges":
                    createAccurateEdges = int.Parse(a.Value);
                    data_5_3 = a;
                    break;
                case "ImportSolidBodies":
                    importSolidBodies = a.Value == "1";
                    data_5_4 = a;
                    break;
                case "ImportSheetBodies":
                    importSheetBodies = a.Value == "1";
                    data_5_5 = a;
                    break;
                case "ImportWireBodies":
                    importWireBodies = a.Value == "1";
                    data_5_6 = a;
                    break;
                case "ImportMeshBodies":
                    importMeshBodies = a.Value == "1";
                    data_5_7 = a;
                    break;
                case "ImportHideBodies":
                    importHideBodies = a.Value == "1";
                    data_5_8 = a;
                    break;
                case "ImportAnotations":
                    importAnotations = a.Value == "1";
                    data_5_9 = a;
                    break;
                case "ImportOnlyFromActiveFilter":
                    importOnlyFromActiveFilter = a.Value == "1";
                    data_5_A = a;
                    break;
                case "Sewing":
                    sewing = a.Value == "1";
                    data_5_B = a;
                    break;
                case "SewTolerance":
                    sewTolerance = double
                        .Parse(a.Value, NumberStyles.Float,
                        CultureInfo.InvariantCulture);
                    data_5_C = a;
                    break;
                case "CheckImportGeomerty":
                    checkImportGeomerty = a.Value == "1";
                    data_5_D = a;
                    break;
                case "UpdateProductStructure":
                    updateProductStructure = a.Value == "1";
                    data_5_E = a;
                    break;
                case "AddBodyRecordsInProductStructure":
                    addBodyRecordsInProductStructure = a.Value == "1";
                    data_5_F = a;
                    break;
            }
        }
        #endregion
    }
}
