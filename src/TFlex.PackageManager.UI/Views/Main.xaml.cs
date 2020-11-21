using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        #region private fields
        readonly ConfigurationCollection conf;
        readonly Common.Options options;
        readonly List<PropertyDefinition> modules;

        string key1, key2;
        int importMode;

        System.Threading.Thread thread;
        bool stoped;

        static IntPtr handle = IntPtr.Zero;
        static IntPtr oldWndProc = IntPtr.Zero;
        NativeMethods.WndProc newWndProc;

        const int WM_STOPPED_PROCESSING = 0x0400;
        const int WM_INCREMENT_PROGRESS = 0x0401;
        #endregion

        public Main()
        {
            InitializeComponent();
            Title   = Properties.Resources.AppName;
            options = new Common.Options();

            #region initialize controls
            tvControl1.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            using (UndoRedoManager.Start("Init"))
            {
                conf = new ConfigurationCollection
                {
                    UserDirectory = options.UserDirectory
                };
                UndoRedoManager.FlushHistory();
            }

            UndoRedoManager.CommandDone += delegate
            {
                menuItem2_1.IsEnabled = UndoRedoManager.CanUndo;
                menuItem2_2.IsEnabled = UndoRedoManager.CanRedo;

                button2_1.IsEnabled = UndoRedoManager.CanUndo;
                button2_2.IsEnabled = UndoRedoManager.CanRedo;
            };
            #endregion

            #region initialize resources
            tb_label1.Content = Properties.Resources.Strings["ui_0:tbl_1"][0];
            tb_label2.Content = Properties.Resources.Strings["ui_0:tbl_2"][0];

            sb_label1.Content = string.Format(Properties.Resources.Strings["ui_0:sbl_1"][0], 0);
            sb_label2.Content = string.Format(Properties.Resources.Strings["ui_0:sbl_2"][0], 0);
            sb_label3.Content = string.Format(Properties.Resources.Strings["ui_0:sbl_3"][0], 0);

            sb_label1.ToolTip = Properties.Resources.Strings["ui_0:sbl_1"][1];
            sb_label2.ToolTip = Properties.Resources.Strings["ui_0:sbl_2"][1];
            sb_label3.ToolTip = Properties.Resources.Strings["ui_0:sbl_3"][1];

            menuItem1.Header = Properties.Resources.Strings["ui_0:ctl_1"][0]; // File
            menuItem2.Header = Properties.Resources.Strings["ui_0:ctl_2"][0]; // Edit
            menuItem3.Header = Properties.Resources.Strings["ui_0:ctl_3"][0]; // Processing
            menuItem4.Header = Properties.Resources.Strings["ui_0:ctl_4"][0]; // Settings

            menuItem1_1.Header = Properties.Resources.Strings["ui_0:c_1_1"][0];
            menuItem1_2.Header = Properties.Resources.Strings["ui_0:c_1_2"][0];
            menuItem1_3.Header = Properties.Resources.Strings["ui_0:c_1_3"][0];
            menuItem1_4.Header = Properties.Resources.Strings["ui_0:c_1_4"][0];
            menuItem1_5.Header = Properties.Resources.Strings["ui_0:c_1_5"][0];
            menuItem1_6.Header = Properties.Resources.Strings["ui_0:c_1_6"][0];
            menuItem1_7.Header = Properties.Resources.Strings["ui_0:c_1_7"][0];
            menuItem1_8.Header = Properties.Resources.Strings["ui_0:c_1_8"][0];
            menuItem2_1.Header = Properties.Resources.Strings["ui_0:c_2_1"][0];
            menuItem2_2.Header = Properties.Resources.Strings["ui_0:c_2_2"][0];
            menuItem3_1.Header = Properties.Resources.Strings["ui_0:c_3_1"][0];
            menuItem3_2.Header = Properties.Resources.Strings["ui_0:c_3_2"][0];
            menuItem3_3.Header = Properties.Resources.Strings["ui_0:c_3_3"][0];
            menuItem4_1.Header = Properties.Resources.Strings["ui_0:c_4_1"][0];
            menuItem4_2.Header = Properties.Resources.Strings["ui_0:c_4_2"][0];
            menuItem5_1.Header = Properties.Resources.Strings["ui_0:c_5_1"][0];
            menuItem5_2.Header = Properties.Resources.Strings["ui_0:c_5_2"][0];

            button1_1.ToolTip = Properties.Resources.Strings["ui_0:c_1_1"][1];
            button1_2.ToolTip = Properties.Resources.Strings["ui_0:c_1_2"][1];
            button1_3.ToolTip = Properties.Resources.Strings["ui_0:c_1_3"][1];
            button1_4.ToolTip = Properties.Resources.Strings["ui_0:c_1_4"][1];
            button1_5.ToolTip = Properties.Resources.Strings["ui_0:c_1_5"][1];
            button1_6.ToolTip = Properties.Resources.Strings["ui_0:c_1_6"][1];
            button1_7.ToolTip = Properties.Resources.Strings["ui_0:c_1_7"][1];
            button2_1.ToolTip = Properties.Resources.Strings["ui_0:c_2_1"][1];
            button2_2.ToolTip = Properties.Resources.Strings["ui_0:c_2_2"][1];
            button3_1.ToolTip = Properties.Resources.Strings["ui_0:c_3_1"][1];
            button3_2.ToolTip = Properties.Resources.Strings["ui_0:c_3_2"][1];
            button3_3.ToolTip = Properties.Resources.Strings["ui_0:c_3_3"][1];
            button4_1.ToolTip = Properties.Resources.Strings["ui_0:c_4_1"][1];
            button4_2.ToolTip = Properties.Resources.Strings["ui_0:c_4_2"][1];
            #endregion

            #region initialize property definitions
            inputPath1.PropertyDefinitions.Add(new PropertyDefinition
            {
                TargetProperties = new[]
                {
                    "ConfigName",
                    "TargetDirectory",
                    "Modules"
                },
                IsBrowsable = false
            });

            inputPath2.PropertyDefinitions.Add(new PropertyDefinition
            {
                TargetProperties = new[]
                {
                    "ConfigName",
                    "InitialCatalog",
                    "Modules"
                },
                IsBrowsable = false
            });

            modules = new List<PropertyDefinition>
            {
                new PropertyDefinition // (0) Links
                {
                    TargetProperties = new[]
                    {
                        "LinkTemplate"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (1) Pages
                {
                    TargetProperties = new[]
                    {
                        "PageNames",
                        "ExcludePage",
                        "PageScale",
                        "PageTypes",
                        "CheckDrawingTemplate"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (2) Projections
                {
                    TargetProperties = new[]
                    {
                        "ProjectionNames",
                        "ExcludeProjection",
                        "ProjectionScale"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (3) Variables
                {
                    TargetProperties = new[]
                    {
                        "AddVariables",
                        "EditVariables",
                        "RenameVariables",
                        "RemoveVariables"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (4) Export
                {
                    TargetProperties = new[]
                    {
                        "ImportMode",
                        "Heal",
                        "CreateAccurateEdges",
                        "ImportSolidBodies",
                        "ImportSheetBodies",
                        "ImportWireBodies",
                        "ImportMeshBodies",
                        "ImportHideBodies",
                        "ImportAnotations",
                        "ImportOnlyFromActiveFilter",
                        "SewTolerance",
                        "CheckImportGeomerty",
                        "UpdateProductStructure",
                        "AddBodyRecordsInProductStructure"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (5) Import
                {
                    TargetProperties = new[]
                    {
                        "FileNameSuffix",
                        "TemplateFileName",
                        "Version",
                        "Protocol",
                        "ExportMode",
                        "ColorSource",
                        "ExportSolidBodies",
                        "ExportSheetBodies",
                        "ExportWireBodies",
                        "Export3DPictures",
                        "ExportAnotation",
                        "ExportWelds",
                        "ExportCurves",
                        "ExportContours",
                        "SimplifyGeometry",
                        "ConvertAnalyticGeometryToNurbs",
                        "SaveSolidBodiesAsFaceSet",
                        "ImportWireBodies",
                        "ImportMeshBodies",
                        "ImportAnotations"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (6) Import [mode != 0]
                {
                    TargetProperties = new[]
                    {
                        "FileNameSuffix",
                        "TemplateFileName",
                        "Version",
                        "Protocol",
                        "ExportMode",
                        "ColorSource",
                        "ExportSolidBodies",
                        "ExportSheetBodies",
                        "ExportWireBodies",
                        "Export3DPictures",
                        "ExportAnotation",
                        "ExportWelds",
                        "ExportCurves",
                        "ExportContours",
                        "SimplifyGeometry",
                        "ConvertAnalyticGeometryToNurbs",
                        "SaveSolidBodiesAsFaceSet"
                    },
                    IsBrowsable = false
                }
            };
            #endregion
        }

        #region window proc
        private IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case WM_STOPPED_PROCESSING:
                    button3_2.IsEnabled = false;
                    menuItem3_2.IsEnabled = false;
                    tvControl2.UpdateControl();
                    sb_label2.Content = string.Format(Properties.Resources.Strings["ui_0:sbl_2"][0],
                        tvControl2.CountFiles);
                    UpdateStateToControls();
                    break;
                case WM_INCREMENT_PROGRESS:
                    double[] value = new double[1];
                    Marshal.Copy(lParam, value, 0, value.Length);
                    progressBar.Value = value[0];
                    if (value[0] == 100)
                    {
                        progressBar.Value = 0.0;
                        progressBar.Visibility = Visibility.Hidden;
                    }
                    break;
            }

            return NativeMethods.CallWindowProc(oldWndProc, hWnd, uMsg, wParam, lParam);
        }
        #endregion

        #region main window
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (handle == IntPtr.Zero)
            {
                handle = new WindowInteropHelper(this).Handle;
                newWndProc = new NativeMethods.WndProc(WindowProc);
                oldWndProc = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWLP_WNDPROC);
                NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWLP_WNDPROC,
                    Marshal.GetFunctionPointerForDelegate(newWndProc));
            }

            if (comboBox1.Items.Count == 0 && conf.Configurations.Count > 0)
            {
                for (int i = 0; i < conf.Configurations.Count; i++)
                {
                    var h = conf.Configurations.ElementAt(i);
                    h.Value.PropertyChanged += Header_PropertyChanged;
                    comboBox1.Items.Add(h.Key);

                    var m = h.Value.Modules as Modules;
                    m.PropertyChanged += Modules_PropertyChanged;
                }

                comboBox1.SelectedIndex = 0;
            }
            else
            {
                UpdateStateToControls();
            }

            menuItem2_1.IsEnabled = false;
            menuItem2_2.IsEnabled = false;
            menuItem3_1.IsEnabled = false;
            menuItem3_2.IsEnabled = false;

            button2_1.IsEnabled = false;
            button2_2.IsEnabled = false;
            button3_1.IsEnabled = false;
            button3_2.IsEnabled = false;

            propertyGrid.PropertyValueChanged += Translator_PropertyValueChanged;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // ..
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !QueryOnSaveChanges();
        }
        #endregion

        #region configuration event handlers
        private void Header_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Header cfg))
                return;

            switch (e.PropertyName)
            {
                case "InitialCatalog":
                    tvControl1.RootDirectory = cfg.InitialCatalog;
                    break;
                case "TargetDirectory":
                    tvControl2.RootDirectory = cfg.TargetDirectory;
                    break;
                case "Translator":
                    propertyGrid.SelectedObject = cfg.Translator;
                    SetProcessingMode(cfg);
                    SetWindowTitle(cfg.Translator);
                    (cfg.Modules as Modules).PropertyChanged += Modules_PropertyChanged;
                    break;
            }

            UpdateStateToControls();

            //Debug.WriteLine(string.Format("Header_PropertyChanged [name: {0}]",
            //    e.PropertyName));
        }

        private void Modules_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var m = sender as Modules;
            UpdateModule(m, e.PropertyName);
        }

        private void Translator_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            var obj = conf.Configurations[key1].Translator as Translator;
            var item = e.OriginalSource as PropertyItem;
            if (item.PropertyName == "ImportMode")
            {
                importMode = (int)e.NewValue;
                UpdateModules((int)obj.TMode, 2);
            }

            UpdateStateToControls();

            //Debug.WriteLine(string.Format("Translator_PropertyValueChanged: [name: {0}, value: {1}]", 
            //    item.PropertyName, item.Value));
        }
        #endregion

        #region menubar & toolbar events
        private void Event1_1_Click(object sender, RoutedEventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            string newKey = null, directory = null;
            using (var sfd = new CommonSaveFileDialog())
            {
                sfd.Title = Properties.Resources.Strings["ui_0:c_1_1"][0];
                sfd.InitialDirectory = conf.UserDirectory;
                sfd.DefaultFileName = string.Format("configuration_{0}", conf.Configurations.Count);
                sfd.DefaultExtension = "config";
                sfd.Filters.Add(new CommonFileDialogFilter("Configuration Files", "*.config"));

                if (sfd.ShowDialog() == CommonFileDialogResult.Cancel)
                {
                    sfd.Dispose();
                    return;
                }

                newKey = Path.GetFileNameWithoutExtension(sfd.FileName);
                directory = Path.GetDirectoryName(sfd.FileName);
                sfd.Dispose();
            }

            if (directory != conf.UserDirectory)
            {
                if (QueryOnSaveChanges())
                {
                    conf.UserDirectory = directory;
                    comboBox1.Items.Clear();
                }
                else
                    return;
            }

            Header header = new Header(newKey);
            header.PropertyChanged += Header_PropertyChanged;

            if (key1 != newKey)
            {
                key1 = newKey;

                conf.Configurations.Add(key1, header);
                conf.SetConfiguration(key1);

                comboBox1.Items.Add(key1);
                index = comboBox1.Items.Count - 1;
            }
            else
            {
                conf.Configurations[key1] = header;
                conf.SetConfiguration(key1);

                comboBox1.SelectedIndex = -1;
            }

            comboBox1.SelectedIndex = index;
        } // New configuration

        private void Event1_2_Click(object sender, RoutedEventArgs e)
        {

        } // Open configuration

        private void Event1_3_Click(object sender, RoutedEventArgs e)
        {
            string directory = null;
            using (var ofd = new CommonOpenFileDialog())
            {
                ofd.Title = Properties.Resources.Strings["ui_0:c_1_2"][1];
                ofd.Multiselect = false;
                ofd.IsFolderPicker = true;
                ofd.InitialDirectory = conf.UserDirectory;

                if (ofd.ShowDialog() == CommonFileDialogResult.Cancel)
                {
                    ofd.Dispose();
                    return;
                }

                directory = ofd.FileName;
                ofd.Dispose();
            }

            if (directory != conf.UserDirectory)
            {
                if (QueryOnSaveChanges())
                {
                    conf.UserDirectory = directory;
                    comboBox1.Items.Clear();
                }
                else
                    return;
            }

            if (conf.Configurations.Count > 0)
            {
                foreach (var i in conf.Configurations.Keys)
                {
                    comboBox1.Items.Add(i);
                }

                comboBox1.SelectedIndex = 0;
            }
        } // Open target directory

        private void Event1_4_Click(object sender, RoutedEventArgs e)
        {
            conf.SetConfiguration(key1);
            UpdateStateToControls();
        } // Save configuration

        private void Event1_5_Click(object sender, RoutedEventArgs e)
        {
            conf.SetConfigurations();
            UpdateStateToControls();
        } // Save all configurations

        private void Event1_6_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                string.Format(Properties.Resources.Strings["ui_0:msg_1"][0], key1),
                Properties.Resources.AppName,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                int index = -1;
                int count = conf.Configurations.Count;
                string oldKey = key1;

                for (int i = 0; i < count; i++)
                {
                    var cfg = conf.Configurations.ElementAt(i);
                    if (cfg.Key == oldKey)
                    {
                        if ((count -1) > i)
                        {
                            index = i;
                            key1 = conf.Configurations.ElementAt(i + 1).Key;
                        }
                        else if ((count -1) > 0 && (count -1) == i)
                        {
                            index = (i - 1);
                            key1 = conf.Configurations.ElementAt(i - 1).Key;
                        }
                        else
                        {
                            key1 = string.Empty;
                        }
                        break;
                    }
                }

                conf.Configurations.Remove(oldKey);
                comboBox1.Items.Remove(oldKey);

                if (index == -1)
                    propertyGrid.PropertyDefinitions.Clear();
                else
                    comboBox1.SelectedIndex = index;
            }
        } // Delete configuration

        private void Event1_7_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI headerUI = new PropertiesUI(conf.Configurations[key1])
            {
                Title = Properties.Resources.Strings["ui_0:c_1_7"][1],
                Owner = this
            };
            headerUI.ShowDialog();
        } // Header properties

        private void Event1_8_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Application exit

        private void Event2_1_Click(object sender, RoutedEventArgs e)
        {
            UndoRedoManager.Undo();
            propertyGrid.Update();
            
        } // Undo

        private void Event2_2_Click(object sender, RoutedEventArgs e)
        {
            UndoRedoManager.Redo();
            propertyGrid.Update();
        } // Redo

        private void Event3_1_Click(object sender, RoutedEventArgs e)
        {
            stoped = false;
            thread = new System.Threading.Thread(StartProcessing);
            thread.Start();
            button3_2.IsEnabled = true;
            menuItem3_2.IsEnabled = true;
            progressBar.Visibility = Visibility.Visible;
        } // Start processing

        private void Event3_2_Click(object sender, RoutedEventArgs e)
        {
            stoped = true;
        } // Stop processing

        private void Event3_3_Click(object sender, RoutedEventArgs e)
        {
            tvControl2.CleanRootDirectory();
            UpdateStateToControls();
        } // Clear target directory

        private void Event4_1_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI optionsUI = new PropertiesUI(options)
            {
                Title = Properties.Resources.Strings["ui_0:c_4_1"][0],
                Owner = this
            };
            optionsUI.ShowDialog();
        } // Options

        private void Event4_2_Click(object sender, RoutedEventArgs e)
        {
            var cfg = conf.Configurations[key1];
            var obj = cfg.Translator as Translator;

            if (obj.TMode == TranslatorType.Document)
            {
                var value = tvControl1.EnableAsmTree;
                if (sender is MenuItem mi && mi.IsChecked != value)
                {
                    value = mi.IsChecked;
                    tvControl1.EnableAsmTree = value;
                    tvControl2.EnableAsmTree = value;
                    button4_2.IsChecked      = value;
                }
                else if (sender is ToggleButton tb && tb.IsChecked != value)
                {
                    value = (bool)tb.IsChecked;
                    tvControl1.EnableAsmTree = value;
                    tvControl2.EnableAsmTree = value;
                    menuItem4_2.IsChecked    = value;
                }
            }
        } // Assembly tree view enable/disable

        private void Event5_1_Click(object sender, RoutedEventArgs e)
        {
            AboutUs aboutUs = new AboutUs
            {
                Owner = this
            };
            aboutUs.ShowDialog();
        } // About Us

        private void Event5_2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(null,
                Path.Combine(Properties.Resources.AppDirectory, "TFPackageManager.chm"));
        } // Help

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                key1 = comboBox1.SelectedValue.ToString();
                var cfg = conf.Configurations[key1];

                tvControl1.RootDirectory    = cfg.InitialCatalog;
                tvControl2.RootDirectory    = cfg.TargetDirectory;
                inputPath1.SelectedObject   = cfg;
                inputPath2.SelectedObject   = cfg;
                propertyGrid.SelectedObject = cfg.Translator;
                SetProcessingMode(cfg);
                SetWindowTitle(cfg.Translator);
            }
            else
            {
                tvControl1.RootDirectory    = string.Empty;
                tvControl2.RootDirectory    = string.Empty;
                inputPath1.SelectedObject   = null;
                inputPath2.SelectedObject   = null;
                propertyGrid.SelectedObject = null;
                SetWindowTitle(null);
                comboBox2.Items.Clear();
            }
            UpdateStateToControls();

            //Debug.WriteLine(string.Format("selector_1: [index: {0}, value: {1}]",
            //    comboBox1.SelectedIndex, 
            //    comboBox1.SelectedValue));
        } // configuration list

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                key2 = comboBox2.SelectedValue.ToString();
                var cfg = conf.Configurations[key1];
                var obj = cfg.Translator as Translator;

                switch (key2)
                {
                    case "SaveAs":
                        obj.PMode = ProcessingMode.SaveAs;
                        UpdateModules((int)obj.TMode, 0);
                        tvControl1.SearchPattern = "*.grb";
                        break;
                    case "Export":
                        obj.PMode = ProcessingMode.Export;
                        UpdateModules((int)obj.TMode, 1);
                        tvControl1.SearchPattern = "*.grb";
                        break;
                    case "Import":
                        obj.PMode = ProcessingMode.Import;
                        UpdateModules((int)obj.TMode, 2);
                        switch (obj.TMode)
                        {
                            case TranslatorType.Acis:
                                tvControl1.SearchPattern = "*.sat";
                                break;
                            case TranslatorType.Iges:
                                tvControl1.SearchPattern = "*.igs";
                                break;
                            case TranslatorType.Jt:
                                tvControl1.SearchPattern = "*.jt";
                                break;
                            case TranslatorType.Step:
                                tvControl1.SearchPattern = "*.stp";
                                break;
                        }
                        break;
                }
            }
        } // processing mode
        #endregion

        #region statusbar
        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (tvControl1.SelectedItems.Count > 0 &&
                tvControl1.RootDirectory.Length > 0 &&
                tvControl2.RootDirectory.Length > 0)
            {
                menuItem3_1.IsEnabled = true;
                button3_1.IsEnabled = true;
            }
            else
            {
                menuItem3_1.IsEnabled = false;
                button3_1.IsEnabled = false;
            }
            var str = Properties.Resources.Strings["ui_0:sbl_3"][0];
            sb_label3.Content = string.Format(str, tvControl1.SelectedItems.Count);
        }
        #endregion

        #region extension methods
        /// <summary>
        /// Set main window title.
        /// </summary>
        /// <param name="translator"></param>
        private void SetWindowTitle(object translator)
        {
            if (translator != null)
            {
                var type = (translator as Translator).TMode.ToString();
                Title = string.Format("{0} [{1}]", Properties.Resources.AppName, type);
            }
            else
                Title = Properties.Resources.AppName;
        }

        /// <summary>
        /// Set current processing mode.
        /// </summary>
        /// <param name="cfg"></param>
        private void SetProcessingMode(Header cfg)
        {
            var obj = cfg.Translator;
            int ind = (int)(cfg.Translator as Translator).PMode;

            comboBox2.Items.Clear();

            switch ((obj as Translator).TMode)
            {
                case TranslatorType.Document:
                    comboBox2.Items.Add("SaveAs");
                    comboBox2.SelectedIndex = 0;
                    break;
                case TranslatorType.Acad:
                case TranslatorType.Bitmap:
                case TranslatorType.Pdf:
                    comboBox2.Items.Add("Export");
                    comboBox2.SelectedIndex = 0;
                    break;
                case TranslatorType.Acis:
                case TranslatorType.Iges:
                case TranslatorType.Jt:
                case TranslatorType.Step:
                    importMode = (obj as Translator3D).ImportMode;
                    comboBox2.Items.Add("Export");
                    comboBox2.Items.Add("Import");
                    comboBox2.SelectedIndex = ind > 1 ? 1 : 0;
                    break;
            }
        }

        /// <summary>
        /// Update state of extension module.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="name"></param>
        private void UpdateModule(Modules m, string name)
        {
            switch (name)
            {
                case "Links":
                    if (m.Links)
                        propertyGrid.PropertyDefinitions.Remove(modules[0]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[0]);
                    break;
                case "Pages":
                    if (m.Pages)
                        propertyGrid.PropertyDefinitions.Remove(modules[1]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[1]);
                    break;
                case "Projections":
                    if (m.Projections)
                        propertyGrid.PropertyDefinitions.Remove(modules[2]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[2]);
                    break;
                case "Variables":
                    if (m.Variables)
                        propertyGrid.PropertyDefinitions.Remove(modules[3]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[3]);
                    break;
            }

            //Debug.WriteLine(string.Format("UpdateModules [name: {0}, len {1}]",
            //    name, propertyGrid.PropertyDefinitions.Count));
        }

        /// <summary>
        /// Update state all modules.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        private void UpdateModules(int type, int mode)
        {
            var m = conf.Configurations[key1].Modules as Modules;
            propertyGrid.PropertyDefinitions.Clear();

            if (type == 0 || type == 1 || type == 3 || type == 6)
            {
                foreach (var p in m.GetType().GetProperties())
                {
                    UpdateModule(m, p.Name);
                }
            }

            if (mode == 2)
            {
                if (importMode != 0) // import mode on translator level
                    propertyGrid.PropertyDefinitions.Add(modules[6]);
                else
                    propertyGrid.PropertyDefinitions.Add(modules[5]);
            }
            else
            {
                propertyGrid.PropertyDefinitions.Add(modules[4]);
            }

            //Debug.WriteLine(string.Format("InitModules [type: {0}, mode: {1}, len {2}]",
            //    type, mode, propertyGrid.PropertyDefinitions.Count));
        }

        /// <summary>
        /// Extension method to query on save changes.
        /// </summary>
        /// <returns></returns>
        private bool QueryOnSaveChanges()
        {
            if (conf.HasChanged)
            {
                MessageBoxResult result = MessageBox.Show(
                    Properties.Resources.Strings["ui_0:msg_2"][0],
                    Properties.Resources.AppName,
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        conf.SetConfigurations();
                        UpdateStateToControls();
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extension method to update state to controls.
        /// </summary>
        private void UpdateStateToControls()
        {
            if (conf.Configurations.Count() == 0)
            {
                menuItem1_4.IsEnabled = false; // save
                menuItem1_5.IsEnabled = false; // save all
                menuItem1_6.IsEnabled = false; // delete
                menuItem1_7.IsEnabled = false; // properties
                menuItem2_1.IsEnabled = false; // undo
                menuItem2_2.IsEnabled = false; // redo
                menuItem3_1.IsEnabled = false; // start
                menuItem3_3.IsEnabled = false; // clear target directory
                menuItem4_2.IsEnabled = false; // assembly tree view

                button1_4.IsEnabled = false;
                button1_5.IsEnabled = false;
                button1_6.IsEnabled = false;
                button1_7.IsEnabled = false;
                button2_1.IsEnabled = false;
                button2_2.IsEnabled = false;
                button3_1.IsEnabled = false;
                button3_3.IsEnabled = false;
                button4_2.IsEnabled = false;
                return;
            }
            else
            {
                menuItem1_3.IsEnabled = true;
                menuItem1_6.IsEnabled = true;
                menuItem1_7.IsEnabled = true;

                button1_3.IsEnabled = true;
                button1_6.IsEnabled = true;
                button1_7.IsEnabled = true;

                if (tvControl2.CountFiles > 0)
                {
                    menuItem3_3.IsEnabled = true;
                    button3_3.IsEnabled   = true;
                }
                else
                {
                    menuItem3_3.IsEnabled = false;
                    button3_3.IsEnabled   = false;
                }
            }

            var cfg = conf.Configurations[key1];
            var obj = cfg.Translator as Translator;

            if (cfg.IsChanged && !cfg.IsInvalid)
            {
                menuItem1_4.IsEnabled = true;
                button1_4.IsEnabled   = true;
            }
            else
            {
                menuItem1_4.IsEnabled = false;
                button1_4.IsEnabled   = false;
            }

            if (conf.HasChanged && !cfg.IsInvalid)
            {
                menuItem1_5.IsEnabled = true;
                button1_5.IsEnabled   = true;
            }
            else
            {
                menuItem1_5.IsEnabled = false;
                button1_5.IsEnabled   = false;
            }

            if (obj.TMode == TranslatorType.Document)
            {
                menuItem4_2.IsEnabled = true;
                button4_2.IsEnabled   = true;
            }
            else
            {
                tvControl1.EnableAsmTree = false;
                tvControl2.EnableAsmTree = false;
                button4_2.IsChecked      = false;
                button4_2.IsEnabled      = false;
                menuItem4_2.IsChecked    = false;
                menuItem4_2.IsEnabled    = false;
            }

            var str1 = Properties.Resources.Strings["ui_0:sbl_1"][0];
            var str2 = Properties.Resources.Strings["ui_0:sbl_2"][0];

            sb_label1.Content = string.Format(str1, tvControl1.CountFiles);
            sb_label2.Content = string.Format(str2, tvControl2.CountFiles);
        }

        /// <summary>
        /// Extension method to running processing documents.
        /// </summary>
        private void StartProcessing()
        {
            var path = conf.Configurations[key1].TargetDirectory;
            var logFile = Path.Combine(path, Properties.Resources.LOG_FILE);

            using (StreamWriter logger = new StreamWriter(logFile))
            {
                try
                {
                    ProcessingTask(logger);
                    logger.Close();

                    if (options.OpenLogFile)
                        Process.Start("notepad.exe", logFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void ProcessingTask(StreamWriter logger)
        {
            Header cfg = conf.Configurations[key1];
            double[] counter = { 0.0 };
            double increment = 100.0 / tvControl1.SelectedItems.Count;
            var size = Marshal.SizeOf(counter[0]) * counter.Length;
            IntPtr value = Marshal.AllocHGlobal(size);

            Application.FileLinksAutoRefresh = Application.FileLinksRefreshMode.AutoRefresh;

            Logging logging = new Logging(logger);
            Processing proc = new Processing(cfg, logging);

            logging.PrintHelper();
            logging.WriteLine(LogLevel.INFO, "Started processing");
            logging.WriteLine(LogLevel.INFO, 
                string.Format("Translator [type: {0}, mode: {1}]", 
                (cfg.Translator as Translator).TMode, 
                (cfg.Translator as Translator).PMode));

            foreach (var item in tvControl1.SelectedItems.OrderBy(i => i.Key))
            {
                if ((item.Value.Flags & 0x1) != 0x1)
                    continue;

                if (stoped)
                {
                    NativeMethods.SendMessage(handle, WM_STOPPED_PROCESSING, 
                        IntPtr.Zero, IntPtr.Zero);
                    logging.WriteLine(LogLevel.INFO, "Processing stopped");
                    break;
                }

                logging.WriteLine(LogLevel.INFO, 
                    string.Format("Processing [path: {0}]", item.Value.IPath));
                proc.ProcessingFile(item.Value);

                counter[0] += increment;
                Marshal.Copy(counter, 0, value, counter.Length);
                NativeMethods.SendMessage(handle, WM_INCREMENT_PROGRESS, 
                    IntPtr.Zero, value);
            }

            counter[0] = 100;
            Marshal.Copy(counter, 0, value, counter.Length);
            NativeMethods.SendMessage(handle, WM_INCREMENT_PROGRESS, 
                IntPtr.Zero, value);
            NativeMethods.SendMessage(handle, WM_STOPPED_PROCESSING, 
                IntPtr.Zero, IntPtr.Zero);

            logging.WriteLine(LogLevel.INFO, "Processing ending");
        }
        #endregion
    }
}