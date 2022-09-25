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
            ERefs = new List<ProcItem>();
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
                while (parent != null)
                {
                    parent = parent.Parent;
                    level++;
                }
                return level;
            }
        }

        /// <summary>
        /// <term>0x0</term> None<br/>
        /// <term>0x1</term> Is selected Item<br/>
        /// <term>0x2</term> Reserved<br/>
        /// <term>0x4</term> Marked as selected item from links
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
        /// Target directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Items to processing.
        /// </summary>
        public List<ProcItem> Items { get; }

        /// <summary>
        /// External references.
        /// </summary>
        public List<ProcItem> ERefs { get; }

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
