using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
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
        readonly VariableAction action;
        readonly ObservableCollection<VariableModel> model;
        XElement data;
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

            model = new ObservableCollection<VariableModel>();
        }

        #region internal properties
        /// <summary>
        /// Data source.
        /// </summary>
        internal XElement DataSource { get; set; }
        #endregion

        #region events
        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VariableModel item;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    item = e.NewItems[0] as VariableModel;
                    item.PropertyChanged += Variable_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    item = e.OldItems[0] as VariableModel;
                    foreach (var i in data.Elements())
                    {
                        if (XNode.DeepEquals(i, item.Data))
                        {
                            i.Remove();
                            break;
                        }
                    }
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
            var value5 = action != VariableAction.Rename && value1 || value2;
            var value6 = action == VariableAction.Rename && (value1 || value2) | (value3 || value4);

            //Debug.WriteLine(item.ToString());

            if (value5)
            {
                if (NameValidation(item))
                    return;

                e.Cancel = true;
            }

            if (value6)
            {
                if (NameValidation(item))
                    return;

                e.Cancel = true;
            }

            if (!(value5 | value6))
            {
                if (item.Data.HasAttributes == false)
                {
                    item.InitData(action);
                    data.Add(item.Data);
                }
            }
        }

        private void Data_Changed(object sender, XObjectChangeEventArgs e)
        {
            button_1.IsEnabled = !XNode.DeepEquals(data, DataSource);
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is VariableModel item))
                return;
            
            bool isEmptyName = action != VariableAction.Rename
                ? item.Name == string.Empty
                : item.Name == string.Empty || item.OldName == string.Empty;

            button_1.IsEnabled = !(item.HasErrors || isEmptyName) 
                ? !XNode.DeepEquals(data, DataSource) 
                : false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            data = new XElement(DataSource);

            foreach (var i in data.Elements())
            {
                VariableModel variable = new VariableModel();
                variable.LoadData(i);
                variable.PropertyChanged += Variable_PropertyChanged;
                model.Add(variable);
            }

            dataGrid.ItemsSource = model;
            dataGrid.RowEditEnding  += DataGrid_RowEditEnding;
            model.CollectionChanged += Data_CollectionChanged;
            data.Changed += Data_Changed;

            button_1.IsEnabled = false;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (DataSource.HasElements)
                DataSource.Elements().Remove();

            foreach (var i in data.Elements())
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
        /// <summary>
        /// The name validation in data.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// Returns true when the item is deleted, and false otherwise.
        /// </returns>
        private bool NameValidation(VariableModel item)
        {
            string[] names;
            if (action == VariableAction.Rename)
            {
                names = new string[] { "name", "oldname" };
            }
            else
            {
                names = new string[] { "name" };
            }

            foreach (var i in data.Elements())
            {
                foreach (var name in names)
                {
                    if (i.Attribute(name).Value == string.Empty)
                    {
                        i.Remove();
                        model.Remove(item);
                        return true;
                    }
                }
            }

            return false;
        }

        private Style GetHeaderStyle(string toolTip)
        {
            Style style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter(ToolTipService.ToolTipProperty, toolTip));
            return style;
        }
        #endregion
    }
}