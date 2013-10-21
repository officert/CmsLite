using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface ISectionNodeService
    {
        SectionNode GetById(int id);

        SectionNode GetByUrlName(string urlName);

        IEnumerable<SectionNode> GetAllWithDetails();

        IEnumerable<SectionNode> GetAll(bool includeTrashed = false);

        IEnumerable<SectionNode> GetAllTrashed();

        SectionNode Create(int sectionTemplateId, string displayName, string urlName, bool commit = true);

        void Trash(int id, bool commit = false);

        void Delete(int id, bool commit = true);
    }
}
