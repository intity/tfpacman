using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.UI;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.Editors
{
    /// <summary>
    /// Interaction logic for VariablesEditor.xaml
    /// </summary>
    public partial class VariablesEditor : UserControl, ITypeEditor
    {
        UndoRedo<XElement> variables;

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
                    if (e.Caption == p.PropertyName)
                    {
                        Do();
                        textbox.Text = string.Format("[{0}]", Value.Elements().Count());

                        //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]", 
                        //    p.PropertyName, p.Value));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
                    {
                        Do();
                        textbox.Text = string.Format("[{0}]", Value.Elements().Count());

                        //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
            }
        }

        private void Do()
        {
            Value.Elements().Remove();

            foreach (var e in variables.Value.Elements())
            {
                Value.Add(new XElement(e));
            }
        }

        public XElement Value
        {
            get => (XElement)GetValue(ValueProperty);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(XElement), typeof(VariablesEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
            
            variables = new UndoRedo<XElement>(new XElement(Value));
            textbox.Text = string.Format("[{0}]", Value.Elements().Count());

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
                textbox.Text = string.Format("[{0}]", Value.Elements().Count());

                using (UndoRedoManager.Start(pi.PropertyName))
                {
                    variables.Value = new XElement(Value);
                    UndoRedoManager.Commit();
                }

                //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                //    pi.PropertyName, Value));
            }
        }
    }
}
