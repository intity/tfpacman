using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Links extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.LINKS, 0)]
    public class Links : Files
    {
        #region private fields
        string linkTemplate;
        XAttribute data_0_1;

        readonly char[] pattern;
        #endregion

        public Links(string ext): base (ext)
        {
            linkTemplate = string.Empty;

            var chars = Path.GetInvalidPathChars();
            pattern = new char[chars.Length + 1];
            for (int i = 0; i < chars.Length; i++)
            {
                pattern[i] = chars[i];
            }
            pattern[chars.Length] = '/';
        }

        #region public properties
        /// <summary>
        /// External Link template to fragment.
        /// </summary>
        [PropertyOrder(0)]
        [CustomCategory(Resources.LINKS, "category0")]
        [CustomDisplayName(Resources.LINKS, "dn0_1")]
        [CustomDescription(Resources.LINKS, "dn0_1")]
        [Editor(typeof(CustomTextBoxEditor), typeof(UITypeEditor))]
        public string LinkTemplate
        {
            get => linkTemplate;
            set
            {
                if (linkTemplate == value)
                    return;

                linkTemplate = value;
                data_0_1.Value = value;

                string name = "LinkTemplate";
                string path = value;
                string error = string.Format(Resources.Strings["link:msg_1"][0], 
                    pattern.ToString(""));

                foreach (Match m in Regex.Matches(value, @"\{(.*?)\}"))
                {
                    path = path.Replace(m.Value, "");
                }
                if (path.IsValid(pattern))
                {
                    RemoveError(name, error);
                }
                else
                {
                    AddError(name, error);
                }
                OnPropertyChanged(name);
            }
        }
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();
            data_0_1 = new XAttribute("value", LinkTemplate);
            data.Add(new XElement("parameter",
                new XAttribute("name", "LinkTemplate"),
                data_0_1));
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);
            var a = element.Attribute("value");
            switch (element.Attribute("name").Value)
            {
                case "LinkTemplate":
                    linkTemplate = a.Value;
                    data_0_1 = a;
                    break;
            }
        }
        #endregion
    }
}
