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
        UndoRedo<bool?> value;

        public CustomCheckBoxEditor()
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
                        checkBox.IsChecked = value.Value;

                        //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
                    {
                        checkBox.IsChecked = value.Value;

                        //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]", 
                        //    p.PropertyName, p.Value));
                    }
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

            value = new UndoRedo<bool?>(Value);
            checkBox.Checked += CheckBox_IsChecked;
            checkBox.Unchecked += CheckBox_IsChecked;

            return this;
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (value.Value != Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = Value;
                    UndoRedoManager.Commit();

                    //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                    //    p.PropertyName, p.Value));
                }
            }
        }
    }
}