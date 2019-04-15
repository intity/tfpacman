﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for TreeViewControl.xaml
    /// </summary>
    public partial class TreeViewControl : UserControl
    {
        #region private fields
        private readonly object dummyNode;
        private string targetDirectory;
        private string searchPattern;
        private int countFiles;
        private readonly List<ImageSource> imageSourceList;
        private CustomTreeView treeView;
        #endregion

        public TreeViewControl()
        {
            InitializeComponent();

            dummyNode       = null;
            targetDirectory = null;
            searchPattern   = null;
            SelectedItems   = new ObservableCollection<object>();
            imageSourceList = new List<ImageSource>()
            {
                new BitmapImage(new Uri(Resource.BASE_URI + "collapsed_folder.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "expanded_folder.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "grb.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "dwg.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "dxf.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "dxb.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "bmp.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "jpg.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "gif.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "tif.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "png.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "pdf.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "stp.ico"))
            };
            
            Loaded  += TreeViewControl_Loaded;
        }

        #region public properties
        /// <summary>
        /// Target directory.
        /// </summary>
        public string TargetDirectory
        {
            get { return targetDirectory; }
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;
                    InitLayout();
                }
            }
        }

        /// <summary>
        /// Search pattern to in GetFile method use.
        /// </summary>
        public string SearchPattern
        {
            get { return searchPattern; }
            set
            {
                if (searchPattern != value)
                    searchPattern = value;
            }
        }

        /// <summary>
        /// Selected items.
        /// </summary>
        public ObservableCollection<object> SelectedItems { get; }

        /// <summary>
        /// Total count files.
        /// </summary>
        public int CountFiles
        {
            get
            {
                if (Directory.Exists(targetDirectory))
                {
                    countFiles = GetFiles(targetDirectory,
                        SearchOption.AllDirectories).Count;
                }

                return (countFiles);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Initialize layout control.
        /// </summary>
        public void InitLayout()
        {
            if (targetDirectory == null || treeView == null)
                return;

            if (treeView.Items.Count > 0)
                treeView.Items.Clear();

            if (Directory.Exists(targetDirectory))
            {
                GetDirectories();
                GetFiles();
            }
            else
                countFiles = 0;

            //Debug.WriteLine("InitLayout");
        }

        /// <summary>
        /// Clean target directory.
        /// </summary>
        public void CleanTargetDirectory()
        {
            if (!Directory.Exists(targetDirectory))
                return;

            DirectoryInfo di = new DirectoryInfo(targetDirectory);

            foreach (var i in di.GetFiles()) i.Delete();
            foreach (var i in di.GetDirectories()) i.Delete(true);

            InitLayout();
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
                directory = targetDirectory;

            if (directory == null || treeView == null)
                return;

            foreach (var i in Directory.GetDirectories(directory))
            {
                CustomTreeViewItem subitem = new CustomTreeViewItem
                {
                    Header = i.Substring(i.LastIndexOf("\\") + 1),
                    ImageSource = imageSourceList[0],
                    NodeParent = item,
                    Tag = i
                };

                subitem.Items.Add(dummyNode);
                subitem.Expanded += Folder_Expanded;
                subitem.Collapsed += Folder_Collapsed;

                if (item != null)
                    item.Items.Add(subitem);
                else
                    treeView.Items.Add(subitem);
            }
        }

        private IList<string> GetFiles(string path, SearchOption option)
        {
            string[] patterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (var i in patterns)
                files.AddRange(Directory.GetFiles(path, i, option));
            return files;
        }

        private void GetFiles(CustomTreeViewItem item = null)
        {
            string directory = (item != null ? item.Tag.ToString() : targetDirectory);
            int imageIndex = 0;

            if (directory == null || treeView == null)
                return;

            foreach (var i in GetFiles(directory, SearchOption.TopDirectoryOnly))
            {
                switch (Path.GetExtension(i))
                {
                    case ".grb" : imageIndex = 02; break;
                    case ".dwg" : imageIndex = 03; break;
                    case ".dxf" : imageIndex = 04; break;
                    case ".dxb" : imageIndex = 05; break;
                    case ".bmp" : imageIndex = 06; break;
                    case ".jpeg": imageIndex = 07; break;
                    case ".gif" : imageIndex = 08; break;
                    case ".tiff": imageIndex = 09; break;
                    case ".png" : imageIndex = 10; break;
                    case ".pdf" : imageIndex = 11; break;
                    case ".stp" : imageIndex = 12; break;
                }

                CustomTreeViewItem subitem = new CustomTreeViewItem
                {
                    Header = Path.GetFileName(i),
                    ImageSource = imageSourceList[imageIndex],
                    NodeParent = item,
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
            bool? i_value = item.IsChecked;

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
                    i.IsChecked = i_value;
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

            if (item.NodeParent != null)
            {
                bool? n_value = false;

                foreach (CustomTreeViewItem j in item.NodeParent.Items)
                {
                    if (j.IsChecked != i_value)
                    {
                        n_value = null;
                        break;
                    }
                }

                if (n_value == null)
                    item.NodeParent.IsChecked = null;
                else
                    item.NodeParent.IsChecked = i_value;
            }
        }
        #endregion

        #region event handlers
        private void TreeViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Content == null)
                return;

            treeView = Content as CustomTreeView;
            InitLayout();
        }

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
                int imageIndex = 0;

                switch (Path.GetExtension(item.Tag.ToString()))
                {
                    case ".grb" : imageIndex = 02; break;
                    case ".dwg" : imageIndex = 03; break;
                    case ".dxf" : imageIndex = 04; break;
                    case ".dxb" : imageIndex = 05; break;
                    case ".bmp" : imageIndex = 06; break;
                    case ".jpeg": imageIndex = 07; break;
                    case ".gif" : imageIndex = 08; break;
                    case ".tiff": imageIndex = 09; break;
                    case ".png" : imageIndex = 10; break;
                    case ".pdf" : imageIndex = 11; break;
                    case ".stp" : imageIndex = 12; break;
                }

                item.ImageSource = imageSourceList[imageIndex];
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is CustomTreeViewItem item && e.OriginalSource == sender)
            {
                item.ImageSource = imageSourceList[1];

                if (item.Items.Count == 1 && item.Items[0] == dummyNode)
                {
                    GetDirectories(item);
                    GetFiles(item);
                }
            }
        }

        private void Folder_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is CustomTreeViewItem item && e.OriginalSource == sender)
            {
                item.ImageSource = imageSourceList[0];
            }
        }
        #endregion
    }

    /// <summary>
    /// The TreeView class override.
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

    /// <summary>
    /// The TreeViewItem class override.
    /// </summary>
    internal class CustomTreeViewItem : TreeViewItem
    {
        #region private fields
        private int level = -1;
        #endregion

        #region properties
        /// <summary>
        /// The IsChecked property to checkbox.
        /// </summary>
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Is the item a node.
        /// </summary>
        public bool IsNode
        {
            get
            {
                return File.GetAttributes(Tag.ToString()) == FileAttributes.Directory;
            }
        }

        /// <summary>
        /// The tree node parent object.
        /// </summary>
        public CustomTreeViewItem NodeParent { get; set; }

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
        /// The ImageSource property.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        #endregion

        #region public fields
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource",
                typeof(ImageSource),
                typeof(CustomTreeViewItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached("IsChecked", typeof(bool?), typeof(CustomTreeViewItem),
                new FrameworkPropertyMetadata((bool?)false));
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