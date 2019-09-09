using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Editors
{
    /// <summary>
    /// Interaction logic for InputScaleControl.xaml
    /// </summary>
    public partial class ScaleEditor : UserControl, ITypeEditor
    {
        readonly Dictionary<string, decimal> scale;
        readonly string other;
        UndoRedo<decimal?> value;

        public ScaleEditor()
        {
            InitializeComponent();

            scale = new Dictionary<string, decimal>
            {
                { Resource.GetString(Resource.TRANSLATOR_0, "dn1_3_1", 0), 99999 },
                { "1:1",    1 },
                { "1:2",    0.5m },
                { "1:4",    0.25m },
                { "1:5",    0.2m },
                { "1:10",   0.1m },
                { "1:15",   0.0666666666666666666666666667m },
                { "1:20",   0.05m },
                { "1:25",   0.04m },
                { "1:40",   0.025m },
                { "1:50",   0.02m },
                { "1:75",   0.0133333333333333333333333333m },
                { "1:100",  0.01m },
                { "1:200",  0.005m },
                { "1:250",  0.004m },
                { "1:500",  0.002m },
                { "1:1000", 0.001m },
                { "2:1",    2 },
                { "2.5:1",  2.5m },
                { "4:1",    4 },
                { "5:1",    5 },
                { "10:1",   10 },
                { "20:1",   20 },
                { "50:1",   50 },
                { "100:1",  100 }
            };

            foreach (var i in scale) comboBox.Items.Add(i.Key);
            other = Resource.GetString(Resource.TRANSLATOR_0, "dn1_3_2", 0);

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
            DependencyProperty.Register("Value", typeof(decimal?), typeof(ScaleEditor),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

            value = new UndoRedo<decimal?>(Value);
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            decimalUpDown.ValueChanged += DecimalUpDown_ValueChanged;

            return this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex != 25 && scale.ElementAt(comboBox.SelectedIndex).Value != Value)
            {
                decimalUpDown.Value = scale.ElementAt(comboBox.SelectedIndex).Value;
                decimalUpDown.IsEnabled = true;
            }
        }

        private void DecimalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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

            if (comboBox.SelectedIndex != -1 && 
                comboBox.SelectedIndex != 25 && scale.ElementAt(comboBox.SelectedIndex).Value == Value)
                return;

            if (Value.Value == 99999)
            {
                comboBox.SelectedIndex = 0;
                decimalUpDown.IsEnabled = false;
            }
            else if (scale.ContainsValue(Value.Value))
            {
                for (int i = 0; i < scale.Count; i++)
                {
                    if (scale.ElementAt(i).Value == Value.Value)
                    {
                        comboBox.SelectedIndex = i;
                        decimalUpDown.IsEnabled = true;
                        break;
                    }
                }

                if (comboBox.Items.Count > 25)
                    comboBox.Items.RemoveAt(25);
            }
            else if (comboBox.Items.Count == 25)
            {
                comboBox.Items.Add(other);
                comboBox.SelectedIndex = 25;
            }
        }
    }
}
