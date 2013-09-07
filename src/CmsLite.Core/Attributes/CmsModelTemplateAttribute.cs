using System;

namespace CmsLite.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CmsModelTemplateAttribute : Attribute
    {
        public Type[] AllowedChildModelTypes { get; set; }
    }
}
