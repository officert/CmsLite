using System;

namespace CmsLite.Web.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CmsSectionTemplateAttribute : Attribute
    {
        public string Name { get; set; }

        public string IconImageName { get; set; }
    }
}
