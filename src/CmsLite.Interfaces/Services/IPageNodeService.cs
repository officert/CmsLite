using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IPageNodeService
    {
        PageNode GetById(int id);

        PageNode GetByIdWithDetails(int id);

        PageNode CreateForSection(int parentSectionId, int pageTemplateId, string displayName, string urlName, bool commit = true);

        PageNode CreateForPage(int parentPageId, int pageTemplateId, string displayName, string urlName);

        void Update(int pageId, IEnumerable<Property> properties);

        void Delete(PageNode pageNode, bool commit = true);

        void Delete(int id, bool commit = true);
    }
}
