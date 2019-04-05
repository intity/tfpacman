using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#pragma warning disable CA1721

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputScaleControl.xaml
    /// </summary>
    public partial class InputScaleControl : UserControl, ITypeEditor
    {
        private int index = -1;
        private Dictionary<string, decimal> scale = new Dictionary<string, decimal>();
        private string other;

        public InputScaleControl()
        {
            InitializeComponent();

            scale.Add(Resource.GetString(Resource.TRANSLATOR_0, "dn1_3_1", 0), 99999);
            scale.Add("1:1",    1);
            scale.Add("1:2",    0.5m);
            scale.Add("1:4",    0.25m);
            scale.Add("1:5",    0.2m);
            scale.Add("1:10",   0.1m);
            scale.Add("1:15",   0.0666666666666666666666666667m);
            scale.Add("1:20",   0.05m);
            scale.Add("1:25",   0.04m);
            scale.Add("1:40",   0.025m);
            scale.Add("1:50",   0.02m);
            scale.Add("1:75",   0.0133333333333333333333333333m);
            scale.Add("1:100",  0.01m);
            scale.Add("1:200",  0.005m);
            scale.Add("1:250",  0.004m);
            scale.Add("1:500",  0.002m);
            scale.Add("1:1000", 0.001m);
            scale.Add("2:1",    2);
            scale.Add("2.5:1",  2.5m);
            scale.Add("4:1",    4);
            scale.Add("5:1",    5);
            scale.Add("10:1",   10);
            scale.Add("20:1",   20);
            scale.Add("50:1",   50);
            scale.Add("100:1",  100);

            foreach (var i in scale) comboBox.Items.Add(i.Key);
            other = Resource.GetString(Resource.TRANSLATOR_0, "dn1_3_2", 0);
        }

        public decimal? Value
        {
            get { return (decimal?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(InputScaleControl),
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
            return this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] _scale;

            if (comboBox.SelectedIndex == 0 && decimalUpDown.Value == 99999)
            {
                decimalUpDown.IsEnabled = false;
            }
            else if (comboBox.SelectedIndex == 0 && decimalUpDown.Value != 99999)
            {
                decimalUpDown.Value = scale.ElementAt(comboBox.SelectedIndex).Value;
                decimalUpDown.IsEnabled = false;
            }
            else if (comboBox.SelectedIndex != index && (_scale = ((string)comboBox.SelectedValue).Split(':')).Length == 2)
            {
                decimal.TryParse(_scale[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal num1);
                decimal.TryParse(_scale[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal num2);

                decimalUpDown.Value = (num1 / num2);
                decimalUpDown.IsEnabled = true;
            }
        }

        private void DecimalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is DecimalUpDown dud)
            {
                if (dud.Value == null)
                    return;

                if (scale.ContainsValue(dud.Value.Value))
                {
                    for (int i = 0; i < scale.Count; i++)
                    {
                        if (scale.ElementAt(i).Value == dud.Value.Value)
                        {
                            comboBox.SelectedIndex = (index = i);
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
}
