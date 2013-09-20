using System;
using System.Reflection;

namespace CmsLite.Interfaces.Templating
{
    public interface ITemplateEngine
    {
        void ProcessMvcFiles(Assembly assembly);

        Type GetModelType(Assembly assembly, string modelTypeName);
    }
}
