using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
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
    public class Translator : INotifyPropertyChanged
    {
        #region private fields
        XElement data;
        TranslatorType type;
        ProcessingMode mode;
        #endregion

        #region internal properties
        /// <summary>
        /// Translator type.
        /// </summary>
        internal virtual TranslatorType TMode
        {
            get => type;
            private set
            {
                type = value;
            }
        }

        /// <summary>
        /// Processing mode.
        /// </summary>
        internal virtual ProcessingMode PMode
        {
            get => mode;
            set
            {
                if (mode != value)
                {
                    mode = value;
                    data.Attribute("mode").Value = ((int)value).ToString();
                }
            }
        }
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

        /// <summary>
        /// The method to loaded translator.
        /// </summary>
        /// <param name="data">The translator element.</param>
        internal void LoadTranslator(XElement data)
        {
            this.data = data;

            type = (TranslatorType)int.Parse(data.Attribute("type").Value);
            mode = (ProcessingMode)int.Parse(data.Attribute("mode").Value);

            foreach (var i in data.Elements())
            {
                LoadParameter(i);
            }
        }

        /// <summary>
        /// Create XML-data for new translator.
        /// </summary>
        /// <returns></returns>
        internal virtual XElement NewTranslator()
        {
            data = new XElement("translator");
            data.Add(new XAttribute("type", (int)TMode));
            data.Add(new XAttribute("mode", (int)PMode));

            return data;
        }

        /// <summary>
        /// Virtual method for processing translator parameters.
        /// </summary>
        /// <param name="element">Parent element.</param>
        internal virtual void LoadParameter(XElement element) { }
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