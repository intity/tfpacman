using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.Editors
{
    public partial class CustomTextBoxEditor : TextBox, ITypeEditor
    {
        UndoRedo<string> value;

        public CustomTextBoxEditor()
        {
            Padding = new Thickness(3);
            BorderBrush = Brushes.White;
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
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName && Text != value.Value)
                    {
                        Text = value.Value;
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 &&
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName && Text != value.Value)
                    {
                        Text = value.Value;
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(CustomTextBoxEditor),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
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

            Text = Value;
            LostKeyboardFocus += TextBox_LostKeyboardFocus;
            value = new UndoRedo<string>(Value);

            return this;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            Value = Text;

            if (value.Value != Text)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = Text;
                    UndoRedoManager.Commit();
                }
            }

            //Debug.WriteLine(string.Format("PropertyItem: [name: {0}, value: {1}, can undo: {2}, can redo: {3}]",
            //    p.PropertyName, p.Value, UndoRedoManager.CanUndo, UndoRedoManager.CanRedo));
        }
    }
}
