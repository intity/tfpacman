using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.UI;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputCollectionControl.xaml
    /// </summary>
    public partial class InputCollectionControl : UserControl, ITypeEditor
    {
        private ListValues imt;

        public InputCollectionControl()
        {
            InitializeComponent();
        }

        public string[] Value
        {
            get { return (string[])GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string[]), typeof(InputCollectionControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            textbox.Text = string.Format("[{0}]", Value.Length);

            return this;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            imt = new ListValues
            {
                Owner = Window.GetWindow(Parent),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MultilineText = Value.ToString("\r\n")
            };

            if (imt.ShowDialog() == true)
            {
                Value = imt.MultilineText.Length > 0 
                    ? imt.MultilineText.Replace("\r", "").Split('\n') 
                    : new string[] { };

                textbox.Text = string.Format("[{0}]", Value.Length);
            }
        }
    }
}
