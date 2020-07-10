using System;
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
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            var m = Value as Modules;
            comboBox.SelectedIndex = m.Index;
            //Debug.WriteLine("ResolveEditor");
            return this;
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object),
                typeof(ModulesEditor), new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is PropertyItem item && item.Instance is Header cfg))
                return;

            if (cfg.TIndex != comboBox.SelectedIndex)
            {
                cfg.TIndex = comboBox.SelectedIndex;

                //Debug.WriteLine(string.Format("modules [{0}]",
                //    (Value as Modules).ToString()));
            }
        }
    }
}