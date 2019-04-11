using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.ComponentModel;

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

        private string key1, key2;

        private Thread thread;
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
            tvControl2.SearchPattern = "*.dwg|*.dxf|*.dxb|*.bmp|*.jpeg|*.gif|*.tiff|*.png|*.pdf|*.stp";

            options = new Common.Options();
            self = new ConfigurationCollection { TargetDirectory = options.UserDirectory };
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
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_3", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 0),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem4_1", 0)
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
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_3", 1),
                Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 1)
            };

            menuItem1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1", 0);
            menuItem2.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2", 0);
            menuItem3.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3", 0);

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
            menuItem2_3.Header = controls[10];
            menuItem3_1.Header = controls[11];
            menuItem4_1.Header = controls[12];

            button1_1.ToolTip = tooltips[0];
            button1_2.ToolTip = tooltips[1];
            button1_3.ToolTip = tooltips[2];
            button1_4.ToolTip = tooltips[3];
            button1_5.ToolTip = tooltips[4];
            button1_6.ToolTip = tooltips[5];
            button1_7.ToolTip = tooltips[6];
            button2_1.ToolTip = tooltips[7];
            button2_2.ToolTip = tooltips[8];
            button2_3.ToolTip = tooltips[9];
            button3_1.ToolTip = tooltips[10];

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
                    button2_2.IsEnabled = false;
                    menuItem2_2.IsEnabled = false;
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
            button2_1.IsEnabled   = false;
            button2_2.IsEnabled   = false;

            propertyGrid.PropertyValueChanged += Translator_PropertyValueChanged;
        }

        private void PropertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (comboBox2.SelectedIndex != -1 && comboBox2.SelectedItem.ToString() == "Default")
            {
                if (propertyGrid.PropertyDefinitions.Count == 0)
                {
                    propertyGrid.PropertyDefinitions.Add(new PropertyDefinition
                    {
                        TargetProperties = new[]
                        {
                            "SubDirectoryName",
                            "FileNameSuffix",
                            "TemplateFileName"
                        },
                        IsBrowsable = false
                    });
                }
            }
            else if (propertyGrid.PropertyDefinitions.Count > 0)
            {
                propertyGrid.PropertyDefinitions.Clear();
            }
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
                    tvControl2.TargetDirectory = Path.Combine(self.Configurations[key1].TargetDirectory, key2);
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
                case "Default": t_value = (sender as TranslatorTypes).Default; break;
                case "Acad"   : t_value = (sender as TranslatorTypes).Acad;    break;
                case "Bitmap" : t_value = (sender as TranslatorTypes).Bitmap;  break;
                case "Pdf"    : t_value = (sender as TranslatorTypes).Pdf;     break;
                case "Step"   : t_value = (sender as TranslatorTypes).Step;    break;
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
            UpdateStateToControls();
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
            stoped = false;
            thread = new Thread(StartProcessing);
            thread.Start();
            button2_2.IsEnabled = true;
            menuItem2_2.IsEnabled = true;
            progressBar.Visibility = Visibility.Visible;
        } // Start processing

        private void Event2_2_Click(object sender, RoutedEventArgs e)
        {
            stoped = true;
        } // Stop processing

        private void Event2_3_Click(object sender, RoutedEventArgs e)
        {
            tvControl2.CleanTargetDirectory();
            UpdateStateToControls();
        } // Clear target directory

        private void Event3_1_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI optionsUI = new PropertiesUI(options)
            {
                Title = controls[11],
                Owner = this
            };
            optionsUI.ShowDialog();
        } // Options

        private void Event4_1_Click(object sender, RoutedEventArgs e)
        {
            AboutUs aboutUs = new AboutUs
            {
                Owner = this
            };
            aboutUs.ShowDialog();
        } // About Us

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
                comboBox2.Items.Clear();
                tvControl1.TargetDirectory = string.Empty;
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
                propertyGrid.SelectedObject = self.Configurations[key1].Translators[key2];

                string path = self.Configurations[key1].TargetDirectory;
                if (path.Length > 0)
                {
                    tvControl2.TargetDirectory = Path.Combine(path, key2);
                }
            }
            else
            {
                propertyGrid.SelectedObject = null;
                tvControl2.TargetDirectory  = string.Empty;
            }

            UpdateStateToControls();

            //Debug.WriteLine(string.Format("ComboBox2_SelectionChanged: [{0}, {1}]",
            //    comboBox2.SelectedIndex,
            //    comboBox2.SelectedValue));
        } // translator collection
        #endregion

        #region statusbar
        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (tvControl1.SelectedItems.Count > 0 &&
                tvControl1.TargetDirectory.Length > 0 &&
                tvControl2.TargetDirectory.Length > 0)
            {
                menuItem2_1.IsEnabled = true;
                button2_1.IsEnabled = true;
            }
            else
            {
                menuItem2_1.IsEnabled = false;
                button2_1.IsEnabled = false;
            }

            sb_label3.Content = string.Format(sblabels[2], tvControl1.SelectedItems.Count);
        }
        #endregion

        #region extension methods
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
                menuItem2_1.IsEnabled = false; // start
                menuItem2_3.IsEnabled = false; // clear target directory

                button1_4.IsEnabled = false;
                button1_5.IsEnabled = false;
                button1_6.IsEnabled = false;
                button1_7.IsEnabled = false;
                button2_1.IsEnabled = false;
                button2_3.IsEnabled = false;
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
                    menuItem2_3.IsEnabled = true;
                    button2_3.IsEnabled = true;
                }
                else
                {
                    menuItem2_3.IsEnabled = false;
                    button2_3.IsEnabled = false;
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
            Processing processing = new Processing(self.Configurations[key1], logFile);
            
            logFile.CreateLogFile(Path.Combine(self.Configurations[key1].TargetDirectory, key2));
            logFile.AppendLine("Started processing");
            logFile.AppendLine(string.Format("Translator:\t{0}", key2));

            watch.Start();

            foreach (var i in tvControl1.SelectedItems)
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

            logFile.AppendLine(string.Format("\r\nTotal processing time: {0} ms", watch.ElapsedMilliseconds));
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