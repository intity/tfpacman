using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Xml.Linq;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

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

        private Style GetHeaderStyle(string toolTip)
        {
            Style style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter(ToolTipService.ToolTipProperty, toolTip));
            return style;
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
                    item.InitData(action);
                    DataSource.Add(item.Data);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    item = e.OldItems[0] as VariableModel;
                    foreach (var i in DataSource.Elements())
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            data = new XElement(DataSource);

            foreach (var i in DataSource.Elements())
            {
                VariableModel variable = new VariableModel();
                variable.LoadData(i);
                model.Add(variable);
            }

            dataGrid.ItemsSource = model;
            model.CollectionChanged += Data_CollectionChanged;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = !XNode.DeepEquals(data, DataSource);
        } // OK

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // Cancel
        #endregion
    }
}