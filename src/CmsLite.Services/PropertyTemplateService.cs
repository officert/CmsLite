using System;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;

namespace CmsLite.Services
{
    public class PropertyTemplateService : IPropertyTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPropertyService _propertyService;

        public PropertyTemplateService(IUnitOfWork unitOfWork, IPropertyService propertyService)
        {
            _unitOfWork = unitOfWork;
            _propertyService = propertyService;
        }

        public PropertyTemplate Create(PageTemplate pageTemplate, string propertyName, CmsPropertyType propertyType, int? tabOrder, string tabName = "", bool required = false, string description = "", string displayName = "", bool commit = true)
        {
            if (propertyName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PropertyTemplatePropertyNameCannotBeNull);

            if (pageTemplate == null)
                throw new ArgumentException(Messages.PageTemplateCannotBeNull);

            return CreatePropertyTemplate(pageTemplate, propertyName, propertyType, tabOrder, tabName, required, description, displayName, commit);
        }

        public PropertyTemplate Create(int pageTemplateId, string propertyName, CmsPropertyType propertyType, int? tabOrder, string tabName = "", bool required = false, string description = "", string displayName = "", bool commit = true)
        {
            if (propertyName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PropertyTemplatePropertyNameCannotBeNull);

            var pageTemplate = _unitOfWork.Context.GetDbSet<PageTemplate>()
                        .Include(x => x.PageNodes.Select(y => y.Properties.Select(z => z.PropertyTemplate)))
                        .FirstOrDefault(x => x.Id == pageTemplateId);

            if (pageTemplate == null)
                throw new ArgumentException(string.Format(Messages.PageTemplateNotFound, pageTemplateId));

            return CreatePropertyTemplate(pageTemplate, propertyName, propertyType, tabOrder, tabName, required, description, displayName, commit);
        }

        public void Delete(int id, bool commit)
        {
            var propertyTemplateDbSet = _unitOfWork.Context.GetDbSet<PropertyTemplate>();
            var propertyTemplate = propertyTemplateDbSet.FirstOrDefault(x => x.Id == id);

            if(propertyTemplate == null)
                throw new ArgumentException(string.Format(Messages.PropertyTemplateNotFound, id));

            propertyTemplateDbSet.Remove(propertyTemplate);

            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        #region Private Helpers

        private PropertyTemplate CreatePropertyTemplate(PageTemplate pageTemplate, string propertyName, CmsPropertyType propertyType, int? tabOrder, string tabName = "", bool required = false, string description = "", string displayName = "", bool commit = true)
        {
            var propertyTemplateDbSet = _unitOfWork.Context.GetDbSet<PropertyTemplate>();

            var propertyTemplate = propertyTemplateDbSet.Create();

            propertyTemplate.ParentPageTemplate = pageTemplate;
            propertyTemplate.PropertyName = propertyName;
            propertyTemplate.CmsPropertyType = propertyType.ToString();
            propertyTemplate.DisplayName = !displayName.IsNullOrEmpty() ? displayName : propertyName;
            propertyTemplate.TabName = !tabName.IsNullOrEmpty() ? tabName : CmsConstants.PropertyTemplateDefaultTabName;
            propertyTemplate.Required = required;
            propertyTemplate.Description = description;

            if (pageTemplate.PageNodes != null && pageTemplate.PageNodes.Any())
            {
                CreateNewPropertiesForPageNodesThatUsePageTemplate(pageTemplate, propertyTemplate);
            }

            propertyTemplateDbSet.Add(propertyTemplate);

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return propertyTemplate;
        }

        private void CreateNewPropertiesForPageNodesThatUsePageTemplate(PageTemplate pageTemplate, PropertyTemplate propertyTemplate)
        {
            if(pageTemplate.PageNodes == null)
                return;

            foreach (var pageNode in pageTemplate.PageNodes)
            {
                _propertyService.Create(pageNode, propertyTemplate, "", commit: false);
            }
        }

        #endregion
    }
}
