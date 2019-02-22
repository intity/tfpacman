using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// The TreeView class override.
    /// </summary>
    public class TreeListView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private GridViewColumnCollection columns;
        
        /// <summary> 
        /// TreeListView column collection.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get
            {
                if (columns == null)
                {
                    columns = new GridViewColumnCollection();
                }

                return columns;
            }
        }

        /// <summary>
        /// Enables check boxes for items in a tree-view control.
        /// </summary>
        public bool CheckboxesVisible { get; set; }
    }

    /// <summary>
    /// The TreeViewItem class override.
    /// </summary>
    public class TreeListViewItem : TreeViewItem
    {
        private int level = -1;

        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (level == -1)
                {
                    TreeListViewItem parent = ItemsControlFromItemContainer(this) as TreeListViewItem;
                    level = (parent != null) ? parent.Level + 1 : 0;
                }
                return level;
            }
        }

        /// <summary>
        /// The ImageSource property.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", 
                typeof(ImageSource), 
                typeof(TreeListViewItem), 
                new PropertyMetadata(null));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
    }
}