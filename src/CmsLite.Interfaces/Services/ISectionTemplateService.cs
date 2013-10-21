using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface ISectionTemplateService
    {
        SectionTemplate GetById(int id);

        IEnumerable<SectionTemplate> GetAllSectionTemplates();

        SectionTemplate Create(string controllerName, string name = null, string iconImageName = null, bool commit = true);

        SectionTemplate Update(SectionTemplate sectionTemplate, string name, string iconImageName = null, bool commit = true);

        SectionTemplate Update(int id, string name, string iconImageName = null, bool commit = true);

        void Delete(int id, bool commit = true);
    }
}