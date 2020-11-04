using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using TFlex.PackageManager.UI.Controls;

namespace TFlex.PackageManager.UI.Converters
{
    /// <summary>
    /// Convert Level to left margin
    /// </summary>
    public class LevelToIndentConverter : IValueConverter
    {
        private const double offset = 20.0;

        public object Convert(object obj, Type type, object parameter, CultureInfo culture)
        {
            if (!(obj is CustomTreeViewItem item))
                return new Thickness(0);

            return new Thickness(item.Level * offset, 0, 0, 0);
        }

        public object ConvertBack(object obj, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}