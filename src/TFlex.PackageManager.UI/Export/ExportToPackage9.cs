using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Export
{
    [CustomCategoryOrder(Resource.PACKAGE_9, 4)]
    public class ExportToPackage9 : ExportTo
    {
        #region private field
        private bool export3dModel;

        private readonly byte[] objState = new byte[1];
        private readonly bool[] b_values = new bool[1];
        private bool isChanged;
        #endregion

        public ExportToPackage9(Header header) : base (header)
        {
            export3dModel   = false;
            OutputExtension = "PDF";
        }

        #region internal properties
        internal override bool IsChanged
        {
            get
            {
                return (isChanged ? true : base.IsChanged);
            }
        }
        #endregion

        #region public properies
        [PropertyOrder(18)]
        [CustomCategory(Resource.PACKAGE_9, "category4")]
        [CustomDisplayName(Resource.PACKAGE_9, "dn4_1")]
        [CustomDescription(Resource.PACKAGE_9, "dn4_1")]
        [DefaultValue(false)]
        public bool Export3dModel
        {
            get { return export3dModel; }
            set
            {
                if (export3dModel != value)
                {
                    export3dModel = value;
                    OnChanged(15);
                }
            }
        }
        #endregion

        #region methods
        public override void OnLoaded()
        {
            base.OnLoaded();

            b_values[0] = export3dModel;

            for (int i = 0; i < objState.Length; i++)
                objState[i] = 0;
        }

        public override void OnChanged(int index)
        {
            bool result = false;
            if (!IsLoaded) return;

            switch (index)
            {
                case 15: objState[0] = (byte)(export3dModel ? 1 : 0);
                    break;
            }

            foreach (var i in objState)
            {
                if (i > 0)
                {
                    result = true;
                    break;
                }
            }

            isChanged = result;
            base.OnChanged(index);
        }

        public override bool Export(Document document, Page page, string path)
        {
            ExportToPDF export = new ExportToPDF(document)
            {
                IsSelectPagesDialogEnabled = false,
                OpenExportFile = false,
                Export3DModel = export3dModel
            };

            export.ExportPages.Add(page);

            return export.Export(path);
        }

        internal override void AppendPackageToXml(XElement parent, PackageType package)
        {
            base.AppendPackageToXml(parent, package);

            string value = Enum.GetName(typeof(PackageType), package);
            parent.Elements().Where(p => p.Attribute("id").Value == value).First().Add(
                new XElement("parameter",
                    new XAttribute("name", "Export3dModel"),
                    new XAttribute("value", export3dModel)));
        }

        internal override void PackageTask(XElement element, int flag)
        {
            base.PackageTask(element, flag);

            string value = element.Attribute("value").Value;
            switch (element.Attribute("name").Value)
            {
                case "Export3dModel":
                    if (flag == 0)
                        export3dModel = value == "1" ? true : false;
                    else
                        value = export3dModel ? "1" : "0";
                    break;
            }
            element.Attribute("value").Value = value;
        }
        #endregion
    }
}
