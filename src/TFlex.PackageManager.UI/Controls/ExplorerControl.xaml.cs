using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Model;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for ExplorerControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        #region private fields
        string searchPattern;
        string rootDirectory;
        bool enableAsmTree;
        ImageSource tempImage;
        #endregion

        public ExplorerControl()
        {
            InitializeComponent();

            rootDirectory = null;
            searchPattern = "*.grb";
            SelectedItems = new ObservableDictionary<string, ProcItem>();
        }

        #region public fields
        public static readonly DependencyProperty FlagsProperty =
            DependencyProperty.Register("Flags",
                typeof(int),
                typeof(ExplorerControl),
                new FrameworkPropertyMetadata(0));
        #endregion

        #region public properties
        /// <summary>
        /// Explorer Flags: Input(0), Output(1)
        /// </summary>
        public int Flags
        {
            get => (int)GetValue(FlagsProperty);
            set => SetValue(FlagsProperty, value);
        }

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
        /// Enable assembly tree view.
        /// </summary>
        public bool EnableAsmTree
        {
            get => enableAsmTree;
            set
            {
                if (enableAsmTree != value)
                    enableAsmTree = value;

                if (enableAsmTree)
                {
                    ctv1.Visibility = Visibility.Collapsed;
                    ctv2.Visibility = Visibility.Visible;
                }
                else
                {
                    ctv1.Visibility = Visibility.Visible;
                    ctv2.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Selected items.
        /// </summary>
        public ObservableDictionary<string, ProcItem> SelectedItems { get; }
        #endregion

        #region public methods
        /// <summary>
        /// Update the layout control.
        /// </summary>
        public void UpdateControl()
        {
            CountFiles = 0;

            ctv1.Items.Clear();
            ctv2.Items.Clear();

            if (Flags == 0)
            {
                SelectedItems.Clear();
            }

            if (Directory.Exists(rootDirectory))
            {
                InitTreeItems_1();
                InitTreeItems_2();
            }

            ctv1.UpdateLayout();
            ctv2.UpdateLayout();
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
        private CustomTreeViewItem CreateItem(string path)
        {
            var obj = new ProcItem()
            {
                Directory = Path.GetDirectoryName(path)
            };

            if (Flags == 0)
            {
                obj.IPath = path;
            }
            else
            {
                obj.OPath = path;
            }

            var item = new CustomTreeViewItem
            {
                Header    = Path.GetFileName(path),
                Extension = Path.GetExtension(path),
                Tag       = obj
            };

            item.Selected   += Item_Selected;
            item.Unselected += Item_Unselected;

            return item;
        }

        private void InitTreeItems_1(CustomTreeViewItem item = null)
        {
            var dir = item != null ? item.Tag.ToString() : rootDirectory;

            foreach (var i in Directory.GetDirectories(dir))
            {
                var subItem = new CustomTreeViewItem
                {
                    Header = i.Substring(i.LastIndexOf("\\") + 1),
                    Tag    = i
                };

                if (item != null)
                    item.Items.Add(subItem);
                else
                    ctv1.Items.Add(subItem);

                InitTreeItems_1(subItem);
            }

            GetFiles(item);
        }

        private void GetFiles(CustomTreeViewItem item)
        {
            var dir   = item != null ? item.Tag.ToString() : rootDirectory;
            var opt   = SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(dir, SearchPattern, opt);

            foreach (var i in files)
            {
                CountFiles++;
                var subItem = CreateItem(i);

                if (item != null)
                    item.Items.Add(subItem);
                else
                    ctv1.Items.Add(subItem);
            }
        }

        private void InitTreeItems_2()
        {
            if (searchPattern != "*.grb")
                return;

            var opt   = SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(rootDirectory, SearchPattern, opt);
            foreach (var i in files)
            {
                var item = CreateItem(i);
                ctv2.Items.Add(item);
                GetLinks(item);
            }
        }

        private void GetLinks(CustomTreeViewItem item)
        {
            var obj   = item.Tag as ProcItem;
            var path  = Flags > 0 ? obj.OPath : obj.IPath;
            var links = Application
                .GetDocumentExternalFileLinks(path, true, false, false);

            foreach (var link in links.OrderBy(i => i))
            {
                var subItem = CreateItem(link);
                var objLink = subItem.Tag as ProcItem;
                objLink.Parent = obj;
                obj.Items.Add(objLink);
                item.Items.Add(subItem);
                GetLinks(subItem);
            }

            Application.IdleSession();
        }

        private void CheckingToParent(CustomTreeViewItem item)
        {
            bool? value = item.IsChecked;

            if (item.Parent != null &&
                item.Parent is CustomTreeViewItem parent)
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
        }

        private void SelectedItemTask(CustomTreeViewItem item, int flag)
        {
            if (!(item.Tag is ProcItem obj1))
                return;

            if (flag > 0)
            {
                if (searchPattern == "*.grb")
                {
                    var obj2 = GetItem(item);
                    if (obj2 != null)
                        obj1 = obj2;
                }

                obj1.Flags |= 0x1;
                SelectedItems.Add(obj1.IPath, obj1);
            }
            else
                SelectedItems.Remove(obj1.IPath);
        }

        private ProcItem GetItem(CustomTreeViewItem item)
        {
            foreach (CustomTreeViewItem i in ctv2.Items)
            {
                var value = Compare(item, i);
                if (value != null)
                    return value;
            }
            return null;
        }

        private ProcItem Compare(CustomTreeViewItem src, CustomTreeViewItem dst)
        {
            if ((src.Tag as ProcItem).IPath == (dst.Tag as ProcItem).IPath)
                return dst.Tag as ProcItem;

            foreach (CustomTreeViewItem i in dst.Items)
            {
                var value = Compare(src, i);
                if (value != null)
                    return value;
            }
            return null;
        }
        #endregion

        #region event handlers
        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox cb && 
                cb.TemplatedParent is CustomTreeViewItem item))
                return;

            if (item.HasItems)
            {
                foreach (CustomTreeViewItem i in item.Items)
                {
                    i.IsChecked = true;
                    if (item.IsExpanded)
                        continue;
                    SelectedItemTask(i, 1);
                }
                return;
            }

            CheckingToParent(item);
            SelectedItemTask(item, 1);
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox cb && 
                cb.TemplatedParent is CustomTreeViewItem item))
                return;

            if (item.HasItems)
            {
                foreach (CustomTreeViewItem i in item.Items)
                {
                    i.IsChecked = false;
                    if (item.IsExpanded)
                        continue;
                    SelectedItemTask(i, 0);
                }
                return;
            }

            CheckingToParent(item);
            SelectedItemTask(item, 0);
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            e.Handled = true;
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

        private void Item_Unselected(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            e.Handled = true;
            item.ImageSource = tempImage;
        }
        #endregion
    }
}
