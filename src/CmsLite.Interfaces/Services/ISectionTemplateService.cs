using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface ISectionTemplateService : IServiceBase<SectionTemplate>
    {
        SectionTemplate Create(string controllerName, string name = "", string iconImageName = "", bool commit = true);
        void Delete(int id, bool commit = true);
    }
}