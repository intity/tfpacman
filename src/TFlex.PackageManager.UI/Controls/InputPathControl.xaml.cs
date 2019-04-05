using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputPathControl.xaml
    /// </summary>
    public partial class InputPathControl : UserControl, ITypeEditor
    {
        private string displayName;
        private string description;
        private readonly List<ImageSource> imageSources;

        public InputPathControl()
        {
            InitializeComponent();

            imageSources = new List<ImageSource>
            {
                new BitmapImage(new Uri(Resource.BASE_URI + "open_configurations.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_initial_catalog.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_target_directory.ico"))
            };
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(InputPathControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            switch (propertyItem.PropertyName)
            {
                case "UserDirectory":
                    (buton1.Content as Image).Source = imageSources[0];
                    break;
                case "InitialCatalog":
                    (buton1.Content as Image).Source = imageSources[1];
                    break;
                case "TargetDirectory":
                    (buton1.Content as Image).Source = imageSources[2];
                    break;
            }

            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };

            BindingOperations.SetBinding(this, ValueProperty, binding);
            displayName = propertyItem.DisplayName;
            description = propertyItem.Description;

            return this;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            CustomFolderBrowserDialog fbd = new CustomFolderBrowserDialog
            {
                Title           = displayName,
                Description     = description,
                Owner           = Window.GetWindow(Parent),
                RootFolder      = Environment.SpecialFolder.MyComputer,
                SelectedPath    = Value?.ToString(),
                StartupLocation = WindowStartupLocation.CenterOwner
            };

            if (fbd.ShowDialog())
            {
                Value = fbd.SelectedPath;
            }
        }
    }
}