using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IPagePropertyService
    {

        PageProperty Create(PageNode pageNode, PagePropertyTemplate propertyTemplate, string text = null, bool commit = true);

        PageProperty Create(int pageNodeId, int propertyTemplateId, string text = null, bool commit = true);

        void Delete(int id, bool commit = true);
    }
}
