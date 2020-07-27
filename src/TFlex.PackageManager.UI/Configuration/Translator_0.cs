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
        /// <param name="ext">Target extension the file.</param>
        public Translator_0(string ext = "GRB") : base (ext) { }

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