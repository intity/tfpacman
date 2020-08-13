using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Variable collection.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "parameter")]
    public class VariableCollection : ObservableCollection<VariableModel>, IXmlSerializable, ICloneable
    {
        public VariableCollection() { }
        public VariableCollection(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Variable collection name.
        /// </summary>
        public string Name { get; }

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.ReadToDescendant("variable"))
                return;
            do
            {
                var obj = new XmlSerializer(typeof(VariableModel)).Deserialize(reader);
                if (obj != null)
                {
                    var variable = obj as VariableModel;
                    variable.PropertyChanged += Variable_PropertyChanged;
                    Add(variable);
                }
            } while (reader.ReadToNextSibling("variable"));
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var i in this)
            {
                i.WriteXml(writer);
                i.PropertyChanged += Variable_PropertyChanged;
            }
        }
        #endregion

        #region ICloneable Members
        public object Clone()
        {
            var variables = new VariableCollection(Name);
            foreach (var i in this)
            {
                variables.Add((VariableModel)i.Clone());
            }
            return variables;
        }
        #endregion

        private void Variable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "EndEdit")
                return;

            OnPropertyChanged(e);
        }
    }
}