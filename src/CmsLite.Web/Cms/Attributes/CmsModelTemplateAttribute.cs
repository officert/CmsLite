using System;

namespace CmsLite.Web.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CmsModelTemplateAttribute : Attribute
    {
        public Type[] AllowedChildModelTypes { get; set; }
    }
}
