using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;

namespace CmsLite.Core.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets all the controllers in an assembly that subclass CmsLite.Core.Controllers.CmsBaseController, and have the CmsLite.Core.Attributes.CmsSectionTemplateAttribute attribute.
        /// </summary>
        public static IEnumerable<Type> GetControllers(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(CmsBaseController)) && 
                    t.GetCustomAttributes(typeof(CmsSectionTemplateAttribute), false).Length > 0);
        }

        public static IEnumerable<Type> GetModels(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(CmsModelTemplateAttribute), false).Length > 0);
        }
    }
}