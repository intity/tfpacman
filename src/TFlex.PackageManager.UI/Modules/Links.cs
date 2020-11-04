using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using TFlex.PackageManager.UI.Attributes;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Editors;
using TFlex.PackageManager.UI.Model;
using TFlex.PackageManager.UI.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.UI.Configuration
{
    /// <summary>
    /// Links extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.LINKS, 0), Serializable]
    public class Links : Files
    {
        #region private fields
        string linkTemplate;
        readonly char[] pattern;
        #endregion

        public Links()
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

                string name = "LinkTemplate";
                string path = value;
                string error = string.Format(Resources.Strings["md_0:msg_1"][0], 
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

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 1 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "LinkTemplate":
                        linkTemplate = reader.GetAttribute(1);
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "LinkTemplate");
            writer.WriteAttributeString("value", LinkTemplate);
            writer.WriteEndElement();
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Get external link by the link template.
        /// Keywords
        ///   asm  : item.Parent
        ///   cat  : subdirectory
        ///   part : item
        /// Examples
        ///   template 1: {asm}\{part}
        ///   template 2: {asm}\{cat:asm.name}\{part}
        ///   template 3: {asm:1}\{part}
        /// </summary>
        /// <param name="item">The processing Item.</param>
        /// <returns>
        /// Returns external link, if a parent exists. Returns null otherwise.
        /// </returns>
        internal string GetLink(ProcItem item)
        {
            string link = null;
            var matches = Regex.Matches(LinkTemplate, @"\{(.*?)\}");

            if (matches.Count == 0 || matches.Count < 2)
                return null;

            for (int i = 0; i < matches.Count; i++)
            {
                var group = matches[i].Value.Split(':');
                if (i == 0)
                {
                    if (item.Parent == null)
                        return null;
                    if (!matches[i].Value.Contains("asm"))
                        return null;

                    int level = item.Parent.Level;
                    if (group.Length > 1)
                    {
                        var value = group[1].Substring(0, 1);
                        if (value.IsDigit(0))
                            level = int.Parse(value);
                    }

                    if (level != item.Parent.Level)
                        return null;
                }
                
                if (i == 1 && matches[i].Value.Contains("cat"))
                {
                    if (group.Length < 2)
                        return null;
                    if (group[1].Substring(0, 3) != "asm")
                        return null;

                    var g = group[1].Substring(0, group[1].Length - 1).Split('.');
                    if (g.Length > 1)
                    {
                        switch (g[1])
                        {
                            case "name":
                                link = item.Parent.FName;
                                break;
                        }
                    }
                }
                
                if (matches[i].Value.Contains("part"))
                {
                    if (link == null)
                        link = item.FName;
                    else
                        link += "\\" + item.FName;
                    break;
                }
            }
            return link;
        }
        #endregion
    }
}
