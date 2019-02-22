using System;
using System.ComponentModel;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    internal class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string displayName;

        /// <summary>
        /// The CustomDisplayNameAttribute constructor.
        /// </summary>
        /// <param name="baseName">Base resource name.</param>
        /// <param name="displayName">Resource name.</param>
        public CustomDisplayNameAttribute(string baseName, string displayName) : base(displayName)
        {
            this.displayName = Resource.GetString(baseName, displayName, 0);
        }

        public override string DisplayName
        {
            get { return displayName; }
        }
    }
}