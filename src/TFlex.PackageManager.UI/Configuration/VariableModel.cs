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

        readonly Dictionary<string, List<string>> objErrors;
        readonly string[] error_messages;
        readonly char[] pattern;
        #endregion

        public VariableModel()
        {
            Data = new XElement("variable");

            data_0 = new XAttribute("action", "0");
            data_1 = new XAttribute("name", "");
            data_2 = new XAttribute("oldname", "");
            data_3 = new XAttribute("group", "");
            data_4 = new XAttribute("expression", "");
            data_5 = new XAttribute("external", "0");

            objErrors = new Dictionary<string, List<string>>();
            error_messages = new string[]
            {
                Resource.GetString(Resource.VARIABLES_MD, "message_1", 0),
                Resource.GetString(Resource.VARIABLES_MD, "message_2", 0)
            };

            pattern = new char[] {
                '`', '~', '!', '@', '#', '%', '^',
                '&', '*', '(', ')', '-', '+', '=',
                '[', ']', '{', '}', '|', ';', ':',
                '.', ',', '<', '>', '/', '?', ' ',
                '\\', '\'', '"'
            };
        }

        #region internal methods
        /// <summary>
        /// Load data.
        /// </summary>
        /// <param name="element"></param>
        internal void LoadData(XElement element)
        {
            foreach (var a in element.Attributes())
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
                        External = a.Value == "1";
                        data_5 = a;
                        break;
                }
            }

            Data.Add(data_0, data_1, data_2, data_3, data_4, data_5);
        }

        /// <summary>
        /// Initialize data.
        /// </summary>
        /// <param name="action"></param>
        internal void InitData(VariableAction action)
        {
            if (Data.HasElements)
                return;

            switch (action)
            {
                case VariableAction.Add   : data_0.Value = "0"; break;
                case VariableAction.Edit  : data_0.Value = "1"; break;
                case VariableAction.Rename: data_0.Value = "2"; break;
                case VariableAction.Remove: data_0.Value = "3"; break;
            }

            Data.Add(data_0, data_1, data_2, data_3, data_4, data_5);
        }
        #endregion

        #region public properties
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

        internal XElement Data { get; private set; }

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
        /// <param name="error">Error message.</param>
        private void AddError(string name, string error)
        {
            if (objErrors.TryGetValue(name, out List<string> errors) == false)
            {
                errors = new List<string>();
                objErrors.Add(name, errors);
            }

            if (errors.Contains(error) == false)
            {
                errors.Add(error);
            }

            OnRaiseErrorChanged(name);
        }

        /// <summary>
        /// Remove error from dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="error">Error message.</param>
        private void RemoveError(string name, string error)
        {
            if (objErrors.TryGetValue(name, out List<string> errors))
            {
                errors.Remove(error);
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
                                RemoveError(name, error_messages[0]);
                                break;
                            }
                            else
                            {
                                RemoveError(name, error_messages[1]);
                                break;
                            }
                        }
                    }
                    break;
                case 0:
                    if (isDigit)
                        AddError(name, error_messages[0]);
                    else if (HasErrors)
                        RemoveError(name, error_messages[0]);
                    break;
                case 1:
                    if (!value.IsValid(pattern))
                        AddError(name, error_messages[1]);
                    else if (HasErrors)
                        RemoveError(name, error_messages[1]);
                    break;
            }
        }
        #endregion
    }
}