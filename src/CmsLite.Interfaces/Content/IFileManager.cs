using System;
using System.Reflection;

namespace CmsLite.Interfaces.Content
{
    public interface IFileManager
    {
        void ProcessMvcFiles(Assembly assembly);

        Type GetModelType(Assembly assembly, string modelTypeName);
    }
}
