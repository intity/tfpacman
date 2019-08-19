﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TFlex.Model;

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
        private readonly XDocument data;
        private readonly XElement elms;
        #endregion

        public Package(Header header, string[] si, TranslatorType t_mode)
        {
            this.header = header;

            mode = Enum.GetName(typeof(TranslatorType), t_mode);
            path = Path.Combine(header.TargetDirectory, mode, "package.xml");
            elms = new XElement("elements");
            data = new XDocument(new XDeclaration("1.0", "utf-8", null), 
                   new XElement("package", new XAttribute("type", mode), elms));

            if (File.Exists(path))
                File.Delete(path);

            switch (t_mode)
            {
                case TranslatorType.Document: InitPackage_0(si); break;
            }

            data.Save(path);
        }

        #region internal methods
        /// <summary>
        /// Set attributes to XML-data.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="i_path">Input file name path.</param>
        /// <param name="o_path">Output file name path.</param>
        internal void SetAttributes(string name, string i_path, string o_path)
        {
            var id = ConvertPathToID(i_path);
            var ip = i_path.Replace(header.InitialCatalog + "\\", "");
            var op = o_path.Replace(header.TargetDirectory + "\\" + mode + "\\", "");

            var element = elms.Elements()
                .Where(e => e.Attribute("id").Value == id)
                .FirstOrDefault();

            if (element == null)
                return;

            var old_path = element.Attribute("path").Value;
            if (old_path != i_path && File.Exists(old_path))
            {
                File.Delete(old_path);
            }

            element.Attribute("name").Value = name;
            element.Attribute("path").Value = op;

            data.Save(path);
        }

        /// <summary>
        /// Get parent name.
        /// </summary>
        /// <param name="path">Input file name path.</param>
        /// <returns>Parent file name.</returns>
        internal string GetParentName(string path)
        {
            var id = ConvertPathToID(path);

            foreach (var e in elms.Elements())
            {
                var links = e.Element("links");
                if (links == null)
                    continue;

                var link = links.Elements()
                    .Where(p => p.Attribute("id").Value == id)
                    .FirstOrDefault();

                if (link != null)
                {
                    return e.Attribute("name").Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Get parent path.
        /// </summary>
        /// <param name="path">Input file name path.</param>
        /// <returns>Parent file name path.</returns>
        internal string GetParentPath(string path)
        {
            var id = ConvertPathToID(path);

            foreach (var e in elms.Elements())
            {
                var links = e.Element("links");
                if (links == null)
                    continue;

                var link = links.Elements()
                    .Where(p => p.Attribute("id").Value == id)
                    .FirstOrDefault();

                if (link != null)
                {
                    return Path.Combine(header.TargetDirectory, mode, e.Attribute("path").Value);
                }
            }

            return null;
        }

        /// <summary>
        /// Replace link of fragment.
        /// </summary>
        /// <param name="i_path">Input file name path.</param>
        /// <param name="o_path">Output file name path.</param>
        internal void ReplaceLink(string i_path, string o_path)
        {
            var pp = GetParentPath(i_path);
            if (pp == null)
                return;

            var id = ConvertPathToID(i_path);
            var ip = i_path.Replace(header.InitialCatalog + "\\", "");
            var op = o_path.Replace(header.TargetDirectory + "\\" + mode + "\\", "");

            foreach (var e in elms.Elements())
            {
                var links = e.Element("links");
                if (links == null)
                    continue;

                var link = links.Elements()
                    .Where(p => p.Attribute("id").Value == id)
                    .FirstOrDefault();

                if (link == null)
                    return;

                var document = TFlex.Application.OpenDocument(pp, false);
                if (document == null)
                    return;

                document.BeginChanges(string.Format("Replace link to: {0}", op));

                foreach (var i in document.FileLinks)
                {
                    if (i.FilePath.Contains(ip))
                    {
                        i.FilePath = op;
                        document.Regenerate(new RegenerateOptions { UpdateAllLinks = true });
                    }
                }

                if (document.Changed)
                {
                    document.EndChanges();
                    document.Save();
                }
                else
                    document.CancelChanges();

                document.Close();
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Convert path to ID.
        /// </summary>
        /// <param name="path">Input file name path.</param>
        /// <returns>Returns ID to GUID format.</returns>
        private string ConvertPathToID(string path)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(path));
                return new Guid(hash).ToString("D").ToUpper();
            }
        }

        /// <summary>
        /// Initialize package Document.
        /// </summary>
        /// <param name="si">Selected paths to items.</param>
        private void InitPackage_0(string[] si)
        {
            foreach (var e in si)
            {
                string[] a_group = e.Split('\\');
                string[] a_links = TFlex.Application.GetDocumentExternalFileLinks(e, true, false, true);
                XElement e_links = new XElement("links");
                XElement element = new XElement("element", 
                    new XAttribute("id", ConvertPathToID(e)), 
                    new XAttribute("name", a_group[a_group.Length - 1].Replace(".grb", "")), 
                    new XAttribute("path", e.Replace(header.InitialCatalog + "\\", "")));
                elms.Add(element);

                foreach (var i in a_links)
                {
                    if (si.Contains(i))
                    {
                        string ip = i.Replace(header.InitialCatalog + "\\", "");
                        e_links.Add(new XElement("link", new XAttribute("id", ConvertPathToID(i))));
                    }
                }

                if (e_links.Elements().Count() > 0)
                    element.Add(e_links);
            }
        }
        #endregion
    }
}