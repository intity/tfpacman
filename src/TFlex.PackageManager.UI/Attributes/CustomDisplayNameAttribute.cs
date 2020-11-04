using System;
using System.ComponentModel;
using TFlex.PackageManager.UI.Properties;

namespace TFlex.PackageManager.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    internal class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        readonly string displayName;

        /// <summary>
        /// The CustomDisplayNameAttribute constructor.
        /// </summary>
        /// <param name="baseName">Base resource name.</param>
        /// <param name="displayName">Resource name.</param>
        public CustomDisplayNameAttribute(string baseName, string displayName) : base(displayName)
        {
            this.displayName = Resources.GetString(baseName, displayName)[0];
        }

        public override string DisplayName => displayName;
    }
}