using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Editors
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
                    if (UndoRedoManager.RedoCommands.Count() > 0 &&
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName && decimalUpDown.Value != value.Value)
                    {
                        decimalUpDown.Value = value.Value;
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 &&
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName && decimalUpDown.Value != value.Value)
                    {
                        decimalUpDown.Value = value.Value;
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        public decimal? Value
        {
            get => (decimal?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(BiarcInterpolationEditor),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

            decimalUpDown.ValueChanged += DecimalUpDown_ValueChanged;
            value = new UndoRedo<decimal?>(Value);

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
                }
            }
        }
    }
}