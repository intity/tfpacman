using System.Collections.Generic;
using System.IO;
using TFlex.Model;
using TFlex.PackageManager.Configuration;

namespace TFlex.PackageManager.Model
{
    /// <summary>
    /// The Processing Item class.
    /// </summary>
    public class ProcItem
    {
        /// <summary>
        /// The Processing Item Constructor.
        /// </summary>
        /// <param name="cfg">Input path.</param>
        /// <param name="path">Input path.</param>
        public ProcItem(Header cfg, string path)
        {
            IPath = path;
            Pages = new Dictionary<Page, string>();
            Items = new List<ProcItem>();

            var FInfo = new FileInfo(IPath);
            Directory = FInfo.Directory.FullName
                .Replace(cfg.InitialCatalog, cfg.TargetDirectory);
        }

        /// <summary>
        /// The File Name.
        /// </summary>
        public string FName { get; set; }

        /// <summary>
        /// Input Path the File.
        /// </summary>
        public string IPath { get; }

        /// <summary>
        /// Output Path the File.
        /// </summary>
        public string OPath { get; set; }

        /// <summary>
        /// Target directory to Item.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Processed Pages.
        /// </summary>
        public Dictionary<Page, string> Pages { get; }

        /// <summary>
        /// The Items.
        /// </summary>
        public List<ProcItem> Items { get; }

        /// <summary>
        /// Set directory path.
        /// </summary>
        /// <param name="path"></param>
        public void SetDirectory(string path)
        {
            Directory = path;
        }
    }
}
