using System.Collections.Generic;
using TFlex.Model;

namespace TFlex.PackageManager.UI.Model
{
    /// <summary>
    /// The Processing Item class.
    /// </summary>
    public class ProcItem
    {
        #region private filelds
        int flags;
        #endregion

        /// <summary>
        /// The Processing Item Constructor.
        /// </summary>
        public ProcItem()
        {
            Items = new List<ProcItem>();
            ERefs = new List<ProcItem>();
            Links = new List<ProcItem>();
            Dests = new List<ProcItem>();
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
        public int Flags
        {
            get => flags;
            set
            {
                if (flags != value)
                {
                    flags = value;
                    CfgItems();
                }
            }
        }

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
        /// Unit Path.
        /// </summary>
        public string UPath => IPath ?? OPath;

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
        /// External Links.
        /// </summary>
        public List<ProcItem> Links { get; }

        /// <summary>
        /// Destination Items.
        /// </summary>
        public List<ProcItem> Dests { get; }

        /// <summary>
        /// Processed Pages.
        /// </summary>
        public Dictionary<Page, string> Pages { get; }

        /// <summary>
        /// Parent processing Item.
        /// </summary>
        public ProcItem Parent { get; set; }

        #region private methods
        private void CfgItems()
        {
            if (flags == 0)
            {
                Items.ForEach(i => CfgItems(i));
            }
            else if (Level == 0 && flags == 1)
            {
                Items.ForEach(i => CfgDests(i));
                Links.ForEach(i => CfgDests(i));
            }
            else if (Level == 0 && flags == 4)
            {
                foreach (var item in Items)
                {
                    if (item.Flags != 1)
                        continue;

                    foreach (var link in Links)
                    {
                        if (link.Flags == 0 && link.Parent.Flags == 1)
                        {
                            link.Flags |= 0x1;
                            break;
                        }
                    }
                    CfgLinks(item);
                    break;
                }
            }
            else if (Level == 0 && flags == 5)
            {
                foreach (var link in Links)
                {
                    if (link.Flags == 1)
                    {
                        link.Flags ^= 0x1;
                    }
                }
                Flags ^= 0x5;
            }
        }

        private void CfgItems(ProcItem item)
        {
            //
            // configure items
            //
            if (item.Dests.Count == 0)
                return;

            var dest = item.Dests[0];
            if (!(dest.Flags == 4 && item.Flags == 1))
                return;

            if (dest.ERefs.Count == 1)
            {
                item.Flags ^= 0x1;
                dest.Flags ^= 0x5;
                return;
            }

            int counter = 0;

            foreach (var link in dest.Links)
            {
                if (link.Flags == 1 && link.Parent.Flags == 0)
                {
                    link.Flags ^= 0x1;
                }

                if (link.Flags == 0)
                    counter++;
            }

            if (dest.Links.Count == counter)
            {
                dest.Flags ^= 0x5;
                return;
            }

            foreach (var i in item.Items)
            {
                CfgItems(i); // recursive call
            }
        }

        private void CfgLinks(ProcItem item)
        {
            //
            // configure links
            //
            if (item.Dests.Count == 0)
                return;

            var dest = item.Dests[0];
            if (dest.Flags != 4)
                return;

            foreach (var link in dest.Links)
            {
                if (link.Flags == 0 && link.Parent.Flags == 1)
                {
                    item.Flags ^= 0x1;
                    link.Flags |= 0x1;
                    break;
                }
            }
        }

        private void CfgDests(ProcItem item)
        {
            //
            // configure destination
            //
            if (item.Dests.Count == 0)
                return;

            if (!(item.Flags == 0 && item.Parent.Flags == 1))
                return;

            var dest = item.Dests[0];
            if (dest.Flags == 1)
            {
                dest.Flags ^= 0x5;
                item.Flags |= 0x1;
            }
            else if (dest.Flags == 4 && dest.ERefs.Count > 1)
            {
                item.Flags |= 0x1;
            }
        }
        #endregion
    }
}
