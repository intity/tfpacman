using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputSewingControl.xaml
    /// </summary>
    public partial class InputSewingControl : UserControl, ITypeEditor
    {
        UndoRedo<bool> value1;
        UndoRedo<double> value2;

        public InputSewingControl()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            var tr = p.Instance as Translator3D;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                    if (UndoRedoManager.RedoCommands.Count() > 0 &&
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName)
                    {
                        if (checkBox.IsChecked != value1.Value)
                        {
                            checkBox.IsChecked = value1.Value;
                            tr.Sewing = value1.Value;
                        }

                        if (Value != value2.Value)
                            Value = value2.Value;

                        Debug.WriteLine(string.Format("Undo: {0}", checkBox.IsChecked));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 &&
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName)
                    {
                        if (checkBox.IsChecked != value1.Value)
                        {
                            checkBox.IsChecked = value1.Value;
                            tr.Sewing = value1.Value;
                        }

                        if (Value != value2.Value)
                            Value = value2.Value;

                        Debug.WriteLine(string.Format("Redo: {0}", checkBox.IsChecked));
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(InputSewingControl),
                new FrameworkPropertyMetadata(0.0,
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

            checkBox.IsChecked = tr.Sewing;
            checkBox.Checked += CheckBox_IsChecked;
            checkBox.Unchecked += CheckBox_IsChecked;
            doubleUpDown.Value = Value;
            doubleUpDown.ValueChanged += DoubleUpDown_ValueChanged;

            value1 = new UndoRedo<bool>(tr.Sewing);
            value2 = new UndoRedo<double>(Value);

            return this;
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var p = DataContext as PropertyItem;

            if (value2.Value != Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value2.Value = Value;
                    UndoRedoManager.Commit();
                }
            }
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            doubleUpDown.IsEnabled = checkBox.IsChecked.Value;
            var p = DataContext as PropertyItem;
            var tr = p.Instance as Translator3D;

            if (value1.Value != checkBox.IsChecked)
            {
                tr.Sewing = checkBox.IsChecked.Value;

                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value1.Value = tr.Sewing;
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