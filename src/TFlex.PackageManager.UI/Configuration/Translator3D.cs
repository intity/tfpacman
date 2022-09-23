using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml;
using TFlex.Model;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// Base class for import/export to some universal 3D formats (STEP, ACIS, JT, PRC, 3MF, IGES).
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_3D, 5)]
    [CustomCategoryOrder(Resources.TRANSLATOR_3D, 6)]
    [Serializable]
    public class Translator3D : Files
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
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator3D()
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
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_1")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ExportMode
        {
            get => exportMode;
            set
            {
                if (exportMode != value)
                {
                    exportMode = value;
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
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_2")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ColorSource
        {
            get => colorSource;
            set
            {
                if (colorSource != value)
                {
                    colorSource = value;
                    OnPropertyChanged("ColorSource");
                }
            }
        }

        /// <summary>
        /// Export solid bodies.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_3")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_3")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportSolidBodies
        {
            get => exportSolidBodies;
            set
            {
                if (exportSolidBodies != value)
                {
                    exportSolidBodies = value;
                    OnPropertyChanged("ExportSolidBodies");
                }
            }
        }

        /// <summary>
        /// Export sheet bodies.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_4")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_4")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportSheetBodies
        {
            get => exportSheetBodies;
            set
            {
                if (exportSheetBodies != value)
                {
                    exportSheetBodies = value;
                    OnPropertyChanged("ExportSheetBodies");
                }
            }
        }

        /// <summary>
        /// Export wire bodies.
        /// </summary>
        [PropertyOrder(24)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_5")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportWireBodies
        {
            get => exportWireBodies;
            set
            {
                if (exportWireBodies != value)
                {
                    exportWireBodies = value;
                    OnPropertyChanged("ExportWireBodies");
                }
            }
        }

        /// <summary>
        /// Export 3D images.
        /// </summary>
        [PropertyOrder(25)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_6")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool Export3DPictures
        {
            get => export3DPictures;
            set
            {
                if (export3DPictures != value)
                {
                    export3DPictures = value;
                    OnPropertyChanged("Export3DPictures");
                }
            }
        }

        /// <summary>
        /// Export annotations.
        /// </summary>
        [PropertyOrder(26)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_7")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportAnotation
        {
            get => exportAnotation;
            set
            {
                if (exportAnotation != value)
                {
                    exportAnotation = value;
                    OnPropertyChanged("ExportAnotation");
                }
            }
        }

        /// <summary>
        /// Export welds.
        /// </summary>
        [PropertyOrder(27)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_8")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_8")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportWelds
        {
            get => exportWelds;
            set
            {
                if (exportWelds != value)
                {
                    exportWelds = value;
                    OnPropertyChanged("ExportWelds");
                }
            }
        }

        /// <summary>
        /// Export curves.
        /// </summary>
        [PropertyOrder(28)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_9")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_9")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportCurves
        {
            get => exportCurves;
            set
            {
                if (exportCurves != value)
                {
                    exportCurves = value;
                    OnPropertyChanged("ExportCurves");
                }
            }
        }

        /// <summary>
        /// Export profiles.
        /// </summary>
        [PropertyOrder(29)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_10")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_10")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ExportContours
        {
            get => exportContours;
            set
            {
                if (exportContours != value)
                {
                    exportContours = value;
                    OnPropertyChanged("ExportContours");
                }
            }
        }

        /// <summary>
        /// Simplify geometry.
        /// </summary>
        [PropertyOrder(30)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category5")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn5_11")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn5_11")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool SimplifyGeometry
        {
            get => simplifyGeometry;
            set
            {
                if (simplifyGeometry != value)
                {
                    simplifyGeometry = value;
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
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_1")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_1")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int ImportMode
        {
            get => importMode;
            set
            {
                if (importMode != value)
                {
                    importMode = value;
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
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_2")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_2")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int Heal
        {
            get => heal;
            set
            {
                if (heal != value)
                {
                    heal = value;
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
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_3")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_3")]
        [Editor(typeof(CustomComboBoxEditor), typeof(UITypeEditor))]
        public int CreateAccurateEdges
        {
            get => createAccurateEdges;
            set
            {
                if (createAccurateEdges != value)
                {
                    createAccurateEdges = value;
                    OnPropertyChanged("CreateAccurateEdges");
                }
            }
        }

        /// <summary>
        /// Import solid bodies.
        /// </summary>
        [PropertyOrder(34)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_4")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_4")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportSolidBodies
        {
            get => importSolidBodies;
            set
            {
                if (importSolidBodies != value)
                {
                    importSolidBodies = value;
                    OnPropertyChanged("ImportSolidBodies");
                }
            }
        }

        /// <summary>
        /// Import sheet bodies.
        /// </summary>
        [PropertyOrder(35)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_5")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_5")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportSheetBodies
        {
            get => importSheetBodies;
            set
            {
                if (importSheetBodies != value)
                {
                    importSheetBodies = value;
                    OnPropertyChanged("ImportSheetBodies");
                }
            }
        }

        /// <summary>
        /// Import wire bodies.
        /// </summary>
        [PropertyOrder(36)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_6")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_6")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportWireBodies
        {
            get => importWireBodies;
            set
            {
                if (importWireBodies != value)
                {
                    importWireBodies = value;
                    OnPropertyChanged("ImportWireBodies");
                }
            }
        }

        /// <summary>
        /// Import mesh bodies as 3D images.
        /// </summary>
        [PropertyOrder(37)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_7")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_7")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportMeshBodies
        {
            get => importMeshBodies;
            set
            {
                if (importMeshBodies != value)
                {
                    importMeshBodies = value;
                    OnPropertyChanged("ImportMeshBodies");
                }
            }
        }

        /// <summary>
        /// Import hidden bodies.
        /// </summary>
        [PropertyOrder(38)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_8")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_8")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportHideBodies
        {
            get => importHideBodies;
            set
            {
                if (importHideBodies != value)
                {
                    importHideBodies = value;
                    OnPropertyChanged("ImportHideBodies");
                }
            }
        }

        /// <summary>
        /// Import annotations.
        /// </summary>
        [PropertyOrder(39)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_9")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_9")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportAnotations
        {
            get => importAnotations;
            set
            {
                if (importAnotations != value)
                {
                    importAnotations = value;
                    OnPropertyChanged("ImportAnotations");
                }
            }
        }

        /// <summary>
        /// Import bodies from active layer only.
        /// </summary>
        [PropertyOrder(40)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_10")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_10")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool ImportOnlyFromActiveFilter
        {
            get => importOnlyFromActiveFilter;
            set
            {
                if (importOnlyFromActiveFilter != value)
                {
                    importOnlyFromActiveFilter = value;
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
                    OnPropertyChanged("Sewing");
                }
            }
        }

        /// <summary>
        /// Accuracy sewing.
        /// </summary>
        [PropertyOrder(41)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_11")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_11")]
        [Editor(typeof(SewingEditor), typeof(UITypeEditor))]
        public double SewTolerance
        {
            get => sewTolerance;
            set
            {
                if (sewTolerance != value)
                {
                    sewTolerance = value;
                    OnPropertyChanged("SewTolerance");
                }
            }
        }

        /// <summary>
        /// Check import geomerty.
        /// </summary>
        [PropertyOrder(42)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_12")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_12")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool CheckImportGeomerty
        {
            get => checkImportGeomerty;
            set
            {
                if (checkImportGeomerty != value)
                {
                    checkImportGeomerty = value;
                    OnPropertyChanged("CheckImportGeomerty");
                }
            }
        }

        /// <summary>
        /// Update product structure.
        /// </summary>
        [PropertyOrder(43)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_13")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_13")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool UpdateProductStructure
        {
            get => updateProductStructure;
            set
            {
                if (updateProductStructure != value)
                {
                    updateProductStructure = value;
                    OnPropertyChanged("UpdateProductStructure");
                }
            }
        }

        /// <summary>
        /// Add body records in product structure.
        /// </summary>
        [PropertyOrder(44)]
        [CustomCategory(Resources.TRANSLATOR_3D, "category6")]
        [CustomDisplayName(Resources.TRANSLATOR_3D, "dn6_14")]
        [CustomDescription(Resources.TRANSLATOR_3D, "dn6_14")]
        [Editor(typeof(CustomCheckBoxEditor), typeof(UITypeEditor))]
        public bool AddBodyRecordsInProductStructure
        {
            get => addBodyRecordsInProductStructure;
            set
            {
                if (addBodyRecordsInProductStructure != value)
                {
                    addBodyRecordsInProductStructure = value;
                    OnPropertyChanged("AddBodyRecordsInProductStructure");
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 26 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "ExportMode":
                        exportMode = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ColorSource":
                        colorSource = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ExportSolidBodies":
                        exportSolidBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportSheetBodies":
                        exportSheetBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportWireBodies":
                        exportWireBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "Export3DPictures":
                        export3DPictures = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportAnotation":
                        exportAnotation = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportWelds":
                        exportWelds = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportCurves":
                        exportCurves = reader.GetAttribute(1) == "1";
                        break;
                    case "ExportContours":
                        exportContours = reader.GetAttribute(1) == "1";
                        break;
                    case "SimplifyGeometry":
                        simplifyGeometry = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportMode":
                        importMode = int.Parse(reader.GetAttribute(1));
                        break;
                    case "Heal":
                        heal = int.Parse(reader.GetAttribute(1));
                        break;
                    case "CreateAccurateEdges":
                        createAccurateEdges = int.Parse(reader.GetAttribute(1));
                        break;
                    case "ImportSolidBodies":
                        importSolidBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportSheetBodies":
                        importSheetBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportWireBodies":
                        importWireBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportMeshBodies":
                        importMeshBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportHideBodies":
                        importHideBodies = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportAnotations":
                        importAnotations = reader.GetAttribute(1) == "1";
                        break;
                    case "ImportOnlyFromActiveFilter":
                        importOnlyFromActiveFilter = reader.GetAttribute(1) == "1";
                        break;
                    case "Sewing":
                        sewing = reader.GetAttribute(1) == "1";
                        break;
                    case "SewTolerance":
                        sewTolerance = double.Parse(reader.GetAttribute(1), 
                            CultureInfo.InvariantCulture);
                        break;
                    case "CheckImportGeomerty":
                        checkImportGeomerty = reader.GetAttribute(1) == "1";
                        break;
                    case "UpdateProductStructure":
                        updateProductStructure = reader.GetAttribute(1) == "1";
                        break;
                    case "AddBodyRecordsInProductStructure":
                        addBodyRecordsInProductStructure = reader.GetAttribute(1) == "1";
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportMode");
            writer.WriteAttributeString("value", ExportMode.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ColorSource");
            writer.WriteAttributeString("value", ColorSource.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportSolidBodies");
            writer.WriteAttributeString("value", ExportSolidBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportSheetBodies");
            writer.WriteAttributeString("value", ExportSheetBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportWireBodies");
            writer.WriteAttributeString("value", ExportWireBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Export3DPictures");
            writer.WriteAttributeString("value", Export3DPictures ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportAnotation");
            writer.WriteAttributeString("value", ExportAnotation ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportWelds");
            writer.WriteAttributeString("value", ExportWelds ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportCurves");
            writer.WriteAttributeString("value", ExportCurves ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ExportContours");
            writer.WriteAttributeString("value", ExportContours ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "SimplifyGeometry");
            writer.WriteAttributeString("value", SimplifyGeometry ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportMode");
            writer.WriteAttributeString("value", ImportMode.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Heal");
            writer.WriteAttributeString("value", Heal.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "CreateAccurateEdges");
            writer.WriteAttributeString("value", CreateAccurateEdges.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportSolidBodies");
            writer.WriteAttributeString("value", ImportSolidBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportSheetBodies");
            writer.WriteAttributeString("value", ImportSheetBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportWireBodies");
            writer.WriteAttributeString("value", ImportWireBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportMeshBodies");
            writer.WriteAttributeString("value", ImportMeshBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportHideBodies");
            writer.WriteAttributeString("value", ImportHideBodies ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportAnotations");
            writer.WriteAttributeString("value", ImportAnotations ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "ImportOnlyFromActiveFilter");
            writer.WriteAttributeString("value", ImportOnlyFromActiveFilter ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "Sewing");
            writer.WriteAttributeString("value", Sewing ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "SewTolerance");
            writer.WriteAttributeString("value", SewTolerance.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "CheckImportGeomerty");
            writer.WriteAttributeString("value", CheckImportGeomerty ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "UpdateProductStructure");
            writer.WriteAttributeString("value", UpdateProductStructure ? "1" : "0");
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "AddBodyRecordsInProductStructure");
            writer.WriteAttributeString("value", AddBodyRecordsInProductStructure ? "1" : "0");
            writer.WriteEndElement();
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
        
        internal string GetPrototypePath()
        {
            string path = null;
            
            using (TFlex.Configuration.Files files = new TFlex.Configuration.Files())
            {
                path = ImportMode == 2 
                    ? files.Prototype3DName 
                    : files.Prototype3DAssemblyName;
            }

            return path;
        }
        #endregion
    }
}
