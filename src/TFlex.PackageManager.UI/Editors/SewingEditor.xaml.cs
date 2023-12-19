using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for SewingEditor.xaml
    /// </summary>
    public partial class SewingEditor : UserControl, ITypeEditor
    {
        private UndoRedo<bool> buffer1;
        private UndoRedo<double?> buffer2;

        public SewingEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (e.Caption != p.PropertyName)
                return;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                case CommandDoneType.Redo:
                    Do(p.Instance as Translator3D);
                    break;
            }
        }

        private void Do(Translator3D tr)
        {
            if (checkBox.IsChecked != buffer1.Value)
            {
                checkBox.IsChecked = buffer1.Value;
                tr.Sewing = buffer1.Value;
            }

            if (doubleUpDown.Value != buffer2.Value)
                doubleUpDown.Value = buffer2.Value;
        }

        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(SewingEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var tr = propertyItem.Instance as Translator3D;

            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            buffer1 = new UndoRedo<bool>(tr.Sewing);
            buffer2 = new UndoRedo<double?>(Value);

            checkBox.IsChecked = tr.Sewing;
            checkBox.Checked += CheckBox_IsChecked;
            checkBox.Unchecked += CheckBox_IsChecked;
            doubleUpDown.ValueChanged += DoubleUpDown_ValueChanged;

            return this;
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (buffer2.Value != Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    buffer2.Value = Value;
                    UndoRedoManager.Commit();
                }
            }
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            doubleUpDown.IsEnabled = checkBox.IsChecked.Value;
            var p = DataContext as PropertyItem;
            var tr = p.Instance as Translator3D;

            if (buffer1.Value != checkBox.IsChecked)
            {
                tr.Sewing = checkBox.IsChecked.Value;

                using (UndoRedoManager.Start(p.PropertyName))
                {
                    buffer1.Value = tr.Sewing;
                    UndoRedoManager.Commit();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            doubleUpDown.IsEnabled = checkBox.IsChecked.Value;
        }
    }
}