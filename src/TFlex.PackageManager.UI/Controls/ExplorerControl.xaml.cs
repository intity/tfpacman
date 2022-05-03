using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Configuration;
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
            var obj = new ProcItem();
            var cfg = DataContext as Header;
            var dir = Path.GetDirectoryName(path);

            if (Flags == 0)
            {
                obj.IPath = path;
                obj.Directory = dir.Replace(cfg.InitialCatalog, cfg.TargetDirectory);
            }
            else
            {
                obj.OPath = path;
                obj.Directory = dir;
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

                InitTreeItems_1(subItem); // recursive call
            }

            GetFiles(item);
        }

        private void GetFiles(CustomTreeViewItem item)
        {
            var dir = item != null ? item.Tag.ToString() : rootDirectory;
            var cfg = DataContext as Header;
            var obj = cfg.Translator as Files;
            var ext = Flags > 0 ? obj.OExtension : obj.IExtension;
            var opt = SearchOption.TopDirectoryOnly;

            searchPattern = "*" + ext;

            foreach (var i in Directory.GetFiles(dir, searchPattern, opt))
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
            //
            // Initializing a tree structure of assembly units and related 
            // documents.
            //
            if (searchPattern != "*.grb")
                return;

            var opt   = SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(rootDirectory, searchPattern, opt);
            foreach (var i in files)
            {
                var item = CreateItem(i);
                ctv2.Items.Add(item);
                GetLinks(item);
            }
        }

        private void GetLinks(CustomTreeViewItem item)
        {
            var data  = item.Tag as ProcItem;
            var path  = Flags > 0 ? data.OPath : data.IPath;
            var links = Application
                .GetDocumentExternalFileLinks(path, true, false, false)
                .OrderBy(i => i);

            foreach (var i in links)
            {
                var subItem = CreateItem(i);
                var subData = subItem.Tag as ProcItem;
                subData.Parent = data;
                data.Items.Add(subData);
                item.Items.Add(subItem);
                GetLinks(subItem); // recursive call
            }

            Application.IdleSession();
        }

        private void CheckingToParent(CustomTreeViewItem item)
        {
            bool? value = item.IsChecked;

            if (item.Parent is CustomTreeViewItem parent)
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
                CheckingToParent(parent); // recursive call
            }
        }

        private void SelectedItemTask(ProcItem item)
        {
            foreach (var i in item.Items)
            {
                if (item.Flags == 0)
                {
                    int result = 0;
                    var parent = item.Parent;
                    while (parent != null)
                    {
                        result = parent.Flags;
                        parent = parent.Parent;
                    }
                    i.Flags = result;
                }
                SelectedItemTask(i); // recursive call
            }
        }

        private void SelectedItemTask(ProcItem src, ProcItem dst, ref int res)
        {
            if (dst.IPath == src.IPath && dst.Level != src.Level)
            {
                if (res > 0)
                {
                    if (dst.Parent == null || dst.Parent.Flags > 0)
                    {
                        dst.Flags = res;
                        res = 0; // inverted result (1 -> 0)
                    }
                }
                else
                {
                    dst.Flags = res;
                    res |= 0x1; // (0 -> 1)
                }
            }

            foreach (var i in dst.Items)
            {
                SelectedItemTask(src, i, ref res); // recursive call
            }
        }

        private void SelectedItemTask(ProcItem data, int value)
        {
            int result = value;
            data.Flags = value;
            //
            // pre-processing the flags
            //
            foreach (var item in SelectedItems.Values)
            {
                SelectedItemTask(data, item, ref result);
                if (result != value)
                {
                    //
                    // selection sequence: asm -> part
                    //
                    data.Flags = result;
                    result = value;
                }

                SelectedItemTask(item, data, ref result);
                if (result != value)
                {
                    //
                    // selection sequence: part -> asm
                    //
                    item.Flags = result;
                    result = value;
                }
            }
        }

        private void SelectedItemTask(CustomTreeViewItem item, int flag)
        {
            if (!(item.Tag is ProcItem data))
                return;

            ProcItem d;
            if (flag > 0)
            {
                int value = 0x1;
                if (searchPattern == "*.grb" && (d = GetData(item)) != null)
                {
                    data = d;
                    SelectedItemTask(data, value);
                }
                else
                    data.Flags = value;

                SelectedItems.Add(data.IPath, data);
            }
            else
            {
                SelectedItems.Remove(data.IPath);

                if (searchPattern == "*.grb" && (d = GetData(item)) != null)
                {
                    data = d;
                    SelectedItemTask(data, 0);
                }
            }
        }

        private ProcItem GetData(CustomTreeViewItem item)
        {
            //
            // Getting data by a selected Item from the assembly tree.
            //
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
            var data1 = src.Tag as ProcItem;
            var data2 = dst.Tag as ProcItem;

            if (data1.IPath == data2.IPath && data1.Level == data2.Level)
            {
                return data2;
            }

            foreach (CustomTreeViewItem i in dst.Items)
            {
                var value = Compare(src, i); // recursive call
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
