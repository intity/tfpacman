using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for BiarcInterpolationControl.xaml
    /// </summary>
    public partial class BiarcInterpolationControl : UserControl, ITypeEditor
    {
        public BiarcInterpolationControl()
        {
            InitializeComponent();
        }

        public decimal? Value
        {
            get { return (decimal?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal?), typeof(BiarcInterpolationControl),
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
    }
}