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
        private UndoRedo<decimal?> buffer;

        public BiarcInterpolationEditor()
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
                    decimalUpDown.Value = buffer.Value;
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

            buffer = new UndoRedo<decimal?>(Value);
            decimalUpDown.ValueChanged += DecimalUpDown_ValueChanged;

            return this;
        }

        private void DecimalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(DataContext is PropertyItem p))
                return;
            
            if (buffer.Value != decimalUpDown.Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    buffer.Value = decimalUpDown.Value;
                    UndoRedoManager.Commit();
                }
            }
        }
    }
}