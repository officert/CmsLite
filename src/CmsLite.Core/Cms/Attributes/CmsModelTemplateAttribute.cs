using System;

namespace CmsLite.Core.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CmsModelTemplateAttribute : Attribute
    {
        public Type[] AllowedChildModelTypes { get; set; }
    }
}
