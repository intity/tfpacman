using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using TFlex.PackageManager.UI.Configuration;

namespace TFlex.PackageManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Variables.xaml
    /// </summary>
    public partial class Variables : Window
    {
        #region private fields
        VariableCollection variables;
        readonly VariableAction action;
        #endregion

        public Variables(VariableAction action)
        {
            InitializeComponent();
            this.action = action;

            #region initialize resources
            button1.Content = Properties.Resources.Strings["ui_3:btn_1"][0];
            button2.Content = Properties.Resources.Strings["ui_3:btn_2"][0];
            column1.Header  = Properties.Resources.Strings["ui_3:col_1"][0];
            column2.Header  = Properties.Resources.Strings["ui_3:col_2"][0];
            column3.Header  = Properties.Resources.Strings["ui_3:col_3"][0];
            column4.Header  = Properties.Resources.Strings["ui_3:col_4"][0];

            column1.HeaderStyle = GetHeaderStyle(Properties.Resources.Strings["ui_3:col_1"][1]);
            column2.HeaderStyle = GetHeaderStyle(Properties.Resources.Strings["ui_3:col_2"][1]);
            column3.HeaderStyle = GetHeaderStyle(Properties.Resources.Strings["ui_3:col_3"][1]);
            column4.HeaderStyle = GetHeaderStyle(Properties.Resources.Strings["ui_3:col_4"][1]);
            column5.HeaderStyle = GetHeaderStyle(Properties.Resources.Strings["ui_3:col_5"][1]);
            #endregion

            switch (action)
            {
                case VariableAction.Add:
                    column2.Visibility = Visibility.Collapsed;
                    break;
                case VariableAction.Edit:
                    column2.Visibility = Visibility.Collapsed;
                    break;
                case VariableAction.Rename:
                    column3.Visibility = Visibility.Collapsed;
                    column4.Visibility = Visibility.Collapsed;
                    column5.Visibility = Visibility.Collapsed;
                    Width = 500;
                    break;
                case VariableAction.Remove:
                    column2.Visibility = Visibility.Collapsed;
                    column3.Visibility = Visibility.Collapsed;
                    column4.Visibility = Visibility.Collapsed;
                    column5.Visibility = Visibility.Collapsed;
                    Width = 300;
                    break;
            }
        }

        #region internal properties
        /// <summary>
        /// Data source.
        /// </summary>
        internal VariableCollection DataSource { get; set; }
        #endregion

        #region private properties
        private bool IsValid { get; set; } = true;
        #endregion

        #region events
        private void DataGrid_Error(object sender, ValidationErrorEventArgs e)
        {
            switch (e.Action)
            {
                case ValidationErrorEventAction.Added:
                    IsValid = button1.IsEnabled = false;
                    break;
                case ValidationErrorEventAction.Removed:
                    IsValid = true;
                    break;
            }
        }

        private void Variables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VariableModel item;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    item = e.NewItems[0] as VariableModel;
                    item.Action = (int)action;
                    item.PropertyChanged += Variable_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    button1.IsEnabled = !Enumerable.SequenceEqual(variables, DataSource);
                    break;
            }
        }

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "EndEdit")
                return;

            button1.IsEnabled = IsValid && !Enumerable.SequenceEqual(variables, DataSource);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            variables = DataSource.Clone() as VariableCollection;
            foreach (var i in variables)
            {
                i.PropertyChanged += Variable_PropertyChanged;
            }
            dataGrid.ItemsSource = variables;
            variables.CollectionChanged += Variables_CollectionChanged;

            button1.IsEnabled = false;
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