using System.Windows;
using System.Windows.Controls;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// The Custom TreeView class.
    /// </summary>
    public class CustomTreeView : TreeView
    {
        #region properties
        /// <summary>
        /// Enables check boxes for items in a tree-view control.
        /// </summary>
        public bool CheckboxesVisible { get; set; }
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