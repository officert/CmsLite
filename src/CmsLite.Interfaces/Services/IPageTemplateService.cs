using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IPageTemplateService : IServiceBase<PageTemplate>
    {
        PageTemplate Find(int sectionTemplateId, int pageTemplateId);
        
        PageTemplate CreateForSectionTemplate(SectionTemplate sectionTemplate, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true);
        PageTemplate CreateForSectionTemplate(int sectionTemplateId, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true);
        PageTemplate CreateForPageTemplate(int pageTemplateId, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true);
        
        PageTemplate Update(PageTemplate pageTemplate, string modelName, string name, string iconImageName = null, bool commit = true);
        
        void Delete(PageTemplate pageTemplate, bool commit = true);
        void Delete(int id, bool commit = true);
    }
}
