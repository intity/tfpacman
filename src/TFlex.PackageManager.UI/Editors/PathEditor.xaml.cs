using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TFlex.PackageManager.UI.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for PathEditor.xaml
    /// </summary>
    public partial class PathEditor : UserControl, ITypeEditor
    {
        #region private fields
        UndoRedo<string> value;
        #endregion

        public PathEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoManager_CommandDone;
        }

        private void UndoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
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
            DependencyProperty.Register("Value", typeof(string), typeof(PathEditor),
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
                    (buton1.Content as Image).Source = Resources["icon_1"] as ImageSource;
                    break;
                case "InitialCatalog":
                    (buton1.Content as Image).Source = Resources["icon_2"] as ImageSource;
                    break;
                case "TargetDirectory":
                    (buton1.Content as Image).Source = Resources["icon_3"] as ImageSource;
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

            if (fbd.ShowDialog() && Value != fbd.SelectedPath)
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