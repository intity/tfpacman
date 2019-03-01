using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for TreeListViewControl.xaml
    /// </summary>
    public partial class TreeListViewControl : UserControl, INotifyPropertyChanged
    {
        #region private fields
        private object dummyNode;
        private string targetDirectory;
        private string searchPattern;
        private int countFiles;
        private int colCount;
        private List<ImageSource> imageSourceList;
        private TreeListView treeListView;
        private ObservableDictionary<string, bool?[]> selectedItems;
        #endregion

        public TreeListViewControl()
        {
            InitializeComponent();

            dummyNode       = null;
            targetDirectory = null;
            searchPattern   = null;
            selectedItems   = new ObservableDictionary<string, bool?[]>();
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
                new BitmapImage(new Uri(Resource.BASE_URI + "pdf.ico"))
            };

            Loaded += TreeListViewControl_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
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
        public ObservableDictionary<string, bool?[]> SelectedItems
        {
            get { return (selectedItems); }
        }

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
                        SearchOption.AllDirectories).Count();
                }

                return (countFiles);
            }
        }
        #endregion

        public void InitLayout()
        {
            if (targetDirectory == null || treeListView == null)
                return;

            if (treeListView.Items.Count > 0)
                treeListView.Items.Clear();

            if (Directory.Exists(targetDirectory))
            {
                GetDirectories();
                GetFiles();

                Debug.WriteLine("InitLayout");
            }
        }

        private bool? [] DefaultValuesInit()
        {
            bool? [] value = new bool?[colCount];
            for (int i = 0; i < colCount; i++)
                value[i] = false;
            return value;
        }

        private void TreeListViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("TreeListViewControl_Loaded");

            if (Content == null)
                return;

            treeListView = Content as TreeListView;
            colCount = treeListView.Columns.Count;

            InitLayout();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                IsChecked(checkBox);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                IsChecked(checkBox);
            }
        }

        private void IsChecked(CheckBox checkBox)
        {
            int iColumn = 0;
            bool?[] value = null;

            var cp = checkBox.TemplatedParent as ContentPresenter;
            var rw = cp.Parent as GridViewRowPresenter;
            var ti = rw.TemplatedParent as TreeListViewItem;

            string path = checkBox.Tag.ToString();
            bool exists = selectedItems.ContainsKey(path);
            bool isfile = File.GetAttributes(path) == FileAttributes.Directory ? false : true;

            for (int i = 0; i < colCount; i++)
            {
                var ch = VisualTreeHelper.GetChild(rw, i);

                if (ch.Equals(cp))
                {
                    if (exists)
                    {
                        value = DefaultValuesInit();
                        value = selectedItems[path];
                        value[i] = checkBox.IsChecked;

                        if (value.Contains(true) && isfile)
                        {
                            selectedItems[path] = value;
                        }
                        else
                        {
                            selectedItems.Remove(path);
                        }
                    }

                    iColumn = i;
                    break;
                }
            }

            if (!exists)
            {
                if (checkBox.IsChecked == true && isfile)
                {
                    value = DefaultValuesInit();
                    value[iColumn] = checkBox.IsChecked;
                    selectedItems.Add(path, value);
                    selectedItems[path] = value;
                }

                GroupConfigure(ti, iColumn, checkBox.IsChecked, 1);
            }

            if (ti.Parent is TreeListViewItem ti_parent)
            {
                bool? cb_value = GroupConfigure(ti_parent, iColumn, checkBox.IsChecked, 0);
                CellConfigure(ti_parent, iColumn, cb_value, 1);
            }
        }

        /// <summary>
        /// The Cell configuration.
        /// </summary>
        /// <param name="ti">Tree Item</param>
        /// <param name="iColumn">The Column index</param>
        /// <param name="value">Boolean value</param>
        /// <param name="flag">
        /// Flags definition for gets or sets values of cell: 
        /// (0) - get value, 
        /// (1) - set value
        /// </param>
        /// <returns></returns>
        private bool? CellConfigure(TreeListViewItem ti, int iColumn, bool? value, int flag)
        {
            bool? result = null;

            var sp = VisualTreeHelper.GetChild(ti, 0) as StackPanel;
            var bd = VisualTreeHelper.GetChild(sp, 0) as Border;
            var rw = VisualTreeHelper.GetChild(bd, 0) as GridViewRowPresenter;
            var cp = VisualTreeHelper.GetChild(rw, iColumn) as ContentPresenter;
            var dp = VisualTreeHelper.GetChild(cp, 0) as DockPanel;
            var cb = dp.FindName("Cb") as CheckBox;

            return result = (flag > 0 ? (cb.IsChecked = value) : cb.IsChecked);
        }

        /// <summary>
        /// The Cell group configuration.
        /// </summary>
        /// <param name="ti">Tree item</param>
        /// <param name="iColumn"></param>
        /// <param name="value"></param>
        /// <param name="flag">
        /// Flags definition for gets or sets values of cell group:
        /// (0) - get value,
        /// (1) - set value
        /// </param>
        /// <returns></returns>
        private bool? GroupConfigure(TreeListViewItem ti, int iColumn, bool? value, int flag)
        {
            int count = 0;
            bool? result = null;

            var sp = VisualTreeHelper.GetChild(ti, 0) as StackPanel;
            var ip = VisualTreeHelper.GetChild(sp, 1) as ItemsPresenter;
            int ic = VisualTreeHelper.GetChildrenCount(ip);

            if (ic == 0)
            {
                ti.IsExpanded = true;
                ti.UpdateLayout();
                ti.IsExpanded = false;
                ic = VisualTreeHelper.GetChildrenCount(ip);
            }

            if (ic == 0) return null;

            var sp_child = VisualTreeHelper.GetChild(ip, 0) as StackPanel;
            int ic_child = VisualTreeHelper.GetChildrenCount(sp_child);

            for (int i = 0; i < ic_child; i++)
            {
                var tv_child = VisualTreeHelper.GetChild(sp_child, i) as TreeListViewItem;

                if (CellConfigure(tv_child, iColumn, value, flag) == true)
                {
                    count++;
                }
            }

            if (count > 0 && count != ic_child)
                result = null;
            else if (count == ic_child)
                result = true;
            else
                result = false;

            return result;
        }

        private void GetDirectories(TreeListViewItem item = null)
        {
            string directory;

            if (item != null)
            {
                item.Items.Clear();
                directory = item.Tag.ToString();
            }
            else
                directory = targetDirectory;

            if (directory == null || treeListView == null)
                return;

            foreach (var i in Directory.GetDirectories(directory))
            {
                TreeListViewItem subitem = new TreeListViewItem
                {
                    Header = i.Substring(i.LastIndexOf("\\") + 1),
                    BorderBrush = new SolidColorBrush(new Color { A = 100, R = 213, G = 213, B = 213 }),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    ImageSource = imageSourceList[0],
                    Tag = i
                };

                subitem.Items.Add(dummyNode);
                subitem.Collapsed += Folder_Collapsed;
                subitem.Expanded += Folder_Expanded;

                if (item != null)
                    item.Items.Add(subitem);
                else
                    treeListView.Items.Add(subitem);
            }
        }

        private string[] GetFiles(string path, SearchOption option)
        {
            string [] patterns = searchPattern.Split('|');
            ArrayList files = new ArrayList();
            foreach (var i in patterns)
                files.AddRange(Directory.GetFiles(path, i, option));
            return files.ToArray(typeof(string)) as string[];
        }

        private void GetFiles(TreeListViewItem item = null)
        {
            string directory = (item != null ? item.Tag.ToString() : targetDirectory);
            int imageIndex = 0;

            if (directory == null || treeListView == null)
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
                }

                TreeListViewItem subitem = new TreeListViewItem
                {
                    Header = Path.GetFileName(i),
                    BorderBrush = new SolidColorBrush(new Color { A = 100, R = 213, G = 213, B = 213 }),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    ImageSource = imageSourceList[imageIndex],
                    Tag = i
                };

                subitem.Selected += TreeListViewItem_Selected;
                subitem.Unselected += TreeListViewItem_Unselected;

                if (item != null)
                    item.Items.Add(subitem);
                else
                    treeListView.Items.Add(subitem);
            }
        }

        private void Folder_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is TreeListViewItem item && e.OriginalSource == sender)
            {
                item.ImageSource = imageSourceList[0];
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeListViewItem item && e.OriginalSource == sender)
            {
                item.ImageSource = imageSourceList[1];

                if (item.Items.Count == 1 && item.Items[0] == dummyNode)
                {
                    GetDirectories(item);
                    GetFiles(item);
                }
            }
        }

        private void TreeListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeListViewItem item)
            {
                var image   = item.ImageSource as BitmapImage;
                int stride  = (image.PixelWidth * image.Format.BitsPerPixel + 7) / 8;
                int length  = stride * image.PixelHeight;
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

        private void TreeListViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeListViewItem item)
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
                }

                item.ImageSource = imageSourceList[imageIndex];
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
