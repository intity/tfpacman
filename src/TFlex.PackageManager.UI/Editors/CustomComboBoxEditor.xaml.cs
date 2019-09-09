using System.Diagnostics;
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
    /// <summary>
    /// Interaction logic for CustomComboBoxEditor.xaml
    /// </summary>
    public partial class CustomComboBoxEditor : UserControl, ITypeEditor
    {
        UndoRedo<int> value;

        public CustomComboBoxEditor()
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
                        comboBox.SelectedIndex = value.Value;

                        //Debug.WriteLine(string.Format("Undo: [name: {0}, value: {1}]", 
                        //    p.PropertyName, p.Value));
                    }
                    break;
                case CommandDoneType.Redo:
                    if (e.Caption == p.PropertyName)
                    {
                        comboBox.SelectedIndex = value.Value;

                        //Debug.WriteLine(string.Format("Redo: [name: {0}, value: {1}]",
                        //    p.PropertyName, p.Value));
                    }
                    break;
            }
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
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);

            switch (propertyItem.PropertyName)
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

            value = new UndoRedo<int>(Value);
            SetItems(propertyItem.PropertyName);
            comboBox.SelectedIndex = Value;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;

            return this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = DataContext as PropertyItem;
            Value = comboBox.SelectedIndex;

            if (value.Value != comboBox.SelectedIndex)
            {
                using (UndoRedoManager.Start(p.PropertyName))
                {
                    value.Value = comboBox.SelectedIndex;
                    UndoRedoManager.Commit();
                }

                //Debug.WriteLine(string.Format("Commit: [name: {0}, value: {1}]", 
                //    p.PropertyName, p.Value));
            }
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
                    comboBox.Items.Add("DWG");
                    comboBox.Items.Add("DXF");
                    comboBox.Items.Add("DXB");
                    break;
                case 3:
                    comboBox.Items.Add("BMP");
                    comboBox.Items.Add("JPEG");
                    comboBox.Items.Add("GIF");
                    comboBox.Items.Add("TIFF");
                    comboBox.Items.Add("PNG");
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
                    comboBox.Items.Add("12");
                    comboBox.Items.Add("13");
                    comboBox.Items.Add("14");
                    comboBox.Items.Add("2000");
                    comboBox.Items.Add("2004");
                    comboBox.Items.Add("2007");
                    comboBox.Items.Add("2010");
                    comboBox.Items.Add("2013");
                    break;
                case "ConvertAreas":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_1_1", 0));
                    break;
                case "ConvertToLines":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_2_1", 0));
                    break;
                case "ConvertDimensions":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_1", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_3_2", 0));
                    break;
                case "ConvertLineText":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_4_1", 0));
                    break;
                case "ConvertMultitext":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_1", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_5_2", 0));
                    break;
                case "BiarcInterpolationForSplines":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_1, "dn4_6_1", 0));
                    break;
                #endregion
                #region Translator_7
                case "Version":
                    comboBox.Items.Add("JT 8.1");
                    comboBox.Items.Add("JT 9.5");
                    break;
                #endregion
                #region Translator_10
                case "Protocol":
                    comboBox.Items.Add("AP203");
                    comboBox.Items.Add("AP214");
                    comboBox.Items.Add("AP242");
                    break;
                #endregion
                #region Translator_3D
                case "ExportMode":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_1_1", 0));
                    break;
                case "ColorSource":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn4_2_1", 0));
                    break;
                case "ImportMode":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_1", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_1_2", 0));
                    break;
                case "Heal":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_1", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_2", 0));
                    break;
                case "CreateAccurateEdges":
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_0", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_1", 0));
                    comboBox.Items.Add(Resource.GetString(Resource.TRANSLATOR_3D, "dn5_2_2", 0));
                    break;
                    #endregion
            }
        }
    }
}
