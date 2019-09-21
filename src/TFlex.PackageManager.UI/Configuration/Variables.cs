using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Variable collection.
    /// </summary>
    public class Variables : ObservableCollection<VariableModel>, ICloneable
    {
        readonly string name;

        public Variables(string name)
        {
            this.name = name;
            Data = new XElement("parameter", new XAttribute("name", name));
        }

        #region methods
        /// <summary>
        /// Load the variables to collection.
        /// </summary>
        /// <param name="element">Parent element.</param>
        internal void LoadData(XElement element)
        {
            foreach (var e in element.Elements())
            {
                Add(new VariableModel
                {
                    Action     = int.Parse(e.Attribute("action").Value),
                    Name       = e.Attribute("name").Value,
                    OldName    = e.Attribute("oldname").Value,
                    Group      = e.Attribute("group").Value,
                    Expression = e.Attribute("expression").Value,
                    External   = e.Attribute("external").Value == "1"
                });
            }

            Data = element;
        }

        /// <summary>
        /// Clone object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var variables = new Variables(name);
            foreach (var e in Data.Elements())
            {
                variables.Add(new VariableModel
                {
                    Action     = int.Parse(e.Attribute("action").Value),
                    Name       = e.Attribute("name").Value,
                    OldName    = e.Attribute("oldname").Value,
                    Group      = e.Attribute("group").Value,
                    Expression = e.Attribute("expression").Value,
                    External   = e.Attribute("external").Value == "1"
                });
            }

            return variables;
        }
        #endregion

        public XElement Data { get; private set; }

        #region Collection Members
        protected override void InsertItem(int index, VariableModel item)
        {
            Data.Add(item.Data);
            base.InsertItem(index, item);
            //Debug.WriteLine(string.Format("InsertItem: [index: {0}]", index));
        }

        protected override void RemoveItem(int index)
        {
            Data.Elements().ElementAt(index).Remove();
            base.RemoveItem(index);
            //Debug.WriteLine(string.Format("RemoveItem: [index: {0}]", index));
        }

        protected override void ClearItems()
        {
            Data.Elements().Remove();
            base.ClearItems();
        }
        #endregion
    }
}