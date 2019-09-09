﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private fields
        private readonly ConfigurationCollection self;

        private readonly CustomTreeView treeListView1;
        private readonly CustomTreeView treeListView2;

        private readonly Common.Options options;

        private readonly string[] tblabels;
        private readonly string[] sblabels;
        private readonly string[] messages;
        private readonly string[] controls;
        private readonly string[] tooltips;

        private string key1, key2, key3;
        private int importMode;

        private System.Threading.Thread thread;
        private bool stoped;

        private static IntPtr handle = IntPtr.Zero;
        private static IntPtr oldWndProc = IntPtr.Zero;
        private NativeMethods.WndProc newWndProc;

        const int WM_STOPPED_PROCESSING = 0x0400;
        const int WM_INCREMENT_PROGRESS = 0x0401;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Title = Resource.AppName;

            #region initialize controls
            treeListView1 = new CustomTreeView { CheckboxesVisible = true };

            tvControl1.Content = treeListView1;
            tvControl1.SearchPattern = "*.grb";
            tvControl1.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            treeListView2 = new CustomTreeView();

            tvControl2.Content = treeListView2;
            tvControl2.SearchPattern = "*.grb|*.dwg|*.dxf|*.dxb|*.sat|*.bmp|*.jpeg|*.gif|*.tiff|*.png|*.igs|*.jt|*.pdf|*.stp";

            options = new Common.Options();
            

            using (UndoRedoManager.Start("Init"))
            {
                self = new ConfigurationCollection
                {
                    TargetDirectory = options.UserDirectory
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
            Resource.InitPaths();

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
                    "InputExtension",
                    "TranslatorTypes"
                },
                IsBrowsable = false
            });

            inputPath2.PropertyDefinitions.Add(new PropertyDefinition
            {
                TargetProperties = new[]
                {
                    "ConfigurationName",
                    "InitialCatalog",
                    "InputExtension",
                    "TranslatorTypes"
                },
                IsBrowsable = false
            });

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
                    sb_label2.Content = string.Format(sblabels[1], tvControl2.CountFiles);
                    tvControl2.InitLayout();
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

            propertyGrid.SelectedObjectChanged += PropertyGrid_SelectedObjectChanged;

            if (comboBox1.Items.Count == 0 && self.Configurations.Count > 0)
            {
                for (int i = 0; i < self.Configurations.Count; i++)
                {
                    self.Configurations.ElementAt(i).Value.PropertyChanged += Header_PropertyChanged;
                    self.Configurations.ElementAt(i).Value.TranslatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;
                    comboBox1.Items.Add(self.Configurations.ElementAt(i).Key);
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

        private void PropertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // ..
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
            switch (e.PropertyName)
            {
                case "InitialCatalog":
                    tvControl1.TargetDirectory = self.Configurations[key1].InitialCatalog;
                    break;
                case "TargetDirectory":
                    tvControl2.TargetDirectory = key2 != null
                        ? Path.Combine(self.Configurations[key1].TargetDirectory, key2)
                        : self.Configurations[key1].TargetDirectory;
                    break;
            }

            UpdateStateToControls();
        }

        private void TranslatorTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int selected_index = comboBox2.SelectedIndex;
            int contains_index = comboBox2.Items.IndexOf(e.PropertyName);
            bool t_value       = false;

            switch (e.PropertyName)
            {
                case "Document": t_value = (sender as TranslatorTypes).Document; break;
                case "Acad"    : t_value = (sender as TranslatorTypes).Acad;     break;
                case "Acis"    : t_value = (sender as TranslatorTypes).Acis;     break;
                case "Bitmap"  : t_value = (sender as TranslatorTypes).Bitmap;   break;
                case "Iges"    : t_value = (sender as TranslatorTypes).Iges;     break;
                case "Jt"      : t_value = (sender as TranslatorTypes).Jt;       break;
                case "Pdf"     : t_value = (sender as TranslatorTypes).Pdf;      break;
                case "Step"    : t_value = (sender as TranslatorTypes).Step;     break;
            }

            if (t_value)
            {
                comboBox2.Items.Add(e.PropertyName);
                comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
            }
            else
            {
                comboBox2.Items.Remove(e.PropertyName);
                comboBox2.SelectedIndex = selected_index != contains_index
                    ? selected_index
                    : selected_index - 1;
            }
        }

        private void Translator_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            var item = e.OriginalSource as PropertyItem;
            if (item.PropertyName == "ImportMode")
            {
                propertyGrid.PropertyDefinitions.Clear();
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition
                {
                    TargetProperties = GetTargetProperties((int)e.NewValue),
                    IsBrowsable = false
                });
            }
            
            UpdateStateToControls();

            //Debug.WriteLine(string.Format("PropertyGrid_ValueChanged: [name: {0}, value: {1}]", 
            //    item.PropertyName, item.Value));
        }
        #endregion

        #region menubar & toolbar events
        private void Event1_1_Click(object sender, RoutedEventArgs e)
        {
            CommonSaveFileDialog sfd = new CommonSaveFileDialog
            {
                Title            = controls[0],
                InitialDirectory = self.TargetDirectory,
                DefaultFileName  = string.Format("configuration_{0}", self.Configurations.Count),
                DefaultExtension = "config"
            };

            sfd.Filters.Add(new CommonFileDialogFilter("Configuration Files", "*.config"));

            if (sfd.ShowDialog() == CommonFileDialogResult.Cancel)
                return;

            string newKey = Path.GetFileNameWithoutExtension(sfd.FileName);
            var directory = Path.GetDirectoryName(sfd.FileName);
            int index     = comboBox1.SelectedIndex;

            if (directory != self.TargetDirectory)
            {
                if (QueryOnSaveChanges())
                {
                    self.TargetDirectory = directory;
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
            header.TranslatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;

            if (key1 != newKey)
            {
                key1 = newKey;

                self.Configurations.Add(key1, header);
                self.Configurations[key1].ConfigurationTask(1);

                comboBox1.Items.Add(key1);
                index = comboBox1.Items.Count - 1;
            }
            else
            {
                self.Configurations[key1] = header;
                self.Configurations[key1].ConfigurationTask(1);

                comboBox1.SelectedIndex = -1;
            }

            comboBox1.SelectedIndex = index;

            sfd.Dispose();
        } // New configuration

        private void Event1_2_Click(object sender, RoutedEventArgs e)
        {

        } // Open configuration

        private void Event1_3_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                Title            = tooltips[2],
                Multiselect      = false,
                IsFolderPicker   = true,
                InitialDirectory = self.TargetDirectory
            };

            if (ofd.ShowDialog() == CommonFileDialogResult.Cancel)
                return;

            if (ofd.FileName != self.TargetDirectory)
            {
                if (QueryOnSaveChanges())
                {
                    self.TargetDirectory = ofd.FileName;
                    comboBox1.Items.Clear();
                }
                else
                    return;
            }

            if (self.Configurations.Count > 0)
            {
                foreach (var i in self.Configurations.Keys)
                {
                    comboBox1.Items.Add(i);
                }

                comboBox1.SelectedIndex = 0;
            }

            ofd.Dispose();
        } // Open target directory

        private void Event1_4_Click(object sender, RoutedEventArgs e)
        {
            self.Configurations[key1].ConfigurationTask(1);
            UpdateStateToControls();
        } // Save configuration

        private void Event1_5_Click(object sender, RoutedEventArgs e)
        {
            self.SetConfigurations();
            UpdateStateToControls();
        } // Save all configurations

        private void Event1_6_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                string.Format(messages[0], key1),
                "T-FLEX Package Manager",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                int index = -1;
                string oldKey = key1;

                foreach (var i in self.Configurations)
                {
                    if (i.Key == oldKey)
                    {
                        break;
                    }
                    else
                    {
                        key1 = i.Key;
                        index++;
                    }
                }

                self.Configurations[oldKey].ConfigurationTask(2);
                self.Configurations.Remove(oldKey);
                comboBox1.Items.Remove(oldKey);
                comboBox1.SelectedIndex = index;

                if (index == -1) key1 = string.Empty;
            }
        } // Delete configuration

        private void Event1_7_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI headerUI = new PropertiesUI(self.Configurations[key1])
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
                Title = controls[11],
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
                tvControl1.TargetDirectory = self.Configurations[key1].InitialCatalog;
                inputPath1.SelectedObject  = self.Configurations[key1];
                inputPath2.SelectedObject  = self.Configurations[key1];

                string[] items1 = comboBox2.Items.OfType<string>().ToArray();
                string[] items2 = self.Configurations[key1].Translators.Keys.ToArray();

                if (!Enumerable.SequenceEqual(items1, items2))
                {
                    comboBox2.Items.Clear();

                    foreach (var i in self.Configurations[key1].Translators)
                    {
                        comboBox2.Items.Add(i.Key);
                    }

                    comboBox2.SelectedIndex = 0;
                }
            }
            else
            {
                tvControl1.TargetDirectory = string.Empty;
                tvControl2.TargetDirectory = string.Empty;
                inputPath1.SelectedObject  = null;
                inputPath2.SelectedObject  = null;
                comboBox2.Items.Clear();
            }

            UpdateStateToControls();

            //Debug.WriteLine(string.Format("ComboBox1_SelectionChanged: [index: {0}, value: {1}]",
            //    comboBox1.SelectedIndex,
            //    comboBox1.SelectedValue));
        } // configuration list

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                key2 = comboBox2.SelectedValue.ToString();
                object t_mode = self.Configurations[key1].Translators[key2];
                propertyGrid.SelectedObject = t_mode;

                switch (key2)
                {
                    case "Acis": importMode = (t_mode as Translator_2).ImportMode; break;
                    case "Iges": importMode = (t_mode as Translator_6).ImportMode; break;
                    case "Jt"  : importMode = (t_mode as Translator_7).ImportMode; break;
                    case "Step": importMode = (t_mode as Translator_10).ImportMode; break;
                }

                uint p_mode = (t_mode as Translator).Processing;

                comboBox3.Items.Clear();

                switch (p_mode)
                {
                    case 0:
                        comboBox3.Items.Add("SaveAs");
                        break;
                    case 1:
                        comboBox3.Items.Add("Export");
                        break;
                    case 3:
                        comboBox3.Items.Add("Export");
                        comboBox3.Items.Add("Import");
                        break;
                }

                if (comboBox3.Items.Count > 0)
                    comboBox3.SelectedIndex = 0;

                var o_path = self.Configurations[key1].TargetDirectory;
                if (o_path.Length > 0)
                {
                    tvControl2.TargetDirectory = Path.Combine(o_path, key2);
                }
            }
            else
            {
                propertyGrid.SelectedObject = null;
                comboBox3.Items.Clear();
            }

            UpdateStateToControls();

            //Debug.WriteLine(string.Format("ComboBox2_SelectionChanged: [{0}, {1}]",
            //    comboBox2.SelectedIndex,
            //    comboBox2.SelectedValue));
        } // translator mode

        private void ComboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox3.SelectedIndex != -1)
            {
                key3 = comboBox3.SelectedValue.ToString();
                UpdatePropertyDefinitions();

                switch (key3)
                {
                    case "SaveAs":
                    case "Export":
                        tvControl1.SearchPattern = "*.grb";
                        break;
                    case "Import":                        
                        switch (key2)
                        {
                            case "Acis":
                                tvControl1.SearchPattern = "*.sat";
                                break;
                            case "Iges":
                                tvControl1.SearchPattern = "*.igs";
                                break;
                            case "Jt":
                                tvControl1.SearchPattern = "*.jt";
                                break;
                            case "Step":
                                tvControl1.SearchPattern = "*.stp";
                                break;
                        }
                        break;
                }

                tvControl1.InitLayout();
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
        /// Update property definitions.
        /// </summary>
        private void UpdatePropertyDefinitions()
        {
            propertyGrid.PropertyDefinitions.Clear();

            if (key3 == "Import")
            {
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition
                {
                    TargetProperties = GetTargetProperties(importMode),
                    IsBrowsable = false
                });
            }
            else
            {
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition
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
                        "Sewing",
                        "CheckImportGeomerty",
                        "UpdateProductStructure",
                        "AddBodyRecordsInProductStructure"
                    },
                    IsBrowsable = false
                });
            }
        }

        /// <summary>
        /// Get target properties for browsable definition.
        /// </summary>
        /// <param name="mode">Import type.</param>
        /// <returns>Target properties.</returns>
        private string[] GetTargetProperties(int mode)
        {
            string[] properties;

            if (mode != 0)
            {
                properties = new string[]
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
                };
            }
            else
            {
                properties = new string[]
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
                };
            }

            return properties;
        }

        /// <summary>
        /// Extension method to query on save changes.
        /// </summary>
        /// <returns></returns>
        private bool QueryOnSaveChanges()
        {
            if (self.HasChanged)
            {
                MessageBoxResult result = MessageBox.Show(
                    messages[1],
                    "T-FLEX Package Manager",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        self.SetConfigurations();
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

            if (self.Configurations.Count() == 0)
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

            if (self.Configurations[key1].IsChanged &&
                self.Configurations[key1].IsInvalid == false)
            {
                menuItem1_4.IsEnabled = true;
                button1_4.IsEnabled = true;
            }
            else
            {
                menuItem1_4.IsEnabled = false;
                button1_4.IsEnabled = false;
            }

            if (self.HasChanged &&
                self.Configurations[key1].IsInvalid == false)
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
            double[] count = { 0.0 };
            var size = Marshal.SizeOf(count[0]) * count.Length;
            IntPtr value = Marshal.AllocHGlobal(size);
            Stopwatch watch = new Stopwatch();
            LogFile logFile = new LogFile(options);
            TranslatorType t_mode = TranslatorType.Document;
            ProcessingMode p_mode = ProcessingMode.SaveAs;

            switch (key2)
            {
                case "Acad"  : t_mode = TranslatorType.Acad;   break;
                case "Acis"  : t_mode = TranslatorType.Acis;   break;
                case "Bitmap": t_mode = TranslatorType.Bitmap; break;
                case "Iges"  : t_mode = TranslatorType.Iges;   break;
                case "Jt"    : t_mode = TranslatorType.Jt;     break;
                case "Pdf"   : t_mode = TranslatorType.Pdf;    break;
                case "Step"  : t_mode = TranslatorType.Step;   break;
            }

            switch (key3)
            {
                case "Export": p_mode = ProcessingMode.Export; break;
                case "Import": p_mode = ProcessingMode.Import; break;
            }

            watch.Start();

            string[] si = tvControl1.SelectedItems.OrderBy(i => i).Cast<string>().ToArray();
            Processing processing = new Processing(self.Configurations[key1], si, t_mode, p_mode, logFile);

            logFile.CreateLogFile(Path.Combine(self.Configurations[key1].TargetDirectory, key2));
            logFile.AppendLine("Started processing");
            logFile.AppendLine(string.Format("Translator mode:\t{0}", key2));
            logFile.AppendLine(string.Format("Processing mode:\t{0}", p_mode));

            foreach (var i in si)
            {
                double increment = 100.0 / tvControl1.SelectedItems.Count;

                if (stoped)
                {
                    NativeMethods.SendMessage(handle, WM_STOPPED_PROCESSING, IntPtr.Zero, IntPtr.Zero);
                    break;
                }

                logFile.AppendLine("\n");
                processing.ProcessingFile(self.Configurations[key1].Translators[key2], i.ToString());

                count[0] += increment;
                Marshal.Copy(count, 0, value, count.Length);
                NativeMethods.SendMessage(handle, WM_INCREMENT_PROGRESS, IntPtr.Zero, value);
            }

            watch.Stop();

            logFile.AppendLine(string.Format("\r\nTotal processing time:\t{0} ms", watch.ElapsedMilliseconds));
            logFile.SetContentsToLog();
            logFile.OpenLogFile();

            count[0] = 100;
            Marshal.Copy(count, 0, value, count.Length);
            NativeMethods.SendMessage(handle, WM_INCREMENT_PROGRESS, IntPtr.Zero, value);
            NativeMethods.SendMessage(handle, WM_STOPPED_PROCESSING, IntPtr.Zero, IntPtr.Zero);
        }
        #endregion
    }
}