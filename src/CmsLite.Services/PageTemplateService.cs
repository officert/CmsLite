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
    public class PageTemplateService : IPageTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPageNodeService _pageNodeService;
        private readonly IPagePropertyTemplateService _propertyTemplateService;

        public PageTemplateService(IUnitOfWork unitOfWork,IPageNodeService pageNodeService, IPagePropertyTemplateService propertyTemplateService)
        {
            _unitOfWork = unitOfWork;
            _pageNodeService = pageNodeService;
            _propertyTemplateService = propertyTemplateService;
        }

        public PageTemplate GetById(int id)
        {
            return _unitOfWork.Context.GetDbSet<PageTemplate>().FirstOrDefault(x => x.Id == id);
        }

        public PageTemplate Find(int sectionTemplateId, int pageTemplateId)
        {
            var sectionTemplate = _unitOfWork.Context.GetDbSet<SectionTemplate>()
                            .Include(x => x.PageTemplates)
                            .FirstOrDefault(x => x.Id == sectionTemplateId);

            if (sectionTemplate == null)
                throw new ArgumentException(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId));

            return sectionTemplate.PageTemplates == null
                ? null
                : sectionTemplate.PageTemplates.FirstOrDefault(x => x.Id == pageTemplateId);
        }

        public PageTemplate CreateForSectionTemplate(SectionTemplate sectionTemplate, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true)
        {
            if (actionName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateActionNameCannotBeNull);

            if (modelName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateModelNameCannotBeNull);

            if (sectionTemplate == null)
                throw new ArgumentException(Messages.SectionTemplateCannotBeNull);

            return CreatePageTemplateForSectionTemplate(sectionTemplate, actionName, modelName, name, iconImageName, commit);
        }

        public PageTemplate CreateForSectionTemplate(int sectionTemplateId, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true)
        {
            if (actionName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateActionNameCannotBeNull);

            if (modelName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateModelNameCannotBeNull);

            var sectionTemplate = _unitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplateId);

            if (sectionTemplate == null)
                throw new ArgumentException(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId));

            return CreatePageTemplateForSectionTemplate(sectionTemplate, actionName, modelName, name, iconImageName, commit);
        }

        public PageTemplate CreateForPageTemplate(int pageTemplateId, string actionName, string modelName, string name = null, string iconImageName = null, bool commit = true)
        {
            if (actionName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateActionNameCannotBeNull);

            if (modelName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageTemplateModelNameCannotBeNull);

            var parentPageTemplate = GetById(pageTemplateId);

            if (parentPageTemplate == null)
                throw new ArgumentException(string.Format(Messages.PageTemplateNotFound, pageTemplateId));

            var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();

            var pageTemplate = pageTemplateDbSet.Create();
            pageTemplate.ParentPageTemplate = parentPageTemplate;
            pageTemplate.ActionName = actionName;
            pageTemplate.Name = name.IsNullOrEmpty()
                            ? actionName
                            : name;
            pageTemplate.IconImageName = iconImageName.IsNullOrEmpty()
                            ? CmsConstants.PageTemplateDefaultThumbnail
                            : iconImageName;
            pageTemplate.ModelName = modelName;

            pageTemplateDbSet.Add(pageTemplate);

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return pageTemplate;
        }

        public PageTemplate Update(PageTemplate pageTemplate, string modelName, string name = null, string iconImageName = null, bool commit = true)
        {
            pageTemplate = _unitOfWork.Context.GetDbSet<PageTemplate>()
                            .Include(x => x.PropertyTemplates)
                            .FirstOrDefault(x => x.Id == pageTemplate.Id);

            if (pageTemplate == null)
                throw new ArgumentException(Messages.PageTemplateCannotBeNull);

            pageTemplate.Name = !name.IsNullOrEmpty()
                                ? name
                                : pageTemplate.Name;
            pageTemplate.IconImageName = !iconImageName.IsNullOrEmpty()
                                ? iconImageName
                                : pageTemplate.IconImageName;

            if (pageTemplate.ModelName != modelName)
            {
                RemovePropertyTemplateFromPageTemplate(pageTemplate);
            }

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return pageTemplate;
        }

        public void Delete(PageTemplate pageTemplate, bool commit = true)
        {
            if (pageTemplate == null)
                throw new ArgumentException(Messages.PageTemplateCannotBeNull);

            DeletePageTemplate(pageTemplate, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();
            var pageTemplates = pageTemplateDbSet
                                .Include(x => x.PageTemplates)
                                .Include(x => x.PageNodes);
            var pageTemplate = pageTemplates.FirstOrDefault(x => x.Id == id);

            if (pageTemplate == null)
                throw new ArgumentException(string.Format(Messages.PageTemplateNotFound, id));

            DeletePageTemplate(pageTemplate, commit);
        }

        #region Private Helpers

        private PageTemplate CreatePageTemplateForSectionTemplate(SectionTemplate sectionTemplate, string actionName, string modelName, string name = "", string iconImageName = "", bool commit = true)
        {
            var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();

            var pageTemplate = pageTemplateDbSet.Create();
            pageTemplate.ParentSectionTemplate = sectionTemplate;
            pageTemplate.ActionName = actionName;
            pageTemplate.Name = name.IsNullOrEmpty()
                            ? actionName
                            : name;
            pageTemplate.IconImageName = iconImageName.IsNullOrEmpty()
                            ? CmsConstants.PageTemplateDefaultThumbnail
                            : iconImageName;
            pageTemplate.ModelName = modelName;

            pageTemplateDbSet.Add(pageTemplate);

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return pageTemplate;
        }

        private void DeletePageTemplate(PageTemplate pageTemplate, bool commit = true)
        {
            var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();

            if (pageTemplate.PageNodes != null && pageTemplate.PageNodes.Any())
            {
                foreach (var pageNode in pageTemplate.PageNodes.ToList())
                {
                    _pageNodeService.Delete(pageNode, false);
                }
            }

            if (pageTemplate.PageTemplates != null && pageTemplate.PageTemplates.Any())
            {
                foreach (var childPageTemplate in pageTemplate.PageTemplates.ToList())
                {
                    Delete(childPageTemplate, false);
                }
            }

            pageTemplateDbSet.Remove(pageTemplate);

            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        private void RemovePropertyTemplateFromPageTemplate(PageTemplate pageTemplate)
        {
            if (pageTemplate.PropertyTemplates != null && pageTemplate.PropertyTemplates.Any())
            {
                foreach (var propertyTemplate in pageTemplate.PropertyTemplates.ToList())
                {
                    _propertyTemplateService.Delete(propertyTemplate.Id, false);
                }
            }
        }

        #endregion
    }
}
