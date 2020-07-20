using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using TFlex.PackageManager.Common;
using TFlex.PackageManager.Properties;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The action definition to processing variables.
    /// </summary>
    public enum VariableAction
    {
        Add,
        Edit,
        Rename,
        Remove
    }

    /// <summary>
    /// The VariableModel class.
    /// </summary>
    public class VariableModel : IEditableObject, INotifyPropertyChanged, ICloneable
    {
        #region private fields
        int action;
        string name;
        string oldname;
        string group;
        string expression;
        bool external;
        XAttribute data_0;
        XAttribute data_1;
        XAttribute data_2;
        XAttribute data_3;
        XAttribute data_4;
        XAttribute data_5;
        #endregion

        /// <summary>
        /// The default constructor.
        /// </summary>
        public VariableModel()
        {
            data_0 = new XAttribute("action", Action);
            data_1 = new XAttribute("name", Name);
            data_2 = new XAttribute("oldname", OldName);
            data_3 = new XAttribute("group", Group);
            data_4 = new XAttribute("expression", Expression);
            data_5 = new XAttribute("external", External ? "1" : "0");

            Data.Add(data_0, data_1, data_2, data_3, data_4, data_5);
        }

        /// <summary>
        /// The constructor for preload data.
        /// </summary>
        /// <param name="data"></param>
        public VariableModel(XElement data)
        {
            Data = data;
            LoadData();
        }

        #region methods
        /// <summary>
        /// Load data.
        /// </summary>
        private void LoadData()
        {
            foreach (var a in Data.Attributes())
            {
                switch (a.Name.ToString())
                {
                    case "action":
                        action = int.Parse(a.Value);
                        data_0 = a;
                        break;
                    case "name":
                        name = a.Value;
                        data_1 = a;
                        break;
                    case "oldname":
                        oldname = a.Value;
                        data_2 = a;
                        break;
                    case "group":
                        group = a.Value;
                        data_3 = a;
                        break;
                    case "expression":
                        expression = a.Value;
                        data_4 = a;
                        break;
                    case "external":
                        external = a.Value == "1";
                        data_5 = a;
                        break;
                }
            }
        }

        public override string ToString()
        {
            VariableAction mode = VariableAction.Add;
            switch (action)
            {
                case 1: mode = VariableAction.Edit;   break;
                case 2: mode = VariableAction.Rename; break;
                case 3: mode = VariableAction.Remove; break;
            }

            return string.Format(
                "Action: {0}, Name: {1}, OldName: {2}, Expression: {3}, External: {4}", 
                mode, Name, OldName, Expression, External);
        }
        #endregion

        #region public properties
        /// <summary>
        /// Variable data.
        /// </summary>
        public XElement Data { get; } = new XElement("variable");

        /// <summary>
        /// Processing mode for variable.
        /// </summary>
        public int Action
        {
            get => action;
            set
            {
                if (action != value)
                {
                    action = value;
                    data_0.Value = value.ToString();
                    OnPropertyChanged("Action");
                }
            }
        }

        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name
        {
            get => name ?? string.Empty;
            set
            {
                if (name != value)
                {
                    name = value;
                    data_1.Value = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Old variable name.
        /// </summary>
        public string OldName
        {
            get => oldname ?? string.Empty;
            set
            {
                if (oldname != value)
                {
                    oldname = value;
                    data_2.Value = value;
                    OnPropertyChanged("OldName");
                }
            }
        }

        /// <summary>
        /// Group name.
        /// </summary>
        public string Group
        {
            get => group ?? string.Empty;
            set
            {
                if (group != value)
                {
                    group = value;
                    data_3.Value = value;
                    OnPropertyChanged("Group");
                }
            }
        }

        /// <summary>
        /// Expression for variable.
        /// </summary>
        public string Expression
        {
            get => expression ?? string.Empty;
            set
            {
                if (expression != value)
                {
                    expression = value;
                    data_4.Value = value;
                    OnPropertyChanged("Expression");
                }
            }
        }

        /// <summary>
        /// External variable.
        /// </summary>
        public bool External
        {
            get => external;
            set
            {
                if (external != value)
                {
                    external = value;
                    data_5.Value = value ? "1" : "0";
                    OnPropertyChanged("External");
                }
            }
        }
        #endregion

        #region IEditableObject Members
        private bool inEdit;
        private VariableModel backup;

        public void BeginEdit()
        {
            if (inEdit) return;
            inEdit = true;
            backup = Clone() as VariableModel;
            //Debug.WriteLine("BeginEdit");
        }

        public void CancelEdit()
        {
            if (!inEdit) return;
            inEdit     = false;
            Name       = backup.Name;
            OldName    = backup.OldName;
            Group      = backup.Group;
            Expression = backup.Expression;
            External   = backup.External;
            //Debug.WriteLine("CancelEdit");
        }

        public void EndEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            backup = null;
            //Debug.WriteLine("EndEdit");
            OnPropertyChanged("EndEdit");
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OpPropertyChanged event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region ICloneable Members
        public object Clone()
        {
            return new VariableModel(new XElement(Data));
        }
        #endregion
    }

    public class CellValidation : ValidationRule
    {
        readonly char[] pattern;

        public CellValidation()
        {
            pattern = new char[] {
                '`', '~', '!', '@', '#', '%', '^',
                '&', '*', '(', ')', '-', '+', '=',
                '[', ']', '{', '}', '|', ';', ':',
                '.', ',', '<', '>', '/', '?', ' ',
                '\\', '\'', '"'
            };
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;
            if (str.IsDigit(0))
            {
                return new ValidationResult(false, Resources.Strings["ui_3:msg_1"][0]);
            }
            else if (!str.IsValid(pattern))
            {
                return new ValidationResult(false, Resources.Strings["ui_3:msg_2"][0]);
            }
            return ValidationResult.ValidResult;
        }
    }

    public class RowValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var variable = (value as BindingGroup).Items[0] as VariableModel;
            if (variable.Action != 2)
            {
                if (variable.Name == string.Empty)
                {
                    return new ValidationResult(false, Resources.Strings["ui_3:msg_3"][0]);
                }
            }
            else
            {
                if (variable.Name == string.Empty | variable.OldName == string.Empty)
                {
                    return new ValidationResult(false, Resources.Strings["ui_3:msg_3"][0]);
                }
                else if (variable.Name == variable.OldName)
                {
                    return new ValidationResult(false, Resources.Strings["ui_3:msg_4"][0]);
                }
                else
                {
                    var name1 = variable.Name.Substring(0, 1);
                    var name2 = variable.OldName.Substring(0, 1);
                    var type1 = name1 == "$" ? "text" : "real";
                    var type2 = name2 == "$" ? "text" : "real";
                    if (type1 != type2)
                    {
                        return new ValidationResult(false, 
                            Resources.Strings["ui_3:msg_5"][0]);
                    }
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}