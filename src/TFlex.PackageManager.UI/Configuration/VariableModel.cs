using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TFlex.PackageManager.Configuration
{
    public enum VariableAction
    {
        Add,
        Edit,
        Rename,
        Remove
    }

    public class VariableModel
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

        public VariableModel()
        {
            Data = new XElement("variable");

            data_0 = new XAttribute("action", "0");
            data_1 = new XAttribute("name", "");
            data_2 = new XAttribute("oldname", "");
            data_3 = new XAttribute("group", "");
            data_4 = new XAttribute("expression", "");
            data_5 = new XAttribute("external", "0");
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
                }
            }
        }
        #endregion

        internal XElement Data { get; private set; }
    }
}
