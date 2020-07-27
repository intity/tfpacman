using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Xml.Linq;
using TFlex.PackageManager.Attributes;
using TFlex.PackageManager.Editors;
using TFlex.PackageManager.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#pragma warning disable CA1707
#pragma warning disable CA1819

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The document translator class.
    /// </summary>
    [CustomCategoryOrder(Resources.TRANSLATOR_0, 3)]
    public class Translator_0 : Links
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ext">Target extension the file.</param>
        public Translator_0(string ext = "GRB") : base (ext)
        {

        }

        #region public properties
        /// <summary>
        /// Add variables.
        /// </summary>
        [PropertyOrder(9)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_1")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_1")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection AddVariables { get; } = new VariableCollection("AddVariables");

        /// <summary>
        /// Edit variables.
        /// </summary>
        [PropertyOrder(10)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_2")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_2")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection EditVariables { get; } = new VariableCollection("EditVariables");

        /// <summary>
        /// Rename variables.
        /// </summary>
        [PropertyOrder(11)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_3")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_3")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection RenameVariables { get; } = new VariableCollection("RenameVariables");

        /// <summary>
        /// Remove variables.
        /// </summary>
        [PropertyOrder(12)]
        [CustomCategory(Resources.TRANSLATOR_0, "category3")]
        [CustomDisplayName(Resources.TRANSLATOR_0, "dn3_4")]
        [CustomDescription(Resources.TRANSLATOR_0, "dn3_4")]
        [Editor(typeof(VariablesEditor), typeof(UITypeEditor))]
        public VariableCollection RemoveVariables { get; } = new VariableCollection("RemoveVariables");
        #endregion

        #region internal properties
        internal override TranslatorType TMode => TranslatorType.Document;
        #endregion

        #region internal methods
        internal override XElement NewTranslator()
        {
            XElement data = base.NewTranslator();

            data.Add(AddVariables.Data);
            data.Add(EditVariables.Data);
            data.Add(RenameVariables.Data);
            data.Add(RemoveVariables.Data);

            PMode = ProcessingMode.SaveAs;
            return data;
        }

        internal override void LoadParameter(XElement element)
        {
            base.LoadParameter(element);
            switch (element.Attribute("name").Value)
            {
                case "AddVariables":
                    AddVariables.LoadData(element);
                    break;
                case "EditVariables":
                    EditVariables.LoadData(element);
                    break;
                case "RenameVariables":
                    RenameVariables.LoadData(element);
                    break;
                case "RemoveVariables":
                    RemoveVariables.LoadData(element);
                    break;
            }
        }
        #endregion
    }
}