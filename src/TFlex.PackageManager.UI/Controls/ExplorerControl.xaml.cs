using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TFlex.PackageManager.UI.Common;
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
        int stateItems;
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
                    stateItems = 0; // changed
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
                    if (Flags == 0 && stateItems == 0)
                    {
                        UpdateItems();
                        stateItems = 1; // updated
                    }
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

                if (searchPattern == "*.grb")
                {
                    //
                    // configure items
                    //
                    foreach (var i in Items.Values)
                    {
                        CfgItems(i);
                    }
                }

                if (Flags > 0 && CountFiles > 0)
                {
                    //
                    // select all output items for tree configure
                    //
                    foreach (var i in Items.Values)
                    {
                        i.Flags |= 0x1;
                    }
                    UpdateItems();
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

            if (Flags == 0)
            {
                obj.IPath = path;
            }
            else
            {
                obj.OPath = path;
            }

            return obj;
        }

        private CustomTreeViewItem CreateItem(string path)
        {
            var item = new CustomTreeViewItem
            {
                Header = Path.GetFileName(path)
            };

            var icon = NativeMethods.GetIcon(path, true);
            if (icon != null)
            {
                item.ImageSource = icon.ToImageSource();
            }

            if (Flags == 0)
            {
                item.Checked   += Item_Checked;
                item.Unchecked += Item_Unchecked;
            }

            return item;
        }
        
        private void InitTreeItems(CustomTreeViewItem item = null)
        {
            var dir = item != null ? item.Tag.ToString() : rootDirectory;

            foreach (var i in Directory.GetDirectories(dir))
            {
                var subItem = new CustomTreeViewItem
                {
                    IsNode = true,
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
            var opt = SearchOption.TopDirectoryOnly;

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

                var subItem = CreateItem(i.UPath);
                subItem.Tag = i.UPath;
                item.Items.Add(subItem);
                GetItems(subItem, i); // recursive call
            }
        }

        private void GetLinks(ProcItem data)
        {
            var links = Application
                .GetDocumentExternalFileLinks(data.UPath, true, false, false)
                .OrderBy(i => i);

            foreach (var i in links)
            {
                if (!i.Contains(rootDirectory))
                {
                    //
                    // the link points outside the root directory
                    //
                    continue;
                }

                if (!File.Exists(i))
                    continue;

                var subData = CreateData(i);
                subData.Parent = data;
                data.Items.Add(subData);
                GetLinks(subData); // recursive call
            }

            Application.IdleSession();
        }

        private void CfgItems(ProcItem data)
        {
            //
            // preconfigure items
            //
            var dest = Items[data.UPath];
            if (dest.Level != data.Level)
            {
                dest.Links.Add(data);
                data.Dests.Add(dest);
            }

            if (data.Parent != null && data.Parent.Level == 0)
            {
                dest.ERefs.Add(data.Parent);
            }

            foreach (var i in data.Items)
            {
                CfgItems(i); // recursive call
            }
        }

        private void UpdateItems()
        {
            if (searchPattern != "*.grb")
                return;
            
            ctv2.Items.Clear();
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
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomTreeViewItem item))
                return;

            var path = item.Tag.ToString();
            var data = Items[path];
            data.Flags ^= 0x1;
            CountItems--;
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
