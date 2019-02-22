using System.IO;
using System.Windows;
using System.Windows.Media;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for ConfigurationProperties.xaml
    /// </summary>
    public partial class HeaderUI : Window
    {
        #region private fields
        private PackageCollection self;
        private Header header;
        private Brush background;
        private string[] messages = new string[2];
        private string key;
        #endregion

        /// <summary>
        /// The ConfigurationProperties constructor.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key">Configuration key name.</param>
        public HeaderUI(string key, PackageCollection self)
        {
            InitializeComponent();

            this.key = key;
            this.self = self;

            button1.Content = Resource.GetString(Resource.HEADER_UI, "button1", 0);
            button2.Content = Resource.GetString(Resource.HEADER_UI, "button2", 0);
            messages[0]     = Resource.GetString(Resource.HEADER_UI, "message1", 0);
            messages[1]     = Resource.GetString(Resource.HEADER_UI, "message2", 0);

            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (key != null)
            {
                Title = Resource.GetString(Resource.HEADER_UI, "title2", 0);
                button1.IsEnabled = true;
                propertyGrid.SelectedObject = self.Configurations[key];
            }
            else
            {
                Title = Resource.GetString(Resource.HEADER_UI, "title1", 0);
                header = new Header();
                button1.IsEnabled = false;
                propertyGrid.SelectedObject = header;
            }
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (e.OriginalSource is PropertyItem item)
            {
                switch (item.PropertyName)
                {
                    case "ConfigurationName":
                        if (self.Configurations.ContainsKey(item.Value.ToString()) && 
                            (key == null || key != item.Value.ToString()))
                        {
                            background = item.Background;
                            item.Background = Brushes.Red;
                            item.ToolTip = string.Format(messages[0], item.Value);
                            button1.IsEnabled = false;
                        }
                        else if (item.Value.ToString().Length > 0)
                        {
                            char[] pattern = Path.GetInvalidFileNameChars();
                            string name = item.Value.ToString();

                            if (name.IndexOfAny(pattern) >= 0)
                            {
                                background = item.Background;
                                item.Background = Brushes.Red;
                                item.ToolTip = string.Format(messages[1], pattern.ToString(""));
                                button1.IsEnabled = false;
                            }
                            else
                            {
                                item.Background = background;
                                item.ToolTip = null;
                                button1.IsEnabled = true;
                            }
                        }
                        else
                        {
                            button1.IsEnabled = false;
                        }
                        break;
                }
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (key == null)
            {
                self.Configurations.Add(header.ConfigurationName, header);
            }

            DialogResult = true;

            Close();
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
    }
}
