using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for TreeViewControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        #region private fields
        readonly object dummyNode;
        string searchPattern;
        string rootDirectory;
        ImageSource tempImage;
        CustomTreeView treeView;
        #endregion

        public ExplorerControl()
        {
            InitializeComponent();

            dummyNode     = null;
            rootDirectory = null;
            SelectedItems = new ObservableCollection<object>();
        }

        #region public properties
        /// <summary>
        /// Total count files.
        /// </summary>
        public int CountFiles { get; private set; }

        /// <summary>
        /// Search pattern to in GetFile method use.
        /// </summary>
        public string SearchPattern
        {
            get => searchPattern;
            set
            {
                if (searchPattern != value)
                {
                    searchPattern = value;
                    UpdateControl();
                }
            }
        }

        /// <summary>
        /// Root directory.
        /// </summary>
        public string RootDirectory
        {
            get => rootDirectory;
            set
            {
                if (rootDirectory != value)
                {
                    rootDirectory = value;
                    UpdateControl();
                }
            }
        }

        /// <summary>
        /// Selected items.
        /// </summary>
        public ObservableCollection<object> SelectedItems { get; }
        #endregion

        #region public methods
        /// <summary>
        /// Update the layout control.
        /// </summary>
        public void UpdateControl()
        {
            if (treeView == null)
                treeView = Content as CustomTreeView;

            if (treeView == null || rootDirectory == null)
                return;

            if (treeView.Items.Count > 0)
                treeView.Items.Clear();

            if (SelectedItems.Count > 0)
                SelectedItems.Clear();

            if (Directory.Exists(rootDirectory))
            {
                GetDirectories();
                GetFiles();
                CountFiles = Directory.GetFiles(rootDirectory, 
                    SearchPattern,
                    SearchOption.AllDirectories).Length;
            }
            else
                CountFiles = 0;

            treeView.UpdateLayout();
            //Debug.WriteLine("UpdateControl");
        }

        /// <summary>
        /// Clean the root directory.
        /// </summary>
        public void CleanRootDirectory()
        {
            if (!Directory.Exists(rootDirectory))
                return;

            DirectoryInfo di = new DirectoryInfo(rootDirectory);

            foreach (var i in di.GetFiles()) i.Delete();
            foreach (var i in di.GetDirectories()) i.Delete(true);

            UpdateControl();
        }
        #endregion

        #region private methods
        private void GetDirectories(CustomTreeViewItem item = null)
        {
            string directory;

            if (item != null)
            {
                item.Items.Clear();
                directory = item.Tag.ToString();
            }
            else
                directory = rootDirectory;

            if (directory == null || treeView == null)
                return;

            foreach (var i in Directory.GetDirectories(directory))
            {
                var subitem = new CustomTreeViewItem
                {
                    Header = i.Substring(i.LastIndexOf("\\") + 1),
                    IsNode = true,
                    Tag = i
                };

                subitem.Items.Add(dummyNode);
                subitem.Expanded += Folder_Expanded;

                if (item != null)
                    item.Items.Add(subitem);
                else
                    treeView.Items.Add(subitem);
            }
        }

        private void GetFiles(CustomTreeViewItem item = null)
        {
            var directory = item != null 
                ? item.Tag.ToString() 
                : rootDirectory;

            if (directory == null || treeView == null)
                return;

            SearchOption option = SearchOption.TopDirectoryOnly;
            foreach (var i in Directory.GetFiles(directory, SearchPattern, option))
            {
                var subitem = new CustomTreeViewItem
                {
                    Header = Path.GetFileName(i),
                    Extension = Path.GetExtension(i),
                    Tag = i
                };

                subitem.Selected   += TreeViewItem_Selected;
                subitem.Unselected += TreeViewItem_Unselected;

                if (item != null)
                    item.Items.Add(subitem);
                else
                    treeView.Items.Add(subitem);
            }
        }

        private void IsChecked(CustomTreeViewItem item)
        {
            bool? value = item.IsChecked;

            if (item.IsNode)
            {
                if (item.IsExpanded == false)
                {
                    item.IsExpanded = true;
                    item.UpdateLayout();
                    item.IsExpanded = false;
                }

                foreach (CustomTreeViewItem i in item.Items)
                {
                    i.IsChecked = value;
                }
            }
            else
            {
                if (item.IsChecked == true)
                {
                    SelectedItems.Add(item.Tag);
                }
                else
                {
                    SelectedItems.Remove(item.Tag);
                }
            }

            if (item.Parent != null && item.Parent is CustomTreeViewItem parent)
            {
                foreach (CustomTreeViewItem i in parent.Items)
                {
                    if (i.IsChecked != value)
                    {
                        value = null;
                        break;
                    }
                }
                parent.IsChecked = value;
            }
        }
        #endregion

        #region event handlers
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.TemplatedParent is CustomTreeViewItem item)
            {
                IsChecked(item);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.TemplatedParent is CustomTreeViewItem item)
            {
                IsChecked(item);
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is CustomTreeViewItem item)
            {
                tempImage = item.ImageSource;
                var image = item.ImageSource as BitmapImage;
                int stride = (image.PixelWidth * image.Format.BitsPerPixel + 7) / 8;
                int length = stride * image.PixelHeight;
                byte[] data = new byte[length];

                image.CopyPixels(data, stride, 0);

                for (int i = 0; i < length; i += 4)
                {
                    data[i + 0] = 255; // R
                    data[i + 1] = 255; // G
                    data[i + 2] = 255; // B
                }

                item.ImageSource = BitmapSource.Create(
                    image.PixelWidth,
                    image.PixelHeight,
                    image.DpiX,
                    image.DpiY,
                    image.Format, null, data, stride);
            }
        }

        private void TreeViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            if (sender is CustomTreeViewItem item)
            {
                item.ImageSource = tempImage;
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is CustomTreeViewItem item && e.OriginalSource == sender)
            {
                if (item.Items.Count == 1 && item.Items[0] == dummyNode)
                {
                    GetDirectories(item);
                    GetFiles(item);
                }
            }
        }
        #endregion
    }
}