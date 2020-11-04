using System;
using System.ComponentModel;
using TFlex.PackageManager.UI.Properties;

namespace TFlex.PackageManager.UI.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    internal class CustomDescriptionAttribute : DescriptionAttribute
    {
        readonly string description;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="baseName">Base resource name.</param>
        /// <param name="name">Resource name.</param>
        public CustomDescriptionAttribute(string baseName, string name) : base(name)
        {
            description = Resources.GetString(baseName, name)[1];
        }

        public override string Description => description;
    }
}