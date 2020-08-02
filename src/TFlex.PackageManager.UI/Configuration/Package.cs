using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TFlex.PackageManager.Model;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Package class.
    /// </summary>
    internal class Package
    {
        /// <summary>
        /// The Package constructor.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="items">Selected Items.</param>
        public Package(Header cfg, string[] items)
        {
            Items = new Dictionary<ProcItem, int>();
            InitPackage(cfg, items);
        }

        /// <summary>
        /// Package Items.
        /// </summary>
        public Dictionary<ProcItem, int> Items { get; }

        #region private methods
        /// <summary>
        /// Initialize the package Items.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="items">Selected Items.</param>
        private void InitPackage(Header cfg, string[] items)
        {
            var ext = "*.grb";
            var opt = SearchOption.AllDirectories;
            var mode = (cfg.Translator as Translator).PMode;
            if (mode == ProcessingMode.Import)
            {
                ext = "*." + (cfg.Translator as Files).TargetExtension.ToLower();
            }
            var files = Directory.GetFiles(cfg.InitialCatalog, ext, opt);
            foreach (var p in files)
            {
                if (Contains(p))
                    continue;
                int flags = 0x0;
                if (items.Contains(p))
                    flags |= 0x1;
                var item = new ProcItem(p)
                {
                    Directory = GetDirectory(cfg, p)
                };
                if (ext == "*.grb")
                {
                    InitItems(cfg, item, items);
                }
                Items.Add(item, flags);
            }
        }

        /// <summary>
        /// Initialize the tree Items to parent.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="item">Parent Item.</param>
        /// <param name="items">Selected Items.</param>
        private void InitItems(Header cfg, ProcItem item, string[] items)
        {
            string[] links = Application
                .GetDocumentExternalFileLinks(item.IPath, true, false, false);

            foreach (var link in links)
            {
                int flags = 0x0;
                if (items.Contains(link))
                    flags |= 0x1;
                var subItem = new ProcItem(link)
                {
                    Directory = GetDirectory(cfg, link),
                    Parent = item
                };
                item.Items.Add(subItem, flags);
                InitItems(cfg, subItem, items);
            }
        }

        /// <summary>
        /// Get target directory from path.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetDirectory(Header cfg, string path)
        {
            return Path.GetDirectoryName(path)
                .Replace(cfg.InitialCatalog, cfg.TargetDirectory);
        }

        private bool Contains(string path)
        {
            foreach (var i in Items)
            {
                if (i.Key.IPath == path)
                    return true;
                if (Contains(i.Key, path))
                    return true;
            }
            return false;
        }

        private bool Contains(ProcItem item, string path)
        {
            foreach (var i in item.Items)
            {
                if (i.Key.IPath == path)
                    return true;
                Contains(i.Key, path);
            }
            return false;
        }
        #endregion
    }
}
