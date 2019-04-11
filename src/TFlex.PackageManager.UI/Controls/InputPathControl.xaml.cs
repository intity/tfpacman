using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public enum InputPathIcon
    {
        UserDirectory,
        InitialCatalog,
        TargetDirectory
    }

    /// <summary>
    /// Interaction logic for InputPathControl.xaml
    /// </summary>
    public partial class InputPathControl : UserControl, ITypeEditor
    {
        private string displayName;
        private string description;
        private InputPathIcon buttonIcon;
        private readonly List<ImageSource> imageSources;
        private readonly string[][] rc_strings;

        public InputPathControl()
        {
            InitializeComponent();

            imageSources = new List<ImageSource>
            {
                new BitmapImage(new Uri(Resource.BASE_URI + "open_configurations.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_initial_catalog.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_target_directory.ico"))
            };

            rc_strings = new string[][]
            {
                new string[] // user directory
                {
                    Resource.GetString(Resource.OPTIONS_UI, "dn1_1", 0),
                    Resource.GetString(Resource.OPTIONS_UI, "dn1_1", 1)
                },
                new string[] // initial catalog
                {
                    Resource.GetString(Resource.HEADER_UI, "dn1_2", 0),
                    Resource.GetString(Resource.HEADER_UI, "dn1_2", 1)
                },
                new string[] // target directory
                {
                    Resource.GetString(Resource.HEADER_UI, "dn1_3", 0),
                    Resource.GetString(Resource.HEADER_UI, "dn1_3", 1)
                }
            };
        }

        public InputPathIcon ButtonIcon
        {
            get { return buttonIcon; }
            set
            {
                if (buttonIcon != value)
                {
                    buttonIcon = value;

                    switch (buttonIcon)
                    {
                        case InputPathIcon.UserDirectory:
                            (buton1.Content as Image).Source = imageSources[0];
                            displayName = rc_strings[0][0];
                            description = rc_strings[0][1];
                            break;
                        case InputPathIcon.InitialCatalog:
                            (buton1.Content as Image).Source = imageSources[1];
                            displayName = rc_strings[1][0];
                            description = rc_strings[1][1];
                            break;
                        case InputPathIcon.TargetDirectory:
                            (buton1.Content as Image).Source = imageSources[2];
                            displayName = rc_strings[2][0];
                            description = rc_strings[2][1];
                            break;
                    }
                }
            }
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