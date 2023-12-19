using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.UI.Common;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;


#pragma warning disable CA1721
#pragma warning disable CA1819

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for StringArrayEditor.xaml
    /// </summary>
    public partial class StringArrayEditor : UserControl, ITypeEditor
    {
        private UndoRedo<string[]> buffer1;
        private UndoRedo<bool> buffer2;

        public StringArrayEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (e.Caption != p.PropertyName)
                return;
            
            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                case CommandDoneType.Redo:
                    Do();
                    break;
            }
        }

        private void Do()
        {
            PropertyItem pi = DataContext as PropertyItem;
            Translator_0 tr = pi.Instance as Translator_0;

            if (Value != buffer1.Value)
            {
                Value = buffer1.Value;
                textbox.Text = string.Format("[{0}]", Value.Length);
            }

            switch (pi.PropertyName)
            {
                case "PageNames":
                    if (tr.ExcludePage != buffer2.Value)
                        tr.ExcludePage = buffer2.Value;
                    break;
                case "ProjectionNames":
                    if (tr.ExcludeProjection != buffer2.Value)
                        tr.ExcludeProjection = buffer2.Value;
                    break;
            }
        }

        public string[] Value
        {
            get => (string[])GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string[]), typeof(StringArrayEditor),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Translator_0 tr = propertyItem.Instance as Translator_0;

            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            textbox.Text = string.Format("[{0}]", Value.Length);
            buffer1 = new UndoRedo<string[]>(Value);

            switch (propertyItem.PropertyName)
            {
                case "PageNames":
                    buffer2 = new UndoRedo<bool>(tr.ExcludePage);
                    break;
                case "ProjectionNames":
                    buffer2 = new UndoRedo<bool>(tr.ExcludeProjection);
                    break;
            }

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

            Views.ListValues imt = new Views.ListValues
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

                using (UndoRedoManager.Start(pi.PropertyName))
                {
                    buffer1.Value = Value;
                    buffer2.Value = imt.ExcludeFromSeach;
                    UndoRedoManager.Commit();
                }
            }
        }
    }
}