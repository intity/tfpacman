using System;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Document translator class.
    /// </summary>
    [Serializable]
    public class Translator_0 : Variables
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator_0()
        {
            IExtension = ".grb";
            OExtension = ".grb";
        }

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Document;
        internal override ProcessingMode PMode => ProcessingMode.SaveAs;
        #endregion
    }
}