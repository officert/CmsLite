using System;

namespace CmsLite.Core.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CmsPageTemplateAttribute : Attribute
    {
        public string Name { get; set; }
        public Type ModelType { get; set; }

        /// <summary>
        /// An array of Action names representing the PageTemplates that you want to allow under this PageTemplate. The Action names must belong to the current controller.
        /// </summary>
        public string[] AllowedChildPageTemplates { get; set; }

        public string IconImageName { get; set; }
    }
}
