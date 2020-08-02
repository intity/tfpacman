﻿using System;
using System.Collections.Generic;
using TFlex.Model;

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
        /// <param name="path">Input path.</param>
        public ProcItem(string path)
        {
            IPath = path;
            Pages = new Dictionary<Page, string>();
            Items = new Dictionary<ProcItem, int>();
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
        /// Items to processing.
        /// Flags
        ///   0x0 None
        ///   0x1 Is selected Item
        ///   0x2 Is processed Item
        /// </summary>
        public Dictionary<ProcItem, int> Items { get; }

        /// <summary>
        /// Parent processing Item.
        /// </summary>
        public ProcItem Parent { get; set; }
    }
}
