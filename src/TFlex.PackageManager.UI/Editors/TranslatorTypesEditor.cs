using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TFlex.PackageManager.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TFlex.PackageManager.Editors
{
    public class TranslatorTypesEditor : ITypeEditor
    {
        private readonly TextBox textBox;

        public TranslatorTypesEditor()
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
            var types = propertyItem.Value as TranslatorTypes;

            textBox.Text = string.Format("[{0}, {1}, {2}, {3}, {4}]", 
                types.Default ? 1 : 0,
                types.Acad    ? 1 : 0,
                types.Bitmap  ? 1 : 0,
                types.Pdf     ? 1 : 0,
                types.Step    ? 1 : 0);

            return textBox;
        }
    }
}
