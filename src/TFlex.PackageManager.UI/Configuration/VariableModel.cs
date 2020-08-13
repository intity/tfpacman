using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
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
    [Serializable, XmlRoot(ElementName = "variable")]
    public class VariableModel : IXmlSerializable, IEditableObject, INotifyPropertyChanged, ICloneable
    {
        #region private fields
        int action;
        string name;
        string oldname;
        string group;
        string expression;
        bool external;
        #endregion

        /// <summary>
        /// The default constructor.
        /// </summary>
        public VariableModel() { }

        #region public properties
        /// <summary>
        /// Action when processing a variable.
        /// </summary>
        public int Action
        {
            get => action;
            set
            {
                if (action != value)
                {
                    action = value;
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
                    OnPropertyChanged("External");
                }
            }
        }
        #endregion

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                switch (i)
                {
                    case 0:
                        action = int.Parse(reader.GetAttribute(i));
                        break;
                    case 1:
                        name = reader.GetAttribute(i);
                        break;
                    case 2:
                        oldname = reader.GetAttribute(i);
                        break;
                    case 3:
                        group = reader.GetAttribute(i);
                        break;
                    case 4:
                        expression = reader.GetAttribute(i);
                        break;
                    case 5:
                        external = reader.GetAttribute(i) == "1";
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("variable");
            writer.WriteAttributeString("action", Action.ToString());
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("oldname", OldName);
            writer.WriteAttributeString("group", Group);
            writer.WriteAttributeString("expression", Expression);
            writer.WriteAttributeString("external", External ? "1" : "0");
            writer.WriteEndElement();
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
            return new VariableModel
            {
                Action     = Action,
                Name       = Name,
                OldName    = OldName,
                Group      = Group,
                Expression = Expression,
                External   = External
            };
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