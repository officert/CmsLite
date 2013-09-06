using CmsLite.Domains.Entities;
using CmsLite.Utilities.Cms;

namespace CmsLite.Interfaces.Services
{
    public interface IPropertyTemplateService : IServiceBase<PropertyTemplate>
    {
        PropertyTemplate Create(PageTemplate pageTemplate,
                                    string propertyName,
                                    CmsPropertyType propertyType,
                                    int? tabOrder,
                                    string tabName = "",
                                    bool required = false,
                                    string description = "",
                                    string displayName = "",
                                    bool commit = true);
        PropertyTemplate Create(int pageTemplateId,
                                    string propertyName, 
                                    CmsPropertyType propertyType, 
                                    int? tabOrder, 
                                    string tabName = "", 
                                    bool required = false, 
                                    string description = "", 
                                    string displayName = "",
                                    bool commit = true);

        void Delete(int id, bool commit = true);
    }
}
