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
        /// Item's hierarchy in the tree.
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
        /// The IsNode property.
        /// </summary>
        public bool IsNode
        {
            get => (bool)GetValue(IsNodeProperty);
            set => SetValue(IsNodeProperty, value);
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

        public static readonly DependencyProperty IsNodeProperty =
            DependencyProperty.Register("IsNode",
                typeof(bool),
                typeof(CustomTreeViewItem),
                new PropertyMetadata(false));

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
        private static void OnChecked(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CustomTreeViewItem item))
                return;

            bool? value = (bool?)e.NewValue;
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

            if (item.Parent is CustomTreeViewItem parent)
            {
                if (parent.IsChecked != value)
                {
                    CheckingParent(item);
                }
            }

            if (value == null)
                return;

            foreach (CustomTreeViewItem i in item.Items)
            {
                if (i.IsChecked != value)
                    i.IsChecked = value;
            }
        }

        private static void CheckingParent(CustomTreeViewItem item)
        {
            var parent  = item.Parent as CustomTreeViewItem;
            bool? value = item.IsChecked;

            foreach (CustomTreeViewItem i in parent.Items)
            {
                if (i.IsChecked != item.IsChecked)
                {
                    value = null;
                    break;
                }
            }

            if (parent.IsChecked != value)
                parent.IsChecked = value;
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
