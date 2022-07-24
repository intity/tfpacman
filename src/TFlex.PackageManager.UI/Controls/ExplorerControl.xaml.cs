using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.UI.Configuration;
using TFlex.PackageManager.UI.Model;

namespace TFlex.PackageManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for ExplorerControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl, INotifyPropertyChanged
    {
        #region private fields
        string searchPattern;
        string rootDirectory;
        bool enableAsmTree;
        int countItems;
        ImageSource tmpImage;
        #endregion

        public ExplorerControl()
        {
            InitializeComponent();

            rootDirectory = null;
            searchPattern = "*.grb";
            Items = new Dictionary<string, ProcItem>();
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
        /// Total count selected items.
        /// </summary>
        public int CountItems
        {
            get => countItems;
            private set
            {
                if (countItems != value)
                {
                    countItems = value;
                    OnPropertyChanged("CountItems");
                }
            }
        }

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
        /// Data items.
        /// </summary>
        public Dictionary<string, ProcItem> Items { get; }
        #endregion

        #region public methods
        /// <summary>
        /// Update the control layouts.
        /// </summary>
        public void UpdateControl()
        {
            CountFiles = 0;
            CountItems = 0;

            Items.Clear();

            ctv1.Items.Clear();
            ctv2.Items.Clear();

            if (Directory.Exists(rootDirectory))
            {
                InitTreeItems();

                if (Flags > 0 && CountFiles > 0)
                {
                    //
                    // select all output items for tree configure
                    //
                    foreach (var i in Items.Values)
                    {
                        i.Flags |= 0x1;
                    }
                    UpdateItems(1);
                }
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
        private ProcItem CreateData(string path)
        {
            var obj = new ProcItem();
            var dir = Path.GetDirectoryName(path);
            var cfg = DataContext as Header;

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

            return obj;
        }

        private CustomTreeViewItem CreateItem(string path)
        {
            var item = new CustomTreeViewItem
            {
                Header    = Path.GetFileName(path),
                Extension = Path.GetExtension(path)
            };

            item.Checked    += Item_Checked;
            item.Unchecked  += Item_Unchecked;
            item.Selected   += Item_Selected;
            item.Unselected += Item_Unselected;

            return item;
        }
        
        private void InitTreeItems(CustomTreeViewItem item = null)
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

                InitTreeItems(subItem); // recursive call
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
                var subData = CreateData(i);
                subItem.Tag = i;
                Items.Add(i, subData);

                if (searchPattern == "*.grb")
                {
                    GetLinks(subData);
                }

                if (item != null)
                    item.Items.Add(subItem);
                else
                    ctv1.Items.Add(subItem);
            }
        }

        private void GetItems(CustomTreeViewItem item, ProcItem data)
        {
            //
            // init subitems from data
            //
            foreach (var i in data.Items)
            {
                if ((i.Flags & 0x1) != 0x1)
                    continue;

                var subPath = Flags > 0 ? i.OPath : i.IPath;
                var subItem = CreateItem(subPath);
                subItem.Tag = subPath;
                item.Items.Add(subItem);
                GetItems(subItem, i); // recursive call
            }
        }

        private void GetLinks(ProcItem data)
        {
            var path = Flags > 0 ? data.OPath : data.IPath;
            var links = Application
                .GetDocumentExternalFileLinks(path, true, false, false)
                .OrderBy(i => i);

            foreach (var i in links)
            {
                var subData = CreateData(i);
                subData.Parent = data;
                data.Items.Add(subData);
                GetLinks(subData); // recursive call
            }

            Application.IdleSession();
        }

        private void CfgItems(ProcItem item, int flag)
        {
            //
            // configure selector
            //
            var path = Flags > 0 ? item.OPath : item.IPath;
            var dest = Items[path];

            if (dest.Flags == 1 && flag == 1)
            {
                if (item.Parent != null && (item.Parent.Flags & 0x1) == 0x1)
                {
                    dest.Flags ^= 0x5;
                    item.Flags |= 0x1;

                    foreach (var i in dest.Items)
                    {
                        CfgItems(i, 0); // recursive call
                    }
                }
            }
            else if (flag == 0)
            {
                if (dest.Flags == 4 && item.Flags == 1)
                {
                    if (item.Parent.Flags == 0 ||
                        item.Parent.Flags == 4)
                    {
                        dest.Flags ^= 0x5;
                        item.Flags ^= 0x1;
                    }

                    foreach (var i in item.Items)
                    {
                        CfgItems(i, 0); // recursive call
                    }

                    foreach (var i in dest.Items)
                    {
                        CfgItems(i, 1); // recursive call
                    }
                }

                if (dest.Flags == 5 && item.Flags == 1)
                {
                    dest.Flags ^= 0x5;
                    item.Flags ^= 0x1;

                    foreach (var i in dest.Items)
                    {
                        CfgItems(i, 0); // recursive call
                    }
                }
            }

            foreach (var i in item.Items)
            {
                CfgItems(i, flag); // recursive call
            }
        }

        private void UpdateItems(int flag)
        {
            if (searchPattern != "*.grb")
                return;
            
            ctv2.Items.Clear();
            //
            // configure items
            //
            foreach (var i in Items.Values)
            {
                CfgItems(i, flag);
            }
            //
            // initialize the tree view
            //
            foreach (var i in Items)
            {
                if ((i.Value.Flags & 0x1) != 0x1)
                    continue;
                
                var item = CreateItem(i.Key);
                item.Tag = i.Key;
                GetItems(item, i.Value);
                ctv2.Items.Add(item);
            }
            //
            // update tree view layout
            //
            ctv2.UpdateLayout();
        }
        #endregion

        #region event handlers
        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            var path = item.Tag.ToString();
            var data = Items[path];
            data.Flags |= 0x1;
            CountItems++;
            UpdateItems(1);
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            var path = item.Tag.ToString();
            var data = Items[path];
            data.Flags ^= 0x1;
            CountItems--;
            UpdateItems(0);
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            e.Handled = true;
            tmpImage = item.ImageSource;
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
            item.ImageSource = tmpImage;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
