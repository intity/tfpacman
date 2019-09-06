using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using UndoRedoFramework;

namespace TFlex.PackageManager.Editors
{
    public partial class CustomComboBoxEditor : ComboBox, ITypeEditor
    {
        UndoRedo<int> value;

        public CustomComboBoxEditor()
        {
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
                        UndoRedoManager.RedoCommands.Last() == p.PropertyName && SelectedIndex != value.Value)
                    {
                        SelectedIndex = value.Value;
                    }
                    break;
                case CommandDoneType.Redo:
                    if (UndoRedoManager.UndoCommands.Count() > 0 &&
                        UndoRedoManager.UndoCommands.Last() == p.PropertyName && SelectedIndex != value.Value)
                    {
                        SelectedIndex = value.Value;
                    }
                    break;
            }

            //Debug.WriteLine(string.Format("Action: [name: {0}, value: {1}, type: {2}]", 
            //    p.PropertyName, p.Value, e.CommandDoneType));
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(CustomComboBoxEditor),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

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

            switch(propertyItem.PropertyName)
            {
                case "Extension":
                    if (propertyItem.Instance is Translator_1)
                    {
                        SetExtensionItems(1);
                    }
                    if (propertyItem.Instance is Translator_3)
                    {
                        SetExtensionItems(3);
                    }
                    break;
            }

            SetItems(propertyItem.PropertyName);
            SelectedIndex = Value;
            SelectionChanged += ComboBox_SelectionChanged;
            value = new UndoRedo<int>(Value);

            return this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            Value = SelectedIndex;

            if (value.Value != SelectedIndex)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = SelectedIndex;
                    UndoRedoManager.Commit();
                }
            }

            //Debug.WriteLine(string.Format("PropertyItem: [name: {0}, value: {1}, can undo: {2}, can redo: {3}]",
            //    p.PropertyName, p.Value, UndoRedoManager.CanUndo, UndoRedoManager.CanRedo));
        }

        /// <summary>
        /// Set extension items.
        /// </summary>
        /// <param name="index">The translator index.</param>
        private void SetExtensionItems(int index)
        {
            switch (index)
            {
                case 1:
                    Items.Add("DWG");
                    Items.Add("DXF");
                    Items.Add("DXB");
                    break;
                case 3:
                    Items.Add("BMP");
                    Items.Add("JPEG");
                    Items.Add("GIF");
                    Items.Add("TIFF");
                    Items.Add("PNG");
                    break;
            }
        }

        /// <summary>
        /// Set items to combobox control.
        /// </summary>
        /// <param name="name">Property name.</param>
        private void SetItems(string name)
        {
            switch (name)
            {
                #region Translator_1
                case "AutocadExportFileVersion":
                    Items.Add("12");
                    Items.Add("13");
                    Items.Add("14");
                    Items.Add("2000");
                    Items.Add("2004");
                    Items.Add("2007");
                    Items.Add("2010");
                    Items.Add("2013");
                    break;
                case "ConvertAreas":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_1", 0));
                    break;
                case "ConvertToLines":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_1", 0));
                    break;
                case "ConvertDimensions":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_1", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_2", 0));
                    break;
                case "ConvertLineText":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_1", 0));
                    break;
                case "ConvertMultitext":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_1", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_2", 0));
                    break;
                case "BiarcInterpolationForSplines":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_1", 0));
                    break;
                #endregion
                #region Translator_7
                case "Version":
                    Items.Add("JT 8.1");
                    Items.Add("JT 9.5");
                    break;
                #endregion
                #region Translator_10
                case "Protocol":
                    Items.Add("AP203");
                    Items.Add("AP214");
                    Items.Add("AP242");
                    break;
                #endregion
                #region Translator_3D
                case "ExportMode":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_1", 0));
                    break;
                case "ColorSource":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_1", 0));
                    break;
                case "ImportMode":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_1", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_2", 0));
                    break;
                case "Heal":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_1", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_2", 0));
                    break;
                case "CreateAccurateEdges":
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_0", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_1", 0));
                    Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_2", 0));
                    break;
                    #endregion
            }
        }
    }
}