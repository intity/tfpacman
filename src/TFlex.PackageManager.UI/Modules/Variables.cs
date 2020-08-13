using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// Variables extension module.
    /// </summary>
    [CustomCategoryOrder(Resources.VARIABLES, 3), Serializable]
    public class Variables : Projections
    {
        public Variables()
        {
            AddVariables    = new VariableCollection("AddVariables");
            EditVariables   = new VariableCollection("EditVariables");
            RenameVariables = new VariableCollection("RenameVariables");
            RemoveVariables = new VariableCollection("RemoveVariables");

            AddVariables.CollectionChanged    += Variables_CollectionChanged;
            EditVariables.CollectionChanged   += Variables_CollectionChanged;
            RenameVariables.CollectionChanged += Variables_CollectionChanged;
            RemoveVariables.CollectionChanged += Variables_CollectionChanged;
        }

        private void Variables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Variables");
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

        #region IXmlSerializable Members
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            for (int i = 0; i < 4 && reader.Read(); i++)
            {
                switch (reader.GetAttribute(0))
                {
                    case "AddVariables":
                        AddVariables.ReadXml(reader);
                        break;
                    case "EditVariables":
                        EditVariables.ReadXml(reader);
                        break;
                    case "RenameVariables":
                        RenameVariables.ReadXml(reader);
                        break;
                    case "RemoveVariables":
                        RemoveVariables.ReadXml(reader);
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "AddVariables");
            AddVariables.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "EditVariables");
            EditVariables.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "RenameVariables");
            RenameVariables.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("parameter");
            writer.WriteAttributeString("name", "RemoveVariables");
            RemoveVariables.WriteXml(writer);
            writer.WriteEndElement();
        }
        #endregion

        #region internal methods
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
