﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.UI.Configuration;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

#pragma warning disable CA1721

namespace TFlex.PackageManager.UI.Editors
{
    /// <summary>
    /// Interaction logic for SewingEditor.xaml
    /// </summary>
    public partial class SewingEditor : UserControl, ITypeEditor
    {
        UndoRedo<bool> value1;
        UndoRedo<double?> value2;

        public SewingEditor()
        {
            InitializeComponent();
            UndoRedoManager.CommandDone += UndoRedoManager_CommandDone;
        }

        private void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            var tr = p.Instance as Translator3D;

            switch (e.CommandDoneType)
            {
                case CommandDoneType.Undo:
                    if (e.Caption == p.PropertyName) Do(tr);
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName) Do(tr);
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]",
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        private void Do(Translator3D tr)
        {
            if (checkBox.IsChecked != value1.Value)
            {
                checkBox.IsChecked = value1.Value;
                tr.Sewing = value1.Value;
            }

            if (doubleUpDown.Value != value2.Value)
                doubleUpDown.Value = value2.Value;
        }

        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(SewingEditor),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var tr = propertyItem.Instance as Translator3D;

            Binding binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            value1 = new UndoRedo<bool>(tr.Sewing);
            value2 = new UndoRedo<double?>(Value);

            checkBox.IsChecked = tr.Sewing;
            checkBox.Checked += CheckBox_IsChecked;
            checkBox.Unchecked += CheckBox_IsChecked;
            doubleUpDown.ValueChanged += DoubleUpDown_ValueChanged;

            return this;
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(DataContext is PropertyItem p))
                return;

            if (value2.Value != Value)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value2.Value = Value;
                    UndoRedoManager.Commit();

                    //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                    //    p.PropertyName, p.Value));
                }
            }
        }

        private void CheckBox_IsChecked(object sender, RoutedEventArgs e)
        {
            doubleUpDown.IsEnabled = checkBox.IsChecked.Value;
            var p = DataContext as PropertyItem;
            var tr = p.Instance as Translator3D;

            if (value1.Value != checkBox.IsChecked)
            {
                tr.Sewing = checkBox.IsChecked.Value;

                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value1.Value = tr.Sewing;
                    UndoRedoManager.Commit();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            doubleUpDown.IsEnabled = checkBox.IsChecked.Value;
        }
    }
}