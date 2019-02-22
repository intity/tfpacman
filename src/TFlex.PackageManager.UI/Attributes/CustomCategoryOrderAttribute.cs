using System;
using TFlex.PackageManager.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TFlex.PackageManager.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class CustomCategoryOrderAttribute : CategoryOrderAttribute
    {
        /// <summary>
        /// The CustomCategoryOrderAttribute constructor.
        /// </summary>
        /// <param name="baseName">Resource name.</param>
        /// <param name="order"></param>
        public CustomCategoryOrderAttribute(string baseName, int order) 
            : base(Resource.GetString(baseName, string.Format("category{0}", order), 0), order) { }
    }
}