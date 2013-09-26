using System.Reflection;

namespace CmsLite.Interfaces.Templating
{
    public interface ITemplateEngine
    {
        void GenerateTemplates(Assembly assembly);
    }
}
