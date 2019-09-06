using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputPathControl.xaml
    /// </summary>
    public partial class InputPathControl : UserControl, ITypeEditor
    {
        #region private fields
        readonly List<ImageSource> imageSources;
        UndoRedo<string> value;
        #endregion

        public InputPathControl()
        {
            InitializeComponent();

            imageSources = new List<ImageSource>
            {
                new BitmapImage(new Uri(Resource.BASE_URI + "open_configurations.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_initial_catalog.ico")),
                new BitmapImage(new Uri(Resource.BASE_URI + "open_target_directory.ico"))
            };

            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                    if (UndoRedoManager.RedoCommands.Count() > 0 &&
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName && Value != value.Value)
                    {
                        Value = value.Value;
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 &&
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName && Value != value.Value)
                    {
                        Value = value.Value;
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(InputPathControl),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            value = new UndoRedo<string>(Value);

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

            return this;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            
            CustomFolderBrowserDialog fbd = new CustomFolderBrowserDialog
            {
                Title           = p.DisplayName,
                Description     = p.Description,
                Owner           = Window.GetWindow(Parent),
                RootFolder      = Environment.SpecialFolder.MyComputer,
                SelectedPath    = Value?.ToString(),
                StartupLocation = WindowStartupLocation.CenterOwner
            };

            if (fbd.ShowDialog())
            {
                Value = fbd.SelectedPath;
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = Value;
                    UndoRedoManager.Commit();
                }
            }
        }
    }
}