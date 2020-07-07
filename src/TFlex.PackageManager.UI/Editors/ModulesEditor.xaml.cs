using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TFlex.PackageManager.Editors
{
    /// <summary>
    /// Interaction logic for ModulesEditor.xaml
    /// </summary>
    public partial class ModulesEditor : UserControl, ITypeEditor
    {
        public ModulesEditor()
        {
            InitializeComponent();

            comboBox.Items.Add("Document");
            comboBox.Items.Add("Acad");
            comboBox.Items.Add("Acis");
            comboBox.Items.Add("Bitmap");
            comboBox.Items.Add("Iges");
            comboBox.Items.Add("Jt");
            comboBox.Items.Add("Pdf");
            comboBox.Items.Add("Step");
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = BindingMode.Default
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            return this;
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ModulesEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is PropertyItem item && item.Instance is Header header))
                return;

            comboBox.SelectedIndex = (int)(header.Translator as Translator).TMode;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is PropertyItem item && item.Instance is Header header))
                return;

            Value = header.TModules[comboBox.SelectedIndex];
        }
    }
}
