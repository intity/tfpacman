using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace TFlex.PackageManager.Converters
{
    /// <summary>
    /// Convert Level to left margin
    /// </summary>
    public class LevelToIndentConverter : IValueConverter
    {
        private const double c_IndentSize = 18.0;

        public object Convert(object obj, Type type, object parameter, CultureInfo culture)
        {
            return new Thickness((int)obj * c_IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object obj, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}