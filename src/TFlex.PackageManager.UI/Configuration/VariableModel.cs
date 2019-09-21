using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using TFlex.PackageManager.Common;

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
    public class VariableModel : INotifyPropertyChanged, INotifyDataErrorInfo
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

        Dictionary<string, List<string>> objErrors;
        string[] error_messages;
        char[] pattern;
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
            InitResources();
        }

        /// <summary>
        /// The constructor for preload data.
        /// </summary>
        /// <param name="data"></param>
        public VariableModel(XElement data)
        {
            Data = data;

            LoadData();
            InitResources();
        }

        #region methods
        /// <summary>
        /// Initialize resources.
        /// </summary>
        private void InitResources()
        {
            objErrors = new Dictionary<string, List<string>>();
            error_messages = new string[]
            {
                Resource.GetString(Resource.VARIABLE_MODEL, "message_0", 0),
                Resource.GetString(Resource.VARIABLE_MODEL, "message_1", 0)
            };

            pattern = new char[] {
                '`', '~', '!', '@', '#', '%', '^',
                '&', '*', '(', ')', '-', '+', '=',
                '[', ']', '{', '}', '|', ';', ':',
                '.', ',', '<', '>', '/', '?', ' ',
                '\\', '\'', '"'
            };
        }

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

                    NameValidation("Name", value);
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

                    NameValidation("OldName", value);
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

        #region INotifyDataErrorInfo Members
        /// <summary>
        /// Occurs when the validation errors have changed for 
        /// a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// The RaiseErrorChanged event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnRaiseErrorChanged(string name)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        }

        /// <summary>
        /// Gets a value that indicates whether the entity 
        /// has validation errors.
        /// </summary>
        [Browsable(false)]
        public bool HasErrors => objErrors.Count > 0;

        /// <summary>
        /// Gets the validation errors for a specified property or for 
        /// the entire entity.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        public IEnumerable GetErrors(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return objErrors.Values;

            objErrors.TryGetValue(name, out List<string> errors);
            return errors;
        }

        /// <summary>
        /// Add error to dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="index">Error message index.</param>
        private void AddError(string name, int index)
        {
            if (objErrors.TryGetValue(name, out List<string> errors) == false)
            {
                errors = new List<string>();
                objErrors.Add(name, errors);
            }

            if (errors.Contains(error_messages[index]) == false)
            {
                errors.Add(error_messages[index]);
            }

            OnRaiseErrorChanged(name);
        }

        /// <summary>
        /// Remove error from dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="index">Error message index.</param>
        private void RemoveError(string name, int index)
        {
            if (objErrors.TryGetValue(name, out List<string> errors))
            {
                errors.Remove(error_messages[index]);
                errors.Clear();
            }

            if (errors == null)
                return;

            if (errors.Count == 0)
            {
                objErrors.Remove(name);
                OnRaiseErrorChanged(name);
            }
        }

        /// <summary>
        /// Validate variable name.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        private void NameValidation(string name, string value)
        {
            char[] chars = value.ToCharArray();
            bool isDigit = chars.Length > 0 ? char.IsDigit(chars[0]) : false;
            int iError = isDigit ? 0 : value.Length > 0 ? 1 : -1;

            switch (iError)
            {
                case -1:
                    if (HasErrors)
                    {
                        foreach (string error in GetErrors(name))
                        {
                            if (error == error_messages[0])
                            {
                                RemoveError(name, 0);
                                break;
                            }
                            else
                            {
                                RemoveError(name, 1);
                                break;
                            }
                        }
                    }
                    break;
                case 0:
                    if (isDigit)
                        AddError(name, 0);
                    else if (HasErrors)
                        RemoveError(name, 0);
                    break;
                case 1:
                    if (!value.IsValid(pattern))
                        AddError(name, 1);
                    else if (HasErrors)
                        RemoveError(name, 1);
                    break;
            }
        }
        #endregion
    }
}