﻿using System.Windows;
using System.Windows.Controls;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// The Custom TreeView class.
    /// </summary>
    public partial class CustomTreeView : TreeView
    {
        public CustomTreeView()
        {
            InitializeComponent();
        }
        
        #region public fields
        public static readonly DependencyProperty FlagsProperty =
            DependencyProperty.Register("Flags",
                typeof(int), typeof(CustomTreeView),
                new FrameworkPropertyMetadata(0));
        #endregion

        #region properties
        /// <summary>
        /// Checkboxes Flags: Visible(0), Collapsed(1)
        /// </summary>
        public int Flags
        {
            get => (int)GetValue(FlagsProperty);
            set => SetValue(FlagsProperty, value);
        }
        #endregion

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeView();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CustomTreeViewItem;
        }
    }
}