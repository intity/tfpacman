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
    public class VariableCollection : ObservableCollection<VariableModel>, ICloneable
    {
        readonly string name;

        public VariableCollection(string name)
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
            Data = element;

            foreach (var e in element.Elements())
            {
                Add(new VariableModel(e));
            }
        }

        /// <summary>
        /// Clone object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var variables = new VariableCollection(name);
            variables.LoadData(new XElement(Data));
            return variables;
        }
        #endregion

        public XElement Data { get; private set; }

        #region Collection Members
        protected override void InsertItem(int index, VariableModel item)
        {
            if (Data.Elements().Contains(item.Data) == false)
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