using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using TFlex.Model;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The translator base class.
    /// </summary>
    public class Translator : INotifyPropertyChanged
    {
        #region private fields
        private bool isLoaded;
        #endregion

        #region internal properties
        /// <summary>
        /// The translator configuration is changed.
        /// </summary>
        internal virtual bool IsChanged { get; }

        /// <summary>
        /// The translator configuration is loaded.
        /// </summary>
        internal bool IsLoaded
        {
            get { return (isLoaded); }
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Cloneable values this object on loaded.
        /// </summary>
        internal virtual void OnLoaded()
        {
            isLoaded = true;
        }

        /// <summary>
        /// The OnChanged event handler.
        /// </summary>
        /// <param name="index">Property index.</param>
        internal virtual void OnChanged(int index)
        {
            OnPropertyChanged("IsChanged");
        }

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        /// <param name="logFile"></param>
        internal virtual void Export(Document document, string path, LogFile logFile) { }

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pages"></param>
        /// <param name="logFile"></param>
        internal virtual void Export(Document document, Dictionary<Page, string> pages, LogFile logFile) { }

        /// <summary>
        /// Configuration task method.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write
        /// </param>
        internal void ConfigurationTask(XElement element, int flag)
        {
            isLoaded = false;

            foreach (var i in element.Elements())
            {
                TranslatorTask(i, flag);
            }

            OnLoaded();
        }

        /// <summary>
        /// Create new translator metadata to Xml.
        /// </summary>
        /// <param name="translator">Translator type.</param>
        /// <returns></returns>
        internal virtual XElement NewTranslator(TranslatorType translator)
        {
            return null;
        }

        /// <summary>
        /// Append translator metadata to parent element.
        /// </summary>
        /// <param name="parent">Parent element from metadata Xml.</param>
        /// <param name="translator">Translator type.</param>
        internal virtual void AppendTranslatorToXml(XElement parent, TranslatorType translator)
        {
            parent.Add(NewTranslator(translator));
        }

        /// <summary>
        /// Virtual method for processing translator parameters.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="flag">
        /// Flag definition: (0) - read, (1) - write.
        /// </param>
        internal virtual void TranslatorTask(XElement element, int flag) { }
        #endregion

        #region INotifyPropertyChanged members
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