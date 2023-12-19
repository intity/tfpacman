using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI.Editors
{
    public partial class CustomCheckBoxEditor : UserControl, ITypeEditor
    {
        private UndoRedo<bool?> buffer;

        public CustomCheckBoxEditor()
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
                    checkBox.IsChecked = buffer.Value;
                    break;
            }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool?), typeof(CustomCheckBoxEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool? Value
        {
            get => (bool?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            buffer = new UndoRedo<bool?>(Value);
            checkBox.Checked += CheckBox_IsChecked;
            checkBox.Unchecked += CheckBox_IsChecked;

            return this;
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (buffer.Value != Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    buffer.Value = Value;
                    UndoRedoManager.Commit();
                }
            }
        }
    }
}