using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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
        private PackageCollection self;
        private List<string> changedConfigurations;

        private TreeListView treeListView1;
        private TreeListView treeListView2;

        private GridViewColumn column1_1;
        private GridViewColumn column1_2;
        private GridViewColumn column1_3;
        private GridViewColumn column1_9;
        private GridViewColumn column2_1;

        private GridViewColumnHeader header1_1;
        private GridViewColumnHeader header1_2;
        private GridViewColumnHeader header1_3;
        private GridViewColumnHeader header1_9;
        private GridViewColumnHeader header2_1;

        private Common.Options options;
        private AboutUs aboutUs;

        private string[] messages = new string[5];
        private string[] controls = new string[12];
        private string[] tooltips = new string[10];

        private string key1, key2;
        private Brush background;
        private bool isValid = true;

        private Thread thread;
        private bool stoped;

        const int GWLP_WNDPROC = (-4);
        const uint WM_STOPPED_PROCESSING = 0x0400;
        const uint WM_INCREMENT_PROGRESS = 0x0401;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Title = Resource.AppName;

            #region initialize controls
            header1_1 = new GridViewColumnHeader
            {
                Content = Resource.GetString(Resource.MAIN_WINDOW, "header1_1", 0),
                Padding = new Thickness(10, 2, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Left
            };

            header1_2 = new GridViewColumnHeader
            {
                Content = "DWG",
                Width = 50
            };

            header1_3 = new GridViewColumnHeader
            {
                Content = "BMP",
                Width = 50
            };

            header1_9 = new GridViewColumnHeader
            {
                Content = "PDF",
                Width = 50
            };

            header2_1 = new GridViewColumnHeader
            {
                Content = Resource.GetString(Resource.MAIN_WINDOW, "header2_1", 0),
                Padding = new Thickness(10, 2, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Left
            };

            column1_1 = new GridViewColumn
            {
                Header = header1_1,
                CellTemplate = tvControl1.Resources["CellTemplateLabel"] as DataTemplate
            };

            column1_2 = new GridViewColumn
            {
                Header = header1_2,
                CellTemplate = tvControl1.Resources["CellTemplateCheckBox"] as DataTemplate
            };

            column1_3 = new GridViewColumn
            {
                Header = header1_3,
                CellTemplate = tvControl1.Resources["CellTemplateCheckBox"] as DataTemplate
            };

            column1_9 = new GridViewColumn
            {
                Header = header1_9,
                CellTemplate = tvControl1.Resources["CellTemplateCheckBox"] as DataTemplate
            };

            column2_1 = new GridViewColumn
            {
                Header = header2_1,
                CellTemplate = tvControl1.Resources["CellTemplateLabel"] as DataTemplate
            };

            treeListView1 = new TreeListView { CheckboxesVisible = true };
            treeListView1.Columns.Add(column1_1);
            treeListView1.Columns.CollectionChanged += Columns_CollectionChanged;

            tvControl1.Content = treeListView1;
            tvControl1.SearchPattern = "*.grb";
            tvControl1.SizeChanged += TvControl1_SizeChanged;
            tvControl1.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            treeListView2 = new TreeListView();
            treeListView2.Columns.Add(column2_1);

            tvControl2.Content = treeListView2;
            tvControl2.SearchPattern = "*.dwg|*.dxf|*.dxb|*.bmp|*.jpeg|*.gif|*.tiff|*.png|*.pdf";
            tvControl2.SizeChanged += TvControl2_SizeChanged;

            options = new Common.Options();
            self = new PackageCollection(options);
            changedConfigurations = new List<string>();
            #endregion

            #region initialize resources
            messages[0] = Resource.GetString(Resource.MAIN_WINDOW, "message1", 0);
            messages[1] = Resource.GetString(Resource.MAIN_WINDOW, "message2", 0);
            messages[2] = Resource.GetString(Resource.MAIN_WINDOW, "message3", 0);
            messages[3] = Resource.GetString(Resource.MAIN_WINDOW, "message4", 0);
            messages[4] = Resource.GetString(Resource.MAIN_WINDOW, "message5", 0);

            label1.Content = Resource.GetString(Resource.MAIN_WINDOW, "label1", 0);
            label2.Content = Resource.GetString(Resource.MAIN_WINDOW, "label2", 0);
            label3.Content = string.Format(
                             Resource.GetString(Resource.MAIN_WINDOW, "label3", 0), 0);
            label4.Content = string.Format(
                             Resource.GetString(Resource.MAIN_WINDOW, "label4", 0), 0);

            controls[00] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 0);
            controls[01] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 0);
            controls[02] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 0);
            controls[03] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 0);
            controls[04] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 0);
            controls[05] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 0);
            controls[06] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 0);
            controls[07] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_8", 0);
            controls[08] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 0);
            controls[09] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 0);
            controls[10] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 0);
            controls[11] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem4_1", 0);

            tooltips[00] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 1);
            tooltips[01] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 1);
            tooltips[02] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 1);
            tooltips[03] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 1);
            tooltips[04] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 1);
            tooltips[05] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 1);
            tooltips[06] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 1);
            tooltips[07] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 1);
            tooltips[08] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 1);
            tooltips[09] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 1);

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
            menuItem3_1.Header = controls[10];
            menuItem4_1.Header = controls[11];

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
            #endregion
        }

        #region window proc
        private delegate IntPtr WinProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private static IntPtr handle = IntPtr.Zero;
        private static IntPtr oldWndProc = IntPtr.Zero;
        private WinProcDelegate newWndProc;

        private IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case WM_STOPPED_PROCESSING:
                    button2_2.IsEnabled = false;
                    menuItem2_2.IsEnabled = false;
                    label4.Content = string.Format(Resource.GetString(Resource.MAIN_WINDOW, "label4", 0), tvControl2.CountFiles);
                    tvControl2.InitLayout();
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

            return WinAPI.CallWindowProc(oldWndProc, hWnd, uMsg, wParam, lParam);
        }
        #endregion

        #region main window
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (handle == IntPtr.Zero)
            {
                handle = new WindowInteropHelper(this).Handle;
                newWndProc = new WinProcDelegate(WindowProc);
                oldWndProc = WinAPI.GetWindowLongPtr(handle, GWLP_WNDPROC);
                WinAPI.SetWindowLongPtr(handle, GWLP_WNDPROC, 
                    Marshal.GetFunctionPointerForDelegate(newWndProc));
            }

            if (comboBox1.Items.Count == 0 && self.Configurations.Count > 0)
            {
                for (int i = 0; i < self.Configurations.Count; i++)
                {
                    self.Configurations.ElementAt(i).Value.TranslatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;
                    comboBox1.Items.Add(self.Configurations.ElementAt(i).Key);
                }

                comboBox1.SelectedIndex = 0;
            }
            else
            {
                UpdateStateToControls();
            }

            menuItem2_2.IsEnabled = false;
            button2_2.IsEnabled = false;
            label4.Content = string.Format(
                Resource.GetString(Resource.MAIN_WINDOW, "label4", 0), 
                tvControl2.CountFiles);

            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            QueryOnSaveChanges();
        }

        private void TranslatorTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Acad":
                    if ((sender as TranslatorTypes).Acad)
                    {
                        comboBox2.Items.Add(e.PropertyName);
                        treeListView1.Columns.Add(column1_2);
                        header1_2.Content = "DWG";
                    }
                    else
                    {
                        comboBox2.Items.Remove(e.PropertyName);
                        treeListView1.Columns.Remove(column1_2);
                    }
                    break;
                case "Bitmap":
                    if ((sender as TranslatorTypes).Bitmap)
                    {
                        comboBox2.Items.Add(e.PropertyName);
                        treeListView1.Columns.Add(column1_3);
                        header1_3.Content = "BMP";
                    }
                    else
                    {
                        comboBox2.Items.Remove(e.PropertyName);
                        treeListView1.Columns.Remove(column1_3);
                    }
                    break;
                case "Pdf":
                    if ((sender as TranslatorTypes).Pdf)
                    {
                        comboBox2.Items.Add(e.PropertyName);
                        treeListView1.Columns.Add(column1_9);
                    }
                    else
                    {
                        comboBox2.Items.Remove(e.PropertyName);
                        treeListView1.Columns.Remove(column1_9);
                    }
                    break;
            }
        } // update controls
        #endregion

        #region tree views
        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            double a_width, c_width;
            
            c_width = treeListView1.Columns.Count > 1 ? (treeListView1.Columns.Count - 1) * 50 : 0;
            a_width = tvControl1.ActualWidth - (c_width + 18);
            treeListView1.Columns[0].Width = a_width;
            header1_1.Width = a_width;
            tvControl1.Content = treeListView1;
        } // resize column (0)

        private void TvControl1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double a_width, c_width = treeListView1.Columns.Count > 1 
                                   ? (treeListView1.Columns.Count - 1) * 50 : 0;

            if (sender is TreeListViewControl tvctl && tvctl.ActualWidth >= (c_width + 18))
            {
                a_width = tvctl.ActualWidth - (c_width + 18);
                column1_1.Width = a_width;
                header1_1.Width = a_width;
            }
        }

        private void TvControl2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width;

            if (sender is TreeListViewControl tvctl && tvctl.ActualWidth >= 18)
            {
                width = tvctl.ActualWidth - 18;
                column2_1.Width = width;
                header2_1.Width = width;
            }
        }
        #endregion

        #region property grid
        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (e.OriginalSource is PropertyItem item)
            {
                switch (item.PropertyName)
                {
                    case "Extension":
                        switch (key2)
                        {
                            case "Acad":
                                header1_2.Content = ((Package_1)self.Configurations[key1].Translators[key2]).OutputExtension;
                                break;
                            case "Bitmap":
                                header1_3.Content = ((Package_3)self.Configurations[key1].Translators[key2]).OutputExtension;
                                break;
                        }
                        break;
                    case "SubDirectoryName":
                        ValidatePath(item, 2, 0);
                        break;
                    case "FileNameSuffix":
                        ValidatePath(item, 3, 1);
                        break;
                    case "TemplateFileName":
                        ValidatePath(item, 4, 1);
                        break;
                }

                if (self.Configurations[key1].IsChanged && 
                    changedConfigurations.Contains(key1) == false && isValid)
                    changedConfigurations.Add(key1);
                else if (!self.Configurations[key1].IsChanged)
                    changedConfigurations.Remove(key1);
                
                UpdateStateToControls();

                Debug.WriteLine(string.Format("PropertyName: {0}, Value: {1}, IsChanged: {2}, total changes: {3}", 
                    item.PropertyName, 
                    item.Value, 
                    self.Configurations[key1].IsChanged, 
                    changedConfigurations.Count));
            }
        }

        /// <summary>
        /// Extension method to validate path name.
        /// </summary>
        /// <param name="item">Property item.</param>
        /// <param name="index">Message index.</param>
        /// <param name="flag">(0) - folder name, (1) - file name</param>
        private void ValidatePath(PropertyItem item, int index, int flag)
        {
            string path = item.Value.ToString();
            char[] pattern = flag > 0 ? Path.GetInvalidFileNameChars() : Path.GetInvalidPathChars();

            if (item.PropertyName == "TemplateFileName")
            {
                foreach (Match i in Regex.Matches(path, @"\{(.*?)\}"))
                {
                    path = path.Replace(i.Value, "");
                }
            }

            if (path.Length > 0 && path.IndexOfAny(pattern) >= 0)
            {
                background = item.Background;
                item.Background = Brushes.Red;
                item.ToolTip = string.Format(messages[index], pattern.ToString(""));
                isValid = false;
            }
            else
            {
                item.Background = background;
                item.ToolTip = null;
                isValid = true;
            }
        }
        #endregion

        #region menubar & toolbar events
        private void Event1_1_Click(object sender, RoutedEventArgs e)
        {
            CommonSaveFileDialog sfd = new CommonSaveFileDialog
            {
                Title = controls[0],
                InitialDirectory = self.TargetDirectory,
                DefaultFileName = "configuration_" + self.Configurations.Count.ToString(),
                DefaultExtension = "config"
            };

            if (sfd.ShowDialog() == CommonFileDialogResult.Cancel)
                return;

            string newKey = Path.GetFileNameWithoutExtension(sfd.FileName);
            var directory = sfd.FileName.Replace("\\" + newKey + ".config", "");

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

            header.TranslatorTypes.PropertyChanged += TranslatorTypes_PropertyChanged;
            self.Configurations.Add(newKey, header);
            self.Configurations[newKey].ConfigurationTask(1);

            comboBox1.Items.Add(newKey);
            comboBox1.SelectedItem = newKey;

            sfd.Dispose();
        } // New configuration

        private void Event1_2_Click(object sender, RoutedEventArgs e)
        {

        } // Open configuration

        private void Event1_3_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                Title = tooltips[2],
                Multiselect = false,
                IsFolderPicker = true,
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

            if (self.Configurations[key1].IsLoaded)
            {
                changedConfigurations.Remove(key1);
                UpdateStateToControls();
            }
        } // Save configuration

        private void Event1_5_Click(object sender, RoutedEventArgs e)
        {
            changedConfigurations.Clear();
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
                self.Configurations[key1].ConfigurationTask(2);
                self.Configurations.Remove(key1);
                changedConfigurations.Remove(key1);
                comboBox1.Items.Remove(key1);
                comboBox1.SelectedIndex = (comboBox1.Items.Count - 1);
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

        private void Event3_1_Click(object sender, RoutedEventArgs e)
        {
            PropertiesUI optionsUI = new PropertiesUI(options)
            {
                Title = controls[10],
                Owner = this
            };
            optionsUI.ShowDialog();
        } // Options

        private void Event4_1_Click(object sender, RoutedEventArgs e)
        {
            aboutUs = new AboutUs
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

                string[] items1 = comboBox2.Items.OfType<string>().ToArray();
                string[] items2 = self.Configurations[key1].Translators.Keys.ToArray();

                if (items2.Length > 0 && !Enumerable.SequenceEqual(items1, items2))
                {
                    comboBox2.Items.Clear();

                    for (int j = treeListView1.Columns.Count - 1; j > 0; j--)
                        treeListView1.Columns.RemoveAt(j);

                    foreach (var i in self.Configurations[key1].Translators)
                    {
                        comboBox2.Items.Add(i.Key);

                        switch (i.Key)
                        {
                            case "Acad":
                                header1_2.Content = ((Package_1)self.Configurations[key1].Translators[i.Key]).OutputExtension;
                                treeListView1.Columns.Add(column1_2);
                                break;
                            case "Bitmap":
                                header1_3.Content = ((Package_3)self.Configurations[key1].Translators[i.Key]).OutputExtension;
                                treeListView1.Columns.Add(column1_3);
                                break;
                            case "Pdf":
                                treeListView1.Columns.Add(column1_9);
                                break;
                        }
                    }

                    comboBox2.SelectedIndex = 0;
                }

                tvControl1.TargetDirectory = self.Configurations[key1].InitialCatalog;
                tvControl2.TargetDirectory = self.Configurations[key1].TargetDirectory;

                UpdateStateToControls();
            }
            else
                propertyGrid.SelectedObject = null;

            Debug.WriteLine(string.Format("ComboBox1_SelectionChanged: [{0}, {1}]", 
                comboBox1.SelectedIndex, 
                comboBox1.SelectedItem));
        } // configuration list

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                key2 = comboBox2.SelectedValue.ToString();
                propertyGrid.SelectedObject = self.Configurations[key1].Translators[key2];
            }
            else
            {
                propertyGrid.SelectedObject = null;
            }

            Debug.WriteLine(string.Format("ComboBox2_SelectionChanged: [{0}, {1}]", 
                comboBox2.SelectedIndex, 
                comboBox2.SelectedItem));
        } // translator collection
        #endregion

        #region statusbar
        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            label3.Content = string.Format(
                Resource.GetString(Resource.MAIN_WINDOW, "label3", 0), 
                tvControl1.SelectedItems.Count());

            UpdateStateToControls();
        }
        #endregion

        #region helper methods
        private bool QueryOnSaveChanges()
        {
            if (changedConfigurations.Count > 0)
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
                        changedConfigurations.Clear();
                        UpdateStateToControls();
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        private void UpdateStateToControls()
        {
            //Debug.WriteLine(string.Format("UpdateStateToControls: {0}", comboBox1.SelectedIndex));

            if (self.Configurations.Count() == 0)
            {
                menuItem1_4.IsEnabled = false; // save
                menuItem1_5.IsEnabled = false; // save all
                menuItem1_6.IsEnabled = false; // delete
                menuItem1_7.IsEnabled = false; // properties
                menuItem2_1.IsEnabled = false; // start

                button1_4.IsEnabled = false;
                button1_5.IsEnabled = false;
                button1_6.IsEnabled = false;
                button1_7.IsEnabled = false;
                button2_1.IsEnabled = false;

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
            }

            if (self.Configurations[key1].IsCreated ||
                self.Configurations[key1].IsChanged)
            {
                menuItem1_4.IsEnabled = true;
                button1_4.IsEnabled = true;
            }
            else
            {
                menuItem1_4.IsEnabled = false;
                button1_4.IsEnabled = false;
            }

            if (self.Configurations[key1].InitialCatalog.Length > 0 &&
                self.Configurations[key1].InitialCatalog != tvControl1.TargetDirectory)
                tvControl1.TargetDirectory = self.Configurations[key1].InitialCatalog;

            if (self.Configurations[key1].TargetDirectory.Length > 0 &&
                self.Configurations[key1].TargetDirectory != tvControl2.TargetDirectory)
                tvControl2.TargetDirectory = self.Configurations[key1].TargetDirectory;

            if (self.Configurations[key1].InitialCatalog.Length > 0 &&
                self.Configurations[key1].TargetDirectory.Length > 0 && 
                tvControl1.SelectedItems.Count > 0 && isValid)
            {
                menuItem2_1.IsEnabled = true;
                button2_1.IsEnabled = true;
            }
            else
            {
                menuItem2_1.IsEnabled = false;
                button2_1.IsEnabled = false;
            }

            if (changedConfigurations.Count > 0)
            {
                menuItem1_5.IsEnabled = true;
                button1_5.IsEnabled = true;
            }
            else
            {
                menuItem1_5.IsEnabled = false;
                button1_5.IsEnabled = false;
            }
        }

        private void StartProcessing()
        {
            double[] count = { 0.0 };
            double increment = 0.0;
            var size = Marshal.SizeOf(count[0]) * count.Length;
            IntPtr value = Marshal.AllocHGlobal(size);
            Stopwatch watch = new Stopwatch();

            options.CreateLogFile(self.Configurations[key1].TargetDirectory);
            options.AppendLine("Started processing");

            watch.Start();

            foreach (var i in tvControl1.SelectedItems)
            {
                increment = 100.0 / (tvControl1.SelectedItems.Count * i.Value.Length);

                if (stoped)
                {
                    WinAPI.SendMessage(handle, WM_STOPPED_PROCESSING, IntPtr.Zero, IntPtr.Zero);
                    break;
                }

                string path = i.Key;

                for (int j = 0; j < i.Value.Length; j++)
                {
                    if (i.Value[j].Value == false)
                    {
                        count[0] += increment;
                        continue;
                    }

                    switch (self.Configurations[key1].Translators.ElementAt(j).Key)
                    {
                        case "Default":
                            options.AppendLine("\r\nTranslator:\tDefault");
                            (self.Configurations[key1].Translators.ElementAt(j).Value as Package_0)
                                .ProcessingFile(path, options);
                            break;
                        case "Acad":
                            options.AppendLine("\r\nTranslator:\tAcad");
                            (self.Configurations[key1].Translators.ElementAt(j).Value as Package_1)
                                .ProcessingFile(path, options);
                            break;
                        case "Bitmap":
                            options.AppendLine("\r\nTranslator:\tBitmap");
                            (self.Configurations[key1].Translators.ElementAt(j).Value as Package_3)
                                .ProcessingFile(path, options);
                            break;
                        case "Pdf":
                            options.AppendLine("\r\nTranslator:\tPdf");
                            (self.Configurations[key1].Translators.ElementAt(j).Value as Package_9)
                                .ProcessingFile(path, options);
                            break;
                    }

                    count[0] += increment;
                    Marshal.Copy(count, 0, value, count.Length);
                    WinAPI.SendMessage(handle, WM_INCREMENT_PROGRESS, IntPtr.Zero, value);
                }
            }

            watch.Stop();

            options.AppendLine(string.Format("\r\nProcessing time:{0} ms", watch.ElapsedMilliseconds));
            options.SetContentsToLog();
            options.OpenLogFile();

            count[0] = 100;
            Marshal.Copy(count, 0, value, count.Length);
            WinAPI.SendMessage(handle, WM_INCREMENT_PROGRESS, IntPtr.Zero, value);
            WinAPI.SendMessage(handle, WM_STOPPED_PROCESSING, IntPtr.Zero, IntPtr.Zero);
        }
        #endregion
    }
}