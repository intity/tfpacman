using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.ComponentModel;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.UI
{
    /// <summary>
    /// Interaction logic for VariablesUI.xaml
    /// </summary>
    public partial class VariablesUI : Window
    {
        #region private fields
        Variables variables;
        readonly VariableAction action;
        #endregion

        public VariablesUI(VariableAction action)
        {
            InitializeComponent();
            this.action = action;

            #region init resource
            button_1.Content = Resource.GetString(Resource.VARIABLES_UI, "button_1", 0);
            button_2.Content = Resource.GetString(Resource.VARIABLES_UI, "button_2", 0);
            column_1.Header  = Resource.GetString(Resource.VARIABLES_UI, "column_1", 0);
            column_2.Header  = Resource.GetString(Resource.VARIABLES_UI, "column_2", 0);
            column_3.Header  = Resource.GetString(Resource.VARIABLES_UI, "column_3", 0);
            column_4.Header  = Resource.GetString(Resource.VARIABLES_UI, "column_4", 0);

            column_1.HeaderStyle = GetHeaderStyle(Resource.GetString(Resource.VARIABLES_UI, "column_1", 1));
            column_2.HeaderStyle = GetHeaderStyle(Resource.GetString(Resource.VARIABLES_UI, "column_2", 1));
            column_3.HeaderStyle = GetHeaderStyle(Resource.GetString(Resource.VARIABLES_UI, "column_3", 1));
            column_4.HeaderStyle = GetHeaderStyle(Resource.GetString(Resource.VARIABLES_UI, "column_4", 1));
            column_5.HeaderStyle = GetHeaderStyle(Resource.GetString(Resource.VARIABLES_UI, "column_5", 1));

            switch (action)
            {
                case VariableAction.Add:
                    column_2.Visibility = Visibility.Collapsed;
                    break;
                case VariableAction.Edit:
                    column_2.Visibility = Visibility.Collapsed;
                    break;
                case VariableAction.Rename:
                    column_3.Visibility = Visibility.Collapsed;
                    column_4.Visibility = Visibility.Collapsed;
                    column_5.Visibility = Visibility.Collapsed;
                    Width = 500;
                    break;
                case VariableAction.Remove:
                    column_2.Visibility = Visibility.Collapsed;
                    column_3.Visibility = Visibility.Collapsed;
                    column_4.Visibility = Visibility.Collapsed;
                    column_5.Visibility = Visibility.Collapsed;
                    Width = 300;
                    break;
            }
            #endregion
        }

        #region internal properties
        /// <summary>
        /// Data source.
        /// </summary>
        internal Variables DataSource { get; set; }
        #endregion

        #region events
        private void Variables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VariableModel item;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    item = e.NewItems[0] as VariableModel;
                    item.PropertyChanged += Variable_PropertyChanged;
                    break;
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            var item = e.Row.Item as VariableModel;

            var value1 = item.Name == string.Empty;
            var value2 = item.Name != string.Empty && item.HasErrors;
            var value3 = item.OldName == string.Empty;
            var value4 = item.OldName != string.Empty && item.HasErrors;

            if (action == VariableAction.Rename)
            {
                e.Cancel = (value1 || value2) | (value3 || value4);
            }
            else
            {
                e.Cancel = value1 || value2;
            }
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is VariableModel item))
                return;

            bool isEmpty = action == VariableAction.Rename 
                ? item.Name == string.Empty | item.OldName == string.Empty
                : item.Name == string.Empty;

            button_1.IsEnabled = !(item.HasErrors || isEmpty) 
                ? !XNode.DeepEquals(variables.Data, DataSource.Data) 
                : false;
        }

        private void Data_Changed(object sender, XObjectChangeEventArgs e)
        {
            bool isEmpty = false;

            if (sender is XElement element)
            {
                bool value1 = element.Attribute("name").Value == string.Empty;
                bool value2 = element.Attribute("oldname").Value == string.Empty;
                isEmpty = action == VariableAction.Rename ? value1 | value2 : value1;
            }

            button_1.IsEnabled = !isEmpty && !XNode.DeepEquals(variables.Data, DataSource.Data);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            variables = DataSource.Clone() as Variables;
            foreach (var i in variables)
            {
                i.PropertyChanged += Variable_PropertyChanged;
            }
            dataGrid.ItemsSource = variables;
            dataGrid.RowEditEnding      += DataGrid_RowEditEnding;
            variables.CollectionChanged += Variables_CollectionChanged;
            variables.Data.Changed      += Data_Changed;

            button_1.IsEnabled = false;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DataSource.Clear();

            foreach (var i in variables)
            {
                DataSource.Add(i);
            }

            DialogResult = true;
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
        #endregion

        #region extension methods
        private Style GetHeaderStyle(string toolTip)
        {
            Style style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter(ToolTipService.ToolTipProperty, toolTip));
            return style;
        }
        #endregion
    }
}