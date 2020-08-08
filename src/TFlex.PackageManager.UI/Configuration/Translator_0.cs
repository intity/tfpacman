using System.Xml.Linq;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The document translator class.
    /// </summary>
    public class Translator_0 : Links
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_0() { }

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Document;
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            PMode = ProcessingMode.SaveAs;
            return base.NewTranslator();
        }
        #endregion
    }
}