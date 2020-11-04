using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TFlex.PackageManager.UI.Editors
{
    public class PageTypesEditor : ITypeEditor
    {
        private readonly TextBox textBox;

        public PageTypesEditor()
        {
            textBox = new TextBox
            {
                BorderBrush = Brushes.White,
                Padding = new Thickness(2)
            };

            textBox.Loaded += TextBox_Loaded;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.IsReadOnly = true;
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var pageTypes = propertyItem.Value as PageTypes;

            textBox.Text = string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]",
                pageTypes.Normal          ? 1 : 0,
                pageTypes.Workplane       ? 1 : 0,
                pageTypes.Auxiliary       ? 1 : 0,
                pageTypes.Text            ? 1 : 0,
                pageTypes.BillOfMaterials ? 1 : 0,
                pageTypes.Circuit         ? 1 : 0);

            return textBox;
        }
    }
}
