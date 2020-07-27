using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TFlex.PackageManager.Model;
using TFlex.PackageManager.Modules;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Package class.
    /// </summary>
    internal class Package
    {
        #region private fields
        readonly Header cfg;
        #endregion

        /// <summary>
        /// The Package constructor.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="items">Selected Items.</param>
        public Package(Header cfg, string[] items)
        {
            this.cfg = cfg;
            var mode = (cfg.Translator as Translator).PMode;
            Items    = new Dictionary<ProcItem, string[]>();

            if (mode != ProcessingMode.Import)
                InitPackage_0(items);
            else
                InitPackage_I(items);
        }

        /// <summary>
        /// Package Items.
        /// </summary>
        public Dictionary<ProcItem, string[]> Items { get; }

        #region private methods
        /// <summary>
        /// Initialize processing Items to parent.
        /// </summary>
        /// <param name="items">Selected Items.</param>
        /// <param name="item">Parent processing Item.</param>
        /// <param name="links">External links.</param>
        private void InitItems(string[] items, ProcItem item, string[] links)
        {
            foreach (var i in items)
            {
                if (item.IPath != i && links.Contains(i))
                {
                    item.Items.Add(new ProcItem(cfg, i));
                }
            }
        }

        /// <summary>
        /// Initialize package to processing documents.
        /// </summary>
        /// <param name="items">Selected Items.</param>
        private void InitPackage_0(string[] items)
        {
            foreach (var i in items)
            {
                var links = Application.GetDocumentExternalFileLinks(i, true, false, true);
                Items.Add(new ProcItem(cfg, i), links);
            }

            foreach (var i in Items)
            {
                InitItems(items, i.Key, i.Value);
            }
        }

        /// <summary>
        /// Initialize package to processing import files.
        /// </summary>
        /// <param name="items">Selected Items.</param>
        private void InitPackage_I(string[] items)
        {
            var ext = "*." + (cfg.Translator as Files).TargetExtension.ToLower();
            var files = Directory.GetFiles(cfg.InitialCatalog, ext, 
                SearchOption.AllDirectories);

            foreach (var i in items)
            {
                if (files.Contains(i))
                {
                    Items.Add(new ProcItem(cfg, i), null);
                }
            }
        }
        #endregion
    }
}