using System;
using System.ComponentModel;
using TFlex.PackageManager.Properties;

namespace TFlex.PackageManager.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal class CustomCategoryAttribute : CategoryAttribute
    {
        private readonly string baseName;

        public CustomCategoryAttribute(string baseName, string category) : base(category)
        {
            this.baseName = baseName;
        }

        protected override string GetLocalizedString(string value)
        {
            return Resources.GetString(baseName, value)[0];
        }
    }
}