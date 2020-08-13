using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TFlex.Model;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Translator type enumeration.
    /// </summary>
    public enum TranslatorType
    {
        Document,
        Acad,
        Acis,
        Bitmap,
        Iges,
        Jt,
        Pdf,
        Step
    }

    /// <summary>
    /// Processing type enumeration.
    /// </summary>
    public enum ProcessingMode
    {
        SaveAs,
        Export,
        Import
    }

    /// <summary>
    /// The translator base class.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "translator")]
    public class Translator : IXmlSerializable, INotifyDataErrorInfo, INotifyPropertyChanged
    {
        public Translator()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        #region internal properties
        /// <summary>
        /// Translator type.
        /// </summary>
        internal virtual TranslatorType TMode { get; private set; }

        /// <summary>
        /// Processing mode.
        /// </summary>
        internal virtual ProcessingMode PMode { get; set; }
        #endregion

        #region internal methods
        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        /// <param name="logging"></param>
        internal virtual void Export(Document document, string path, Logging logging) { }

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pages"></param>
        /// <param name="logging"></param>
        internal virtual void Export(Document document, Dictionary<Page, string> pages, Logging logging) { }

        /// <summary>
        /// The Import virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="path"></param>
        /// <param name="logging"></param>
        internal virtual void Import(Document document, string targetDirectory, string path, Logging logging) { }
        #endregion

        #region IXmlSerializable Members
        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            TMode = (TranslatorType)int.Parse(reader.GetAttribute("type"));
            PMode = (ProcessingMode)int.Parse(reader.GetAttribute("mode"));
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", ((int)TMode).ToString());
            writer.WriteAttributeString("mode", ((int)PMode).ToString());
        }
        #endregion

        #region INotifyDataErrorInfo Members
        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
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
        /// Error messages container.
        /// </summary>
        protected Dictionary<string, List<string>> Errors { get; }

        /// <summary>
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        [Browsable(false)]
        public bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Errors.Values;

            Errors.TryGetValue(name, out List<string> errors);
            return errors;
        }

        /// <summary>
        /// Add error to dictionary.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="error">Error message.</param>
        internal void AddError(string name, string error)
        {
            if (Errors.TryGetValue(name, out List<string> errors) == false)
            {
                errors = new List<string>();
                Errors.Add(name, errors);
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
        internal void RemoveError(string name, string error)
        {
            if (Errors.TryGetValue(name, out List<string> errors))
            {
                errors.Remove(error);
            }

            if (errors == null)
                return;

            if (errors.Count == 0)
            {
                Errors.Remove(name);
                OnRaiseErrorChanged(name);
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
    }
}