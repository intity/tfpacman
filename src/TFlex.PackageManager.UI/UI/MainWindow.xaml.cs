using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using TFlex.PackageManager.Controls;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Export;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

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
        private CommonOpenFileDialog ofd;

        private string[] messages = new string[5];
        private string[] tooltips = new string[10];

        private string key1, key2;
        private Brush background;
        private bool isValid = true;
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

            self = new PackageCollection();
            options = new Common.Options();
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

            tooltips[0] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 1);
            tooltips[1] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 1);
            tooltips[2] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 1);
            tooltips[3] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 1);
            tooltips[4] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 1);
            tooltips[5] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 1);
            tooltips[6] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 1);
            tooltips[7] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 1);
            tooltips[8] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 1);
            tooltips[9] = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 1);

            menuItem1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1", 0);
            menuItem2.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2", 0);
            menuItem3.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3", 0);

            menuItem1_1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_1", 0);
            menuItem1_2.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_2", 0);
            menuItem1_3.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_3", 0);
            menuItem1_4.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_4", 0);
            menuItem1_5.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_5", 0);
            menuItem1_6.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_6", 0);
            menuItem1_7.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_7", 0);
            menuItem1_8.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem1_8", 0);
            menuItem2_1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_1", 0);
            menuItem2_2.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem2_2", 0);
            menuItem3_1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem3_1", 0);
            menuItem4_1.Header = Resource.GetString(Resource.MAIN_WINDOW, "menuItem4_1", 0);

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

        #region main window
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (comboBox1.Items.Count == 0 && self.Configurations.Count > 0)
            {
                foreach (var i in self.Configurations.Keys)
                {
                    comboBox1.Items.Add(i);
                }

                comboBox1.SelectedIndex = 0;
            }
            else
            {
                UpdateStateToControls();
            }

            menuItem2_2.IsEnabled = false;
            button2_2.IsEnabled = false;
            
            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            QueryOnSaveChanges();
        }
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
        }

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
                                header1_2.Content = ((ExportToPackage1)self.Configurations[key1].Translators[key2]).OutputExtension;
                                break;
                            case "Bitmap":
                                header1_3.Content = ((ExportToPackage3)self.Configurations[key1].Translators[key2]).OutputExtension;
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
            string newKey = null;
            HeaderUI headerUI = new HeaderUI(null, self) { Owner = this };

            if (headerUI.ShowDialog() == false)
                return;

            newKey = self.Configurations.Last().Key;
            changedConfigurations.Add(newKey);
            comboBox1.Items.Add(newKey);
            comboBox1.SelectedItem = newKey;

        } // New configuration

        private void Event1_2_Click(object sender, RoutedEventArgs e)
        {

        } // Open configuration

        private void Event1_3_Click(object sender, RoutedEventArgs e)
        {
            ofd = new CommonOpenFileDialog
            {
                Title = tooltips[2],
                Multiselect = false,
                IsFolderPicker = true,
                InitialDirectory = self.TargetDirectory
            };

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok &&
                ofd.FileName != self.TargetDirectory)
            {
                QueryOnSaveChanges();

                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Items.Clear();
                    self.Configurations.Clear();
                }

                self.TargetDirectory = ofd.FileName;
                self.GetConfigurations();

                if (self.Configurations.Count > 0)
                {
                    foreach (var i in self.Configurations.Keys)
                    {
                        comboBox1.Items.Add(i);
                    }

                    comboBox1.SelectedIndex = 0;
                }

                //Debug.WriteLine(string.Format("TargetDirectory: {0}", self.TargetDirectory));
            }
        } // Open target directory

        private void Event1_4_Click(object sender, RoutedEventArgs e)
        {
            self.Configurations[key1].ConfigurationTask(1);

            if (self.Configurations[key1].IsLoaded)
            {
                self.RemoveOldConfiguration(key1);
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

                if (true)
                {
                    self.Configurations.Remove(key1);
                    changedConfigurations.Remove(key1);
                    comboBox1.Items.Remove(key1);
                    comboBox1.SelectedIndex = (comboBox1.Items.Count - 1);
                }
            }
        } // Delete configuration

        private void Event1_7_Click(object sender, RoutedEventArgs e)
        {
            string newKey = null;
            HeaderUI headerUI = new HeaderUI(key1, self) { Owner = this };

            if (headerUI.ShowDialog() == false)
                return;

            if (key1 != (newKey = self.Configurations[key1].ConfigurationName))
            {
                comboBox1.Items[comboBox1.SelectedIndex] = newKey;
                self.RenameConfiguration(key1, newKey);
                comboBox1.SelectedItem = newKey;
            }
            else if (self.Configurations[key1].Translators.Count > 0)
            {
                UpdateControls();
            }
            else if (comboBox2.Items.Count > 0)
            {
                for (int i = treeListView1.Columns.Count - 1; i > 0; i--)
                    treeListView1.Columns.RemoveAt(i);

                comboBox2.Items.Clear();
            }

            changedConfigurations.Add(key1);
            UpdateStateToControls();
        } // Properties (edit)

        private void Event1_8_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Application exit

        private void Event2_1_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in tvControl1.SelectedItems)
            {
                string path = i.Key;

                for (int j = 0; j < i.Value.Length; j++)
                {
                    if (i.Value[j].Value == false || j == 0)
                        continue;

                    switch (self.Configurations[key1].Translators.ElementAt(j - 1).Key)
                    {
                        case "Acad":
                            (self.Configurations[key1].Translators.ElementAt(j - 1).Value as ExportToPackage1).Export(path);
                            break;
                        case "Bitmap":
                            (self.Configurations[key1].Translators.ElementAt(j - 1).Value as ExportToPackage3).Export(path);
                            break;
                        case "Pdf":
                            (self.Configurations[key1].Translators.ElementAt(j - 1).Value as ExportToPackage9).Export(path);
                            break;
                    }
                }
            }

            tvControl2.InitLayout();
        } // Start processing

        private void Event2_2_Click(object sender, RoutedEventArgs e)
        {

        } // Stop processing

        private void Event3_1_Click(object sender, RoutedEventArgs e)
        {
            OptionsUI optionsUI = new OptionsUI(options) { Owner = this };
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

                if (self.Configurations[key1].Translators.Count() == 0)
                {
                    propertyGrid.SelectedObject = null;
                }

                UpdateControls();
                UpdateStateToControls();
            }
            else
                propertyGrid.SelectedObject = null;

            //Debug.WriteLine(string.Format("ComboBox1_SelectionChanged: [{0}, {1}]", comboBox1.SelectedIndex, comboBox1.SelectedItem));
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

            //Debug.WriteLine(string.Format("ComboBox2_SelectionChanged: [{0}, {1}]", comboBox2.SelectedIndex, comboBox2.SelectedItem));
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
        private void QueryOnSaveChanges()
        {
            if (changedConfigurations.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    messages[1],
                    "T-FLEX Package Manager",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    self.SetConfigurations();
                    changedConfigurations.Clear();
                    UpdateStateToControls();
                }
            }
        }

        private void UpdateControls()
        {
            Debug.WriteLine(string.Format("UpdateControls: {0}", self.Configurations[key1].InitialCatalog));

            string[] items1 = comboBox2.Items.OfType<string>().ToArray();
            string[] items2 = self.Configurations[key1].Translators.Keys.ToArray();

            if (items1.Length > 0 && !Enumerable.SequenceEqual(items1, items2))
            {
                comboBox2.Items.Clear();

                for (int i = treeListView1.Columns.Count - 1; i > 0; i--)
                    treeListView1.Columns.RemoveAt(i);
            }

            foreach (var i in self.Configurations[key1].Translators.Keys)
            {
                switch (i)
                {
                    case "Default":
                        if (comboBox2.Items.Contains(i) == false)
                            comboBox2.Items.Add(i);
                        break;
                    case "Acad":
                        header1_2.Content = ((ExportToPackage1)self.Configurations[key1].Translators[i]).OutputExtension;

                        if (comboBox2.Items.Contains(i) == false)
                            comboBox2.Items.Add(i);

                        if (treeListView1.Columns.Contains(column1_2) == false)
                            treeListView1.Columns.Add(column1_2);
                        break;
                    case "Bitmap":
                        header1_3.Content = ((ExportToPackage3)self.Configurations[key1].Translators[i]).OutputExtension;

                        if (comboBox2.Items.Contains(i) == false)
                            comboBox2.Items.Add(i);

                        if (treeListView1.Columns.Contains(column1_3) == false)
                            treeListView1.Columns.Add(column1_3);
                        break;
                    case "Pdf":
                        header1_9.Content = ((ExportToPackage9)self.Configurations[key1].Translators[i]).OutputExtension;

                        if (comboBox2.Items.Contains(i) == false)
                            comboBox2.Items.Add(i);

                        if (treeListView1.Columns.Contains(column1_9) == false)
                            treeListView1.Columns.Add(column1_9);
                        break;
                }
            }

            tvControl1.TargetDirectory = self.Configurations[key1].InitialCatalog;
            tvControl2.TargetDirectory = self.Configurations[key1].TargetDirectory;

            if (comboBox2.Items.Count > 0)
                comboBox2.SelectedIndex = 0;
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
        #endregion
    }
}
