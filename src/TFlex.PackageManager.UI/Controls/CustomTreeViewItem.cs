using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// The TreeViewItem class override.
    /// </summary>
    public class CustomTreeViewItem : TreeViewItem
    {
        #region private fields
        int level = -1;
        #endregion

        #region properties
        /// <summary>
        /// The IsChecked property to checkbox.
        /// </summary>
        public bool? IsChecked
        {
            get => GetValue(IsCheckedProperty) as bool?;
            set => SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Is the item a node.
        /// </summary>
        public bool IsNode
        {
            get => (bool)GetValue(IsNodeProperty);
            set => SetValue(IsNodeProperty, value);
        }

        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (level == -1)
                {
                    level = (ItemsControlFromItemContainer(this) is CustomTreeViewItem parent)
                        ? parent.Level + 1 : 0;
                }
                return level;
            }
        }

        /// <summary>
        /// File extension.
        /// </summary>
        public string Extension
        {
            get => GetValue(ExtensionProperty) as string;
            set => SetValue(ExtensionProperty, value);
        }

        /// <summary>
        /// The ImageSource property.
        /// </summary>
        public ImageSource ImageSource
        {
            get => GetValue(ImageSourceProperty) as ImageSource;
            set => SetValue(ImageSourceProperty, value);
        }
        #endregion

        #region public fields
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached("IsChecked",
                typeof(bool?),
                typeof(CustomTreeViewItem),
                new FrameworkPropertyMetadata((bool?)false));

        public static readonly DependencyProperty IsNodeProperty =
            DependencyProperty.Register("IsNode",
                typeof(bool),
                typeof(CustomTreeViewItem), 
                new PropertyMetadata(false));

        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register("Extension",
                typeof(string),
                typeof(CustomTreeViewItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource",
                typeof(ImageSource),
                typeof(CustomTreeViewItem),
                new PropertyMetadata(null));
        #endregion

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CustomTreeViewItem;
        }
    }
}