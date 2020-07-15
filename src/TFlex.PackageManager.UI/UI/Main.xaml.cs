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
using System.Windows.Interop;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Model;
using Xceed.Wpf.Toolkit.PropertyGrid;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI
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

        readonly string[] tblabels;
        readonly string[] sblabels;
        readonly string[] messages;
        readonly string[] controls;
        readonly string[] tooltips;

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
            Title = Resource.AppName;

            #region initialize controls
            tvControl1.Content = new CustomTreeView
            {
                CheckboxesVisible = true
            };
            tvControl1.SearchPattern = "*.grb";
            tvControl1.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            tvControl2.Content = new CustomTreeView();
            tvControl2.SearchPattern = "*.grb|*.dwg|*.dxf|*.dxb|*.sat|*.bmp|*.jpeg|*.gif|*.tiff|*.png|*.igs|*.jt|*.pdf|*.stp";

            options = new Common.Options();

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
            messages = new string[]
            {
                Resource.GetString(Resource.MAIN_WINDOW, "message1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "message2", 0)
            };

            tblabels = new string[]
            {
                Resource.GetString(Resource.MAIN_WINDOW, "tb_label1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "tb_label2", 0)
            };

            tb_label1.Content = tblabels[0];
            tb_label2.Content = tblabels[1];

            sblabels = new string[]
            {
                Resource.GetString(Resource.MAIN_WINDOW, "sb_label1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "sb_label2", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "sb_label3", 0)
            };

            sb_label1.Content = string.Format(sblabels[0], 0);
            sb_label2.Content = string.Format(sblabels[1], 0);
            sb_label3.Content = string.Format(sblabels[2], 0);

            controls = new string[]
            {
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_8", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_2", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_3", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem4_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem5_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem5_2", 0)
            };

            tooltips = new string[]
            {
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_2", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_3", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem4_1", 1)
            };

            menuItem1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1", 0);
            menuItem2.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2", 0);
            menuItem3.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3", 0);
            menuItem4.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem4", 0);

            menuItem1_1.Header = controls[0];
            menuItem1_2.Header = controls[1];
            menuItem1_3.Header = controls[2];
            menuItem1_4.Header = controls[3];
            menuItem1_5.Header = controls[4];
            menuItem1_6.Header = controls[5];
            menuItem1_7.Header = controls[6];
            menuItem1_8.Header = controls[7];
            menuItem2_1.Header = controls[8];
            menuItem2_2.Header = controls[9];
            menuItem3_1.Header = controls[10];
            menuItem3_2.Header = controls[11];
            menuItem3_3.Header = controls[12];
            menuItem4_1.Header = controls[13];
            menuItem5_1.Header = controls[14];
            menuItem5_2.Header = controls[15];

            button1_1.ToolTip = tooltips[0];
            button1_2.ToolTip = tooltips[1];
            button1_3.ToolTip = tooltips[2];
            button1_4.ToolTip = tooltips[3];
            button1_5.ToolTip = tooltips[4];
            button1_6.ToolTip = tooltips[5];
            button1_7.ToolTip = tooltips[6];
            button2_1.ToolTip = tooltips[7];
            button2_2.ToolTip = tooltips[8];
            button3_1.ToolTip = tooltips[9];
            button3_2.ToolTip = tooltips[10];
            button3_3.ToolTip = tooltips[11];
            button4_1.ToolTip = tooltips[12];

            sb_label1.ToolTip = Resource.GetString(Resource.MAIN_WINDOW, "sb_label1", 1);
            sb_label2.ToolTip = Resource.GetString(Resource.MAIN_WINDOW, "sb_label2", 1);
            sb_label3.ToolTip = Resource.GetString(Resource.MAIN_WINDOW, "sb_label3", 1);

            inputPath1.PropertyDefinitions.Add(new PropertyDefinition
            {
                TargetProperties = new[]
                {
                    "ConfigurationName",
                    "TargetDirectory",
                    "Modules"
                },
                IsBrowsable = false
            });

            inputPath2.PropertyDefinitions.Add(new PropertyDefinition
            {
                TargetProperties = new[]
                {
                    "ConfigurationName",
                    "InitialCatalog",
                    "Modules"
                },
                IsBrowsable = false
            });

            modules = new List<PropertyDefinition>
            {
                new PropertyDefinition // (0) Pages
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
                new PropertyDefinition // (1) Projections
                {
                    TargetProperties = new[]
                    {
                        "ProjectionNames",
                        "ExcludeProjection",
                        "ProjectionScale"
                    },
                    IsBrowsable = false
                },
                new PropertyDefinition // (2) Variables
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
                new PropertyDefinition // (3) Export
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
                new PropertyDefinition // (4) Import
                {
                    TargetProperties = new[]
                    {
                        "FileNameSuffix",
                        "TemplateFileName",
                        "RenameSubdirectory",
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
                new PropertyDefinition // (5) Import [mode != 0]
                {
                    TargetProperties = new[]
                    {
                        "FileNameSuffix",
                        "TemplateFileName",
                        "RenameSubdirectory",
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
                    sb_label2.Content = string.Format(sblabels[1], tvControl2.CountFiles);
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
                    tvControl1.TargetDirectory = cfg.InitialCatalog;
                    break;
                case "TargetDirectory":
                    tvControl2.TargetDirectory = cfg.TargetDirectory;
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
                sfd.Title = controls[0];
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

            Header header = new Header()
            {
                UserDirectory = directory,
                ConfigurationName = newKey
            };

            header.PropertyChanged += Header_PropertyChanged;

            if (key1 != newKey)
            {
                key1 = newKey;

                conf.Configurations.Add(key1, header);
                conf.Configurations[key1].ConfigurationTask(1);

                comboBox1.Items.Add(key1);
                index = comboBox1.Items.Count - 1;
            }
            else
            {
                conf.Configurations[key1] = header;
                conf.Configurations[key1].ConfigurationTask(1);
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
                ofd.Title = tooltips[2];
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
            conf.Configurations[key1].ConfigurationTask(1);
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
                string.Format(messages[0], key1),
                Resource.AppName,
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
                Title = tooltips[6],
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
            tvControl2.CleanTargetDirectory();
            UpdateStateToControls();
        } // Clear target directory

        private void Event4_1_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI optionsUI = new PropertiesUI(options)
            {
                Title = controls[13],
                Owner = this
            };
            optionsUI.ShowDialog();
        } // Options

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
                Path.Combine(Resource.AppDirectory, "TFPackageManager.chm"));
        } // Help

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                key1 = comboBox1.SelectedValue.ToString();
                var cfg = conf.Configurations[key1];

                tvControl1.TargetDirectory  = cfg.InitialCatalog;
                tvControl2.TargetDirectory  = cfg.TargetDirectory;
                inputPath1.SelectedObject   = cfg;
                inputPath2.SelectedObject   = cfg;
                propertyGrid.SelectedObject = cfg.Translator;
                SetProcessingMode(cfg);
                SetWindowTitle(cfg.Translator);
            }
            else
            {
                tvControl1.TargetDirectory  = string.Empty;
                tvControl2.TargetDirectory  = string.Empty;
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

                cfg.Processing = (int)obj.PMode;
            }
        } // processing mode
        #endregion

        #region statusbar
        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (tvControl1.SelectedItems.Count > 0 &&
                tvControl1.TargetDirectory.Length > 0 &&
                tvControl2.TargetDirectory.Length > 0)
            {
                menuItem3_1.IsEnabled = true;
                button3_1.IsEnabled = true;
            }
            else
            {
                menuItem3_1.IsEnabled = false;
                button3_1.IsEnabled = false;
            }

            sb_label3.Content = string.Format(sblabels[2], tvControl1.SelectedItems.Count);
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
                Title = string.Format("{0} [{1}]", Resource.AppName, type);
            }
            else
                Title = Resource.AppName;
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
                    propertyGrid.PropertyDefinitions.Add(modules[5]);
                else
                    propertyGrid.PropertyDefinitions.Add(modules[4]);
            }
            else
            {
                propertyGrid.PropertyDefinitions.Add(modules[3]);
            }

            //Debug.WriteLine(string.Format("InitModules [type: {0}, mode: {1}, len {2}]",
            //    type, mode, propertyGrid.PropertyDefinitions.Count));
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
                    break;
                case "Pages":
                    if (m.Pages)
                        propertyGrid.PropertyDefinitions.Remove(modules[0]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[0]);
                    break;
                case "Projections":
                    if (m.Projections)
                        propertyGrid.PropertyDefinitions.Remove(modules[1]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[1]);
                    break;
                case "Variables":
                    if (m.Variables)
                        propertyGrid.PropertyDefinitions.Remove(modules[2]);
                    else
                        propertyGrid.PropertyDefinitions.Add(modules[2]);
                    break;
            }

            //Debug.WriteLine(string.Format("UpdateModules [name: {0}, len {1}]",
            //    name, propertyGrid.PropertyDefinitions.Count));
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
                    messages[1],
                    Resource.AppName,
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
            //Debug.WriteLine("UpdateStateToControls");

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

                button1_4.IsEnabled = false;
                button1_5.IsEnabled = false;
                button1_6.IsEnabled = false;
                button1_7.IsEnabled = false;
                button2_1.IsEnabled = false;
                button2_2.IsEnabled = false;
                button3_1.IsEnabled = false;
                button3_3.IsEnabled = false;
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
                    button3_3.IsEnabled = true;
                }
                else
                {
                    menuItem3_3.IsEnabled = false;
                    button3_3.IsEnabled = false;
                }
            }

            if (conf.Configurations[key1].IsChanged &&
                conf.Configurations[key1].IsInvalid == false)
            {
                menuItem1_4.IsEnabled = true;
                button1_4.IsEnabled = true;
            }
            else
            {
                menuItem1_4.IsEnabled = false;
                button1_4.IsEnabled = false;
            }

            if (conf.HasChanged &&
                conf.Configurations[key1].IsInvalid == false)
            {
                menuItem1_5.IsEnabled = true;
                button1_5.IsEnabled = true;
            }
            else
            {
                menuItem1_5.IsEnabled = false;
                button1_5.IsEnabled = false;
            }

            sb_label1.Content = string.Format(sblabels[0], tvControl1.CountFiles);
            sb_label2.Content = string.Format(sblabels[1], tvControl2.CountFiles);
        }

        /// <summary>
        /// Extension method to running processing documents.
        /// </summary>
        private void StartProcessing()
        {
            var path = conf.Configurations[key1].TargetDirectory;
            var logFile = Path.Combine(path, Resource.LOG_FILE);

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
            List<ProcItem> pItems = new List<ProcItem>();
            string[] items = tvControl1.SelectedItems.OrderBy(i => i).Cast<string>().ToArray();
            for (int i = 0; i < items.Length; i++)
                pItems.Add(new ProcItem(items[i]));

            double[] counter = { 0.0 };
            double increment = 100.0 / items.Length;
            var size = Marshal.SizeOf(counter[0]) * counter.Length;
            IntPtr value = Marshal.AllocHGlobal(size);

            TFlex.Application.FileLinksAutoRefresh = TFlex.Application.FileLinksRefreshMode.AutoRefresh;

            Logging logging = new Logging(logger);
            Processing proc = new Processing(cfg, logging);

            logging.WriteLine(LogLevel.INFO, "Started processing");
            logging.WriteLine(LogLevel.INFO, 
                string.Format("Translator [type: {0}, mode: {1}]", 
                (cfg.Translator as Translator).TMode, 
                (cfg.Translator as Translator).PMode));

            foreach (var i in pItems)
            {
                if (stoped)
                {
                    NativeMethods.SendMessage(handle, WM_STOPPED_PROCESSING, 
                        IntPtr.Zero, IntPtr.Zero);
                    logging.WriteLine(LogLevel.INFO, "Processing stopped");
                    break;
                }

                logging.WriteLine(LogLevel.INFO, 
                    string.Format("Processing [path: {0}]", i.IPath));
                proc.ProcessingFile(i);

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

        private void SetProcessingMode(Header header)
        {
            var obj = header.Translator;
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
                    comboBox2.SelectedIndex = header.Processing > 1 ? 1 : 0;
                    break;
            }
        }
        #endregion
    }
}