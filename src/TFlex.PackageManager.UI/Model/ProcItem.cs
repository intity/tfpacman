using System.Collections.Generic;
using System.Windows.Documents;
using TFlex.Model;
using TFlex.PackageManager.Configuration;

namespace TFlex.PackageManager.Model
{
    /// <summary>
    /// The Processing Item class.
    /// </summary>
    internal class ProcItem
    {
        /// <summary>
        /// The Processing Item Constructor.
        /// </summary>
        /// <param name="path">Input path.</param>
        public ProcItem(string path)
        {
            IPath = path;
            Pages = new Dictionary<Page, string>();
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
        public string Directory { get; set; }

        /// <summary>
        /// Processed Pages.
        /// </summary>
        public Dictionary<Page, string> Pages { get; }

        /// <summary>
        /// The Parent Processing Item.
        /// </summary>
        public ProcItem Parent { get; set; }
    }
}
