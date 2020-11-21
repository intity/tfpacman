using System.Collections.Generic;
using TFlex.Model;

namespace TFlex.PackageManager.UI.Model
{
    /// <summary>
    /// The Processing Item class.
    /// </summary>
    public class ProcItem
    {
        /// <summary>
        /// The Processing Item Constructor.
        /// </summary>
        public ProcItem()
        {
            Items = new List<ProcItem>();
            Links = new List<FileLink>();
            Pages = new Dictionary<Page, string>();
        }

        /// <summary>
        /// Item level in the hierarchy.
        /// </summary>
        public int Level
        {
            get
            {
                int level = 0;
                var parent = Parent;
                while(parent != null)
                {
                    parent = parent.Parent;
                    level++;
                }
                return level;
            }
        }

        /// <summary>
        /// Flags definition:
        ///   0x0 None
        ///   0x1 Is selected Item
        ///   0x2 Is processed Item
        ///   0x4 Processing the subitem only, if no parent item is selected
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// The File Name.
        /// </summary>
        public string FName { get; set; }

        /// <summary>
        /// Input Path the File.
        /// </summary>
        public string IPath { get; set; }

        /// <summary>
        /// Output Path the File.
        /// </summary>
        public string OPath { get; set; }

        /// <summary>
        /// Target directory to Item.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Items to processing.
        /// </summary>
        public List<ProcItem> Items { get; }

        /// <summary>
        /// Processed Links.
        /// </summary>
        public List<FileLink> Links { get; }

        /// <summary>
        /// Processed Pages.
        /// </summary>
        public Dictionary<Page, string> Pages { get; }

        /// <summary>
        /// Parent processing Item.
        /// </summary>
        public ProcItem Parent { get; set; }
    }
}
