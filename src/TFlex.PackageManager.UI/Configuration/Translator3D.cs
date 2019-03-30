using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Base class for export to some universal 3D formats (STEP, ACIS, JT, PRC, 3MF, IGES).
    /// </summary>
    [CustomCategoryOrder(Resource.TRANSLATOR_3D, 4)]
    public class Translator3D : Category_3
    {
        #region private fields
        private int mode;
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

        private readonly byte[] objState;
        private readonly int[] i_values;
        private readonly bool[] b_values;
        private bool isChanged;
        #endregion

        public Translator3D()
        {
            colorSource       = 1;
            exportSheetBodies = true;
            exportSolidBodies = true;
            exportWireBodies  = true;

            objState          = new byte[11];
            i_values          = new int[2];
            b_values          = new bool[9];
        }

        #region public properties
        /// <summary>
        /// Export type.
        /// </summary>
        [PropertyOrder(17)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_1")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_1")]
        [ItemsSource(typeof(ModeItems))]
        public int Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    OnChanged(17);
                }
            }
        }

        [PropertyOrder(18)]
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
                    OnChanged(18);
                }
            }
        }

        /// <summary>
        /// Export 3D images.
        /// </summary>
        [PropertyOrder(19)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_3")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_3")]
        public bool Export3DPictures
        {
            get { return export3DPictures; }
            set
            {
                if (export3DPictures != value)
                {
                    export3DPictures = value;
                    OnChanged(19);
                }
            }
        }

        /// <summary>
        /// Export annotations.
        /// </summary>
        [PropertyOrder(20)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_4")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_4")]
        public bool ExportAnotation
        {
            get { return exportAnotation; }
            set
            {
                if (exportAnotation != value)
                {
                    exportAnotation = value;
                    OnChanged(20);
                }
            }
        }

        /// <summary>
        /// Export profiles.
        /// </summary>
        [PropertyOrder(21)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_5")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_5")]
        public bool ExportContours
        {
            get { return exportContours; }
            set
            {
                if (exportContours != value)
                {
                    exportContours = value;
                    OnChanged(21);
                }
            }
        }

        /// <summary>
        /// Export curves.
        /// </summary>
        [PropertyOrder(22)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_6")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_6")]
        public bool ExportCurves
        {
            get { return exportCurves; }
            set
            {
                if (exportCurves != value)
                {
                    exportCurves = value;
                    OnChanged(22);
                }
            }
        }

        /// <summary>
        /// Export sheet bodies.
        /// </summary>
        [PropertyOrder(23)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_7")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_7")]
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
        /// Export solid bodies.
        /// </summary>
        [PropertyOrder(24)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_8")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_8")]
        public bool ExportSolidBodies
        {
            get { return exportSolidBodies; }
            set
            {
                if (exportSolidBodies != value)
                {
                    exportSolidBodies = value;
                    OnChanged(24);
                }
            }
        }

        /// <summary>
        /// Export welds.
        /// </summary>
        [PropertyOrder(25)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_9")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_9")]
        public bool ExportWelds
        {
            get { return exportWelds; }
            set
            {
                if (exportWelds != value)
                {
                    exportWelds = value;
                    OnChanged(25);
                }
            }
        }

        /// <summary>
        /// Export wire bodies.
        /// </summary>
        [PropertyOrder(26)]
        [CustomCategory(Resource.TRANSLATOR_3D, "category4")]
        [CustomDisplayName(Resource.TRANSLATOR_3D, "dn4_10")]
        [CustomDescription(Resource.TRANSLATOR_3D, "dn4_10")]
        public bool ExportWireBodies
        {
            get { return exportWireBodies; }
            set
            {
                if (exportWireBodies != value)
                {
                    exportWireBodies = value;
                    OnChanged(26);
                }
            }
        }

        /// <summary>
        /// Simplify geometry.
        /// </summary>
        [PropertyOrder(27)]
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
                    OnChanged(27);
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
            i_values[0] = mode;
            i_values[1] = colorSource;
            b_values[0] = export3DPictures;
            b_values[1] = exportAnotation;
            b_values[2] = exportContours;
            b_values[3] = exportCurves;
            b_values[4] = exportSheetBodies;
            b_values[5] = exportSolidBodies;
            b_values[6] = exportWelds;
            b_values[7] = exportWireBodies;
            b_values[8] = simplifyGeometry;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;

            base.OnLoaded();
        }

        internal override void OnChanged(int index)
        {
            if (!IsLoaded) return;

            switch (index)
            {
                case 17: objState[00] = (byte)(i_values[0] != mode              ? 1 : 0); break;
                case 18: objState[01] = (byte)(i_values[1] != colorSource       ? 1 : 0); break;
                case 19: objState[02] = (byte)(b_values[0] != export3DPictures  ? 1 : 0); break;
                case 20: objState[03] = (byte)(b_values[1] != exportAnotation   ? 1 : 0); break;
                case 21: objState[04] = (byte)(b_values[2] != exportContours    ? 1 : 0); break;
                case 22: objState[05] = (byte)(b_values[3] != exportCurves      ? 1 : 0); break;
                case 23: objState[06] = (byte)(b_values[4] != exportSheetBodies ? 1 : 0); break;
                case 24: objState[07] = (byte)(b_values[5] != exportSolidBodies ? 1 : 0); break;
                case 25: objState[08] = (byte)(b_values[6] != exportWelds       ? 1 : 0); break;
                case 26: objState[09] = (byte)(b_values[7] != exportWireBodies  ? 1 : 0); break;
                case 27: objState[10] = (byte)(b_values[8] != simplifyGeometry  ? 1 : 0); break;
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

        internal override void TranslatorTask(XElement element, int flag)
        {
            base.TranslatorTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "Mode":
                    if (flag == 0)
                        mode = int.Parse(value);
                    else
                        value = mode.ToString();
                    break;
                case "ColorSource":
                    if (flag == 0)
                        colorSource = int.Parse(value);
                    else
                        value = colorSource.ToString();
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
                case "ExportContours":
                    if (flag == 0)
                        exportContours = value == "1";
                    else
                        value = exportContours ? "1" : "0";
                    break;
                case "ExportCurves":
                    if (flag == 0)
                        exportCurves = value == "1";
                    else
                        value = exportCurves ? "1" : "0";
                    break;
                case "ExportSheetBodies":
                    if (flag == 0)
                        exportSheetBodies = value == "1";
                    else
                        value = exportSheetBodies ? "1" : "0";
                    break;
                case "ExportSolidBodies":
                    if (flag == 0)
                        exportSolidBodies = value == "1";
                    else
                        value = exportSolidBodies ? "1" : "0";
                    break;
                case "ExportWelds":
                    if (flag == 0)
                        exportWelds = value == "1";
                    else
                        value = exportWelds ? "1" : "0";
                    break;
                case "ExportWireBodies":
                    if (flag == 0)
                        exportWireBodies = value == "1";
                    else
                        value = exportWireBodies ? "1" : "0";
                    break;
                case "SimplifyGeometry":
                    if (flag == 0)
                        simplifyGeometry = value == "1";
                    else
                        value = simplifyGeometry ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }

    internal class ModeItems : IItemsSource
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
}
