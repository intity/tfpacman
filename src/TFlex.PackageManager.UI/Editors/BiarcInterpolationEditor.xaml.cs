using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for BiarcInterpolationEditor.xaml
    /// </summary>
    public partial class BiarcInterpolationEditor : UserControl, ITypeEditor
    {
        UndoRedo<decimal?> value;

        public BiarcInterpolationEditor()
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
                        decimalUpDown.Value = value.Value;

                        //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
                    {
                        decimalUpDown.Value = value.Value;

                        //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
            }
        }

        public decimal? Value
        {
            get => (decimal?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(BiarcInterpolationEditor),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

            value = new UndoRedo<decimal?>(Value);
            decimalUpDown.ValueChanged += DecimalUpDown_ValueChanged;

            return this;
        }

        private void DecimalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(DataContext is PropertyItem p))
                return;
            
            if (value.Value != decimalUpDown.Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = decimalUpDown.Value;
                    UndoRedoManager.Commit();

                    //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                    //    p.PropertyName, p.Value));
                }
            }
        }
    }
}