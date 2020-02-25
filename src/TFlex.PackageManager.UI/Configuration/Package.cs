using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TFlex.PackageManager.Model;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Package class.
    /// </summary>
    internal class Package
    {
        #region private fields
        private readonly Header header;
        private readonly string mode;
        private readonly string path;
        private XDocument data;
        private XElement docs;
        #endregion

        /// <summary>
        /// The Package constructor.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="t_mode"></param>
        public Package(Header header, TranslatorType t_mode)
        {
            this.header = header;

            mode = Enum.GetName(typeof(TranslatorType), t_mode);
            path = Path.Combine(header.TargetDirectory, mode, "package.xml");

            switch (t_mode)
            {
                case TranslatorType.Document: InitPackage_0(); break;
            }
        }

        /// <summary>
        /// The Package Items.
        /// </summary>
        public IEnumerable<XElement> Items { get => docs.Elements(); }

        #region internal methods
        /// <summary>
        /// Set metadata to XML.
        /// </summary>
        /// <param name="item">The Processing Item.</param>
        internal void SetMetadata(ProcItem item)
        {
            var element = docs.Elements()
                .Where(e => e.Attribute("path").Value == item.IPath)
                .FirstOrDefault();

            var aPath = element?.Element("output")?.Attribute("path");
            if (aPath != null)
            {
                if (File.Exists(aPath.Value) && aPath.Value != item.OPath)
                    File.Delete(aPath.Value);

                aPath.Value = item.OPath;
            }
            else if (element?.Element("output") == null)
                element.Add(new XElement("output", new XAttribute("path", item.OPath)));

            data.Save(path);
        }

        /// <summary>
        /// Get parent name.
        /// </summary>
        /// <param name="item">The Processing Item.</param>
        /// <returns>Parent file name.</returns>
        internal string GetParentName(ProcItem item)
        {
            foreach (var e in docs.Elements())
            {
                var links = e.Element("links");
                if (links == null)
                    continue;

                var link = links.Elements()
                    .Where(p => p.Attribute("path").Value == item.IPath)
                    .FirstOrDefault();

                if (link != null)
                {
                    var iPath = e.Attribute("path").Value;
                    var oPath = e.Element("output")?.Attribute("path").Value;
                    var aPath = oPath != null ? oPath.Split('\\') : iPath.Split('\\');
                    var pName = aPath[aPath.Length - 1].Replace(".grb", "");

                    if (item.Parent == null)
                    {
                        item.Parent = new ProcItem(iPath)
                        {
                            OPath = oPath,
                            FName = pName
                        };
                    }

                    return pName;
                }
            }

            return null;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Get the links element for XML data.
        /// </summary>
        /// <param name="path">Document file name path.</param>
        /// <returns>The links element data.</returns>
        private XElement GetLinksElement(string path)
        {
            string[] a_links = TFlex.Application.GetDocumentExternalFileLinks(path, true, false, true);
            XElement e_links = new XElement("links");

            foreach (var i in a_links)
            {
                e_links.Add(new XElement("link", new XAttribute("path", i)));
            }

            return e_links.Elements().Count() > 0 ? e_links : null;
        }

        /// <summary>
        /// The CreateElement helper method.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private XElement CreateElement(string path)
        {
            var element = new XElement("document", 
                new XAttribute("path", path),
                GetLinksElement(path));
            return element;
        }

        /// <summary>
        /// Initialize Package for the Document mode.
        /// </summary>
        private void InitPackage_0()
        {
            string[] paths = Directory.GetFiles(header.InitialCatalog, "*.grb",
                SearchOption.AllDirectories);

            if (File.Exists(path))
            {
                data = XDocument.Load(path);
                docs = data.Element("package").Element("documents");
                var src = data.Element("package").Attribute("src");
                if (src.Value != header.InitialCatalog)
                {
                    src.Value = header.InitialCatalog;
                    docs.Elements().Remove();

                    foreach (var p in paths)
                    {
                        docs.Add(CreateElement(p));
                    }

                    data.Save(path);
                    return;
                }

                foreach (var p in paths)
                {
                    var element = docs.Elements()
                        .Where(e => e.Attribute("path").Value == p)
                        .FirstOrDefault();
                    if (element != null)
                        continue;

                    docs.Add(CreateElement(p));
                }

                data.Save(path);
                return;
            }
            else
            {
                docs = new XElement("documents");
                data = new XDocument(new XDeclaration("1.0", "utf-8", null),
                    new XElement("package", 
                    new XAttribute("type", mode), 
                    new XAttribute("src", header.InitialCatalog), docs));
            }

            foreach (var p in paths)
            {
                docs.Add(CreateElement(p));
            }

            data.Save(path);
        }
        #endregion
    }
}