using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Variables extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.VARIABLES, 3)]
    public class Variables : Files
    {
        public Variables()
        {
            AddVariables    = new VariableCollection("AddVariables");
            EditVariables   = new VariableCollection("EditVariables");
            RenameVariables = new VariableCollection("RenameVariables");
            RemoveVariables = new VariableCollection("RemoveVariables");
        }

        #region public properties
        /// <summary>
        /// Add variables.
        /// </summary>
        [PropertyOrder(9)]
        [CustomCategory(Resources.VARIABLES, "category3")]
        [CustomDisplayName(Resources.VARIABLES, "dn3_1")]
        [CustomDescription(Resources.VARIABLES, "dn3_1")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection AddVariables { get; }

        /// <summary>
        /// Edit variables.
        /// </summary>
        [PropertyOrder(10)]
        [CustomCategory(Resources.VARIABLES, "category3")]
        [CustomDisplayName(Resources.VARIABLES, "dn3_2")]
        [CustomDescription(Resources.VARIABLES, "dn3_2")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection EditVariables { get; }

        /// <summary>
        /// Rename variables.
        /// </summary>
        [PropertyOrder(11)]
        [CustomCategory(Resources.VARIABLES, "category3")]
        [CustomDisplayName(Resources.VARIABLES, "dn3_3")]
        [CustomDescription(Resources.VARIABLES, "dn3_3")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection RenameVariables { get; }

        /// <summary>
        /// Remove variables.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resources.VARIABLES, "category3")]
        [CustomDisplayName(Resources.VARIABLES, "dn3_4")]
        [CustomDescription(Resources.VARIABLES, "dn3_4")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection RemoveVariables { get; }
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();
            data.Add(AddVariables.Data);
            data.Add(EditVariables.Data);
            data.Add(RenameVariables.Data);
            data.Add(RemoveVariables.Data);
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);
            switch (element.Attribute("name").Value)
            {
                case "AddVariables":
                    AddVariables.LoadData(element);
                    break;
                case "EditVariables":
                    EditVariables.LoadData(element);
                    break;
                case "RenameVariables":
                    RenameVariables.LoadData(element);
                    break;
                case "RemoveVariables":
                    RemoveVariables.LoadData(element);
                    break;
            }
        }

        /// <summary>
        /// The VariablesCount helper method.
        /// </summary>
        /// <returns>Total Variables Count.</returns>
        internal int VariablesCount()
        {
            List<string> variables = new List<string>();

            foreach (var i in AddVariables)
            {
                variables.Add(i.Name);
            }

            foreach (var i in EditVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in RenameVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in RemoveVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            return variables.Count;
        }
        #endregion
    }
}
