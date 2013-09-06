using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface ISectionNodeService : IServiceBase<SectionNode>
    {
        IEnumerable<SectionNode> GetAllWithDetails();
        SectionNode Create(int sectionTemplateId, string displayName, string urlName, bool commit = true);
        void Delete(int id, bool commit = true);
    }
}
