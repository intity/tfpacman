using System;
using System.ComponentModel;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    internal class CustomDescriptionAttribute : DescriptionAttribute
    {
        private readonly string description;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="baseName">Base resource name.</param>
        /// <param name="name">Resource name.</param>
        public CustomDescriptionAttribute(string baseName, string name) : base(name)
        {
            description = Resource.GetString(baseName, name, 1);
        }

        public override string Description
        {
            get { return description; }
        }
    }
}