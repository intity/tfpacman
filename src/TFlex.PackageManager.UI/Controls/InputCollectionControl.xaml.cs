using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.UI;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#pragma warning disable CA1721
#pragma warning disable CA1819

namespace TFlex.PackageManager.Controls
{
    /// <summary>
    /// Interaction logic for InputCollectionControl.xaml
    /// </summary>
    public partial class InputCollectionControl : UserControl, ITypeEditor
    {
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
            bool excludeFromSearch = false;
            PropertyItem pi = DataContext as PropertyItem;
            Translator_0 tr = pi.Instance as Translator_0;

            switch (pi.PropertyName)
            {
                case "PageNames":
                    excludeFromSearch = tr.ExcludePage;
                    break;
                case "ProjectionNames":
                    excludeFromSearch = tr.ExcludeProjection;
                    break;
            }

            ListValues imt = new ListValues
            {
                Owner = Window.GetWindow(Parent),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MultilineText = Value.ToString("\r\n"),
                ExcludeFromSeach = excludeFromSearch
            };

            if (imt.ShowDialog() == true)
            {
                Value = imt.MultilineText.Length > 0 
                    ? imt.MultilineText.Replace("\r", "").Split('\n') 
                    : new string[] { };

                textbox.Text = string.Format("[{0}]", Value.Length);
                
                switch (pi.PropertyName)
                {
                    case "PageNames":
                        tr.ExcludePage = imt.ExcludeFromSeach;
                        break;
                    case "ProjectionNames":
                        tr.ExcludeProjection = imt.ExcludeFromSeach;
                        break;
                }
            }
        }
    }
}
