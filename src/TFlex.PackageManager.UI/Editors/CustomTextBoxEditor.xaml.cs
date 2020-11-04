using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for CustomTextBoxEditor.xaml
    /// </summary>
    public partial class CustomTextBoxEditor : UserControl, ITypeEditor
    {
        UndoRedo<string> value;

        public CustomTextBoxEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                    if (e.Caption == p.PropertyName)
                    {
                        Value = value.Value;

                        //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]", 
                        //    p.PropertyName, p.Value));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
                    {
                        Value = value.Value;

                        //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
            }
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

            value = new UndoRedo<string>(Value);
            LostKeyboardFocus += TextBox_LostKeyboardFocus;
            
            return this;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            Value = textBox.Text;

            if (value.Value != textBox.Text)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = textBox.Text;
                    UndoRedoManager.Commit();
                }

                //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                //    p.PropertyName, p.Value));
            }
        }
    }
}
