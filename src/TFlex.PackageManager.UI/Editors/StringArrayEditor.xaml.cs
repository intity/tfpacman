using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.UI;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;
using System.Linq;

#pragma warning disable CA1721
#pragma warning disable CA1819

namespace TFlex.PackageManager.Editors
{
    /// <summary>
    /// Interaction logic for StringArrayEditor.xaml
    /// </summary>
    public partial class StringArrayEditor : UserControl, ITypeEditor
    {
        UndoRedo<string[]> value;
        UndoRedo<bool> excludeFS;

        public StringArrayEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                    if (UndoRedoManager.RedoCommands.Count() > 0 && 
                        UndoRedoManager.RedoCommands.First() == p.PropertyName) Do();
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 && 
                        UndoRedoManager.UndoCommands.First() == p.PropertyName) Do();
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        private void Do()
        {
            PropertyItem pi = DataContext as PropertyItem;
            Translator_0 tr = pi.Instance as Translator_0;

            if (Value != value.Value)
            {
                Value = value.Value;
                textbox.Text = string.Format("[{0}]", Value.Length);
            }

            switch (pi.PropertyName)
            {
                case "PageNames":
                    if (tr.ExcludePage != excludeFS.Value)
                        tr.ExcludePage = excludeFS.Value;
                    break;
                case "ProjectionNames":
                    if (tr.ExcludeProjection != excludeFS.Value)
                        tr.ExcludeProjection = excludeFS.Value;
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
            value = new UndoRedo<string[]>(Value);

            switch (propertyItem.PropertyName)
            {
                case "PageNames":
                    excludeFS = new UndoRedo<bool>(tr.ExcludePage);
                    break;
                case "ProjectionNames":
                    excludeFS = new UndoRedo<bool>(tr.ExcludeProjection);
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

                using (UndoRedoManager.Start(pi.PropertyName))
                {
                    value.Value = Value;
                    excludeFS.Value = imt.ExcludeFromSeach;
                    UndoRedoManager.Commit();
                }
            }
        }
    }
}