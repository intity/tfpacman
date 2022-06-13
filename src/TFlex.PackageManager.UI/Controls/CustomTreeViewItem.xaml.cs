using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// The TreeViewItem class override.
    /// </summary>
    public partial class CustomTreeViewItem : TreeViewItem
    {
        #region private fields
        int level = -1;
        #endregion

        public CustomTreeViewItem()
        {
            InitializeComponent();
        }

        #region properties
        /// <summary>
        /// The IsChecked property to CheckBox control.
        /// </summary>
        public bool? IsChecked
        {
            get => GetValue(IsCheckedProperty) as bool?;
            set => SetValue(IsCheckedProperty, value);
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
            DependencyProperty.Register("IsChecked",
                typeof(bool?),
                typeof(CustomTreeViewItem),
                new FrameworkPropertyMetadata(false, 
                    new PropertyChangedCallback(OnChecked)));

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

        #region events
        public static readonly RoutedEvent CheckedEvent = 
            EventManager.RegisterRoutedEvent("Checked",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CustomTreeViewItem));

        public static readonly RoutedEvent UncheckedEvent = 
            EventManager.RegisterRoutedEvent("Unchecked",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CustomTreeViewItem));

        public static readonly RoutedEvent IndeterminateEvent = 
            EventManager.RegisterRoutedEvent("Indeterminate",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CustomTreeViewItem));

        public event RoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        public event RoutedEventHandler Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        public event RoutedEventHandler Indeterminate
        {
            add { AddHandler(IndeterminateEvent, value); }
            remove { RemoveHandler(IndeterminateEvent, value); }
        }
        #endregion

        #region private methods
        private static object tmp;
        private static void OnChecked(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CustomTreeViewItem item))
                return;

            bool? value = (bool?)e.NewValue;

            Debug.WriteLine($"OnChecked [item:{item}, value:{value}]");

            RoutedEventArgs args = new RoutedEventArgs();

            if (value == true)
            {
                args.RoutedEvent = CheckedEvent;
            }
            else if (value == false)
            {
                args.RoutedEvent = UncheckedEvent;
            }
            else
            {
                args.RoutedEvent = IndeterminateEvent;
            }

            item.RaiseEvent(args);

            if (value == null)
                return;

            foreach (CustomTreeViewItem i in item.Items)
            {
                if (tmp == null)
                    tmp = item;
                
                i.IsChecked = value;
            }

            CheckingToParent(item);

            tmp = null;
        }

        private static void CheckingToParent(CustomTreeViewItem item)
        {
            bool? value = item.IsChecked;

            if (item.Parent is CustomTreeViewItem parent)
            {
                if (!parent.Equals(tmp))
                {
                    foreach (CustomTreeViewItem i in parent.Items)
                    {
                        if (i.IsChecked != item.IsChecked)
                        {
                            value = null;
                            break;
                        }
                    }
                    parent.IsChecked = value;
                }
                CheckingToParent(parent); // recursive call
            }
        }
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
