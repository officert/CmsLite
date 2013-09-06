using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IPropertyService : IServiceBase<Property>
    {
        Property Create(PageNode pageNode, PropertyTemplate propertyTemplate, string text = null, bool commit = true);
        Property Create(int pageNodeId, int propertyTemplateId, string text = null, bool commit = true);
        void Delete(int id, bool commit = true);
    }
}
