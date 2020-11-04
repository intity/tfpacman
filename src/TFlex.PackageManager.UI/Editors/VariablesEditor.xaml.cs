using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for VariablesEditor.xaml
    /// </summary>
    public partial class VariablesEditor : UserControl, ITypeEditor
    {
        UndoRedo<VariableCollection> variables;

        public VariablesEditor()
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
                    if (e.Caption == p.PropertyName) Do();

                    //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]",
                    //    p.PropertyName, Value.Data));
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName) Do();

                    //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]",
                    //    p.PropertyName, Value.Data));
                    break;
            }
        }

        private void Do()
        {
            Value.Clear();
            foreach (var i in variables.Value)
            {
                Value.Add(i);
            }
            textbox.Text = string.Format("[{0}]", Value.Count());
        }

        public VariableCollection Value => (VariableCollection)GetValue(ValueProperty);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(VariableCollection), typeof(VariablesEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
            
            variables = new UndoRedo<VariableCollection>(Value.Clone() as VariableCollection);
            textbox.Text = string.Format("[{0}]", Value.Count());

            return this;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            PropertyItem pi = DataContext as PropertyItem;
            VariableAction action = VariableAction.Add;
            switch (pi.PropertyName)
            {
                case "EditVariables":
                    action = VariableAction.Edit;
                    break;
                case "RenameVariables":
                    action = VariableAction.Rename;
                    break;
                case "RemoveVariables":
                    action = VariableAction.Remove;
                    break;
            }

            VariablesUI vui = new VariablesUI(action)
            {
                Title = pi.DisplayName,
                Owner = Window.GetWindow(Parent),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataSource = Value
            };

            if (vui.ShowDialog() == true)
            {
                textbox.Text = string.Format("[{0}]", Value.Count());

                using (UndoRedoManager.Start(pi.PropertyName))
                {
                    variables.Value = Value.Clone() as VariableCollection;
                    UndoRedoManager.Commit();
                }

                //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]",
                //    pi.PropertyName, Value.Data));
            }
        }
    }
}
