using System;
using CmsLite.Utilities.Cms;

namespace CmsLite.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CmsModelPropertyAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public CmsPropertyType PropertyType { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public string TabName { get; set; }

        public int TabOrder { get; set; }
    }
}
