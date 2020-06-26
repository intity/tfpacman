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
        Bmf,
        Emf,
        Iges,
        Jt,
        Parasolid,
        Pdf,
        Step,
        Stl
    }

    /// <summary>
    /// Processing type enumeration.
    /// </summary>
    public enum ProcessingMode : uint
    {
        SaveAs = 0x0000,
        Export = 0x0001,
        Import = 0x0002
    }

    /// <summary>
    /// The translator base class.
    /// </summary>
    public class Translator : INotifyPropertyChanged
    {
        #region internal properties
        /// <summary>
        /// The XML-metadata to translator.
        /// </summary>
        internal XElement Data { get; private set; }

        /// <summary>
        /// Translator type.
        /// </summary>
        internal virtual TranslatorType TMode { get; }

        /// <summary>
        /// Processing mode.
        /// </summary>
        internal virtual ProcessingMode PMode { get; }
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
            foreach (var i in data.Elements())
            {
                LoadParameter(i);
            }
        }

        internal virtual XElement NewTranslator()
        {
            return Data = new XElement("translator", 
                new XAttribute("id", TMode), 
                new XAttribute("processing", (uint)PMode));
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