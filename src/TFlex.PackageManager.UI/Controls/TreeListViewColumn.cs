using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TFlex.PackageManager.Controls
{
    public enum TreeListViewColumnType
    {
        Label = 0,
        CheckBox
    }

    /// <summary>
    /// The GridViewColumn class override.
    /// </summary>
    //[ContentProperty("Header")]
    //[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    //[StyleTypedProperty(Property = "HeaderContainerStyle", StyleTargetType = typeof(GridViewColumnHeader))]
    public class TreeListViewColumn : GridViewColumn
    {
        static TreeListViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(TreeListViewColumn),
                new FrameworkPropertyMetadata(null,
                new CoerceValueCallback(OnCoerceWidth)));

            //CellTemplateProperty = 
        }

        /// <summary>
        /// Minimum width for column.
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Maximum width for column.
        /// </summary>
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        private static object OnCoerceWidth(DependencyObject obj, object baseValue)
        {
            if (obj is TreeListViewColumn column)
            {
                if (column.MinWidth > 0 && column.MinWidth > (double)baseValue)
                    return column.MinWidth;

                if (column.MaxWidth > 0 && column.MaxWidth < (double)baseValue)
                    return column.MaxWidth;
            }

            return baseValue;
        }

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(TreeListViewColumn),
                new FrameworkPropertyMetadata(double.NaN,
                new PropertyChangedCallback(OnWidthChanged)));

        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register("MaxWidth", typeof(double), typeof(TreeListViewColumn),
                new FrameworkPropertyMetadata(double.NaN,
                new PropertyChangedCallback(OnWidthChanged)));

        private static void OnWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj != null) (obj as TreeListViewColumn).CoerceValue(WidthProperty);
        }

        public TreeListViewColumnType ColumnType
        {
            get { return (TreeListViewColumnType)GetValue(ColumnTypeProperty); }
            set { SetValue(ColumnTypeProperty, value); }
        }

        public static readonly DependencyProperty ColumnTypeProperty =
            DependencyProperty.Register("ColumnType", typeof(TreeListViewColumnType), typeof(TreeListViewColumn),
                new FrameworkPropertyMetadata(TreeListViewColumnType.Label,
                    new PropertyChangedCallback(OnColumnTypeChanged)));

        private static void OnColumnTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeListViewColumn tvc)
            {
                switch (tvc.ColumnType)
                {
                    case TreeListViewColumnType.Label:
                        break;
                    case TreeListViewColumnType.CheckBox:
                        break;
                }

            }
        }
    }
}