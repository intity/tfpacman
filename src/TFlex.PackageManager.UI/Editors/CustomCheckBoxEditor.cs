using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.Editors
{
    public partial class CustomCheckBoxEditor : CheckBox, ITypeEditor
    {
        UndoRedo<bool> value;

        public CustomCheckBoxEditor()
        {
            Margin = new Thickness(5, 3, 5, 3);
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
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName && IsChecked != value.Value)
                    {
                        IsChecked = value.Value;
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 && 
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName && IsChecked != value.Value)
                    {
                        IsChecked = value.Value;
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]", 
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(CustomCheckBoxEditor),
                new FrameworkPropertyMetadata(false, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool Value
        {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

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

            IsChecked = Value;
            Checked += CheckBox_IsChecked;
            Unchecked += CheckBox_IsChecked;
            value = new UndoRedo<bool>(Value);

            return this;
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            Value = IsChecked.Value;

            if (value.Value != IsChecked)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = IsChecked.Value;
                    UndoRedoManager.Commit();
                }
            }

            //Debug.WriteLine(string.Format("PropertyItem: [name: {0}, value: {1}, can undo: {2}, can redo: {3}]",
            //    p.PropertyName, p.Value, UndoRedoManager.CanUndo, UndoRedoManager.CanRedo));
        }
    }
}