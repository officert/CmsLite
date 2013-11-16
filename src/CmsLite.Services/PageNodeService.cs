using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services.Helpers;
using CmsLite.Utilities;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;

namespace CmsLite.Services
{
    public class PageNodeService : IPageNodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPropertyService _propertyService;

        public PageNodeService(IUnitOfWork unitOfWork, IPropertyService propertyService)
        {
            _unitOfWork = unitOfWork;
            _propertyService = propertyService;
        }

        public PageNode GetById(int id)
        {
            return _unitOfWork.Context.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == id);
        }

        public PageNode GetByIdWithDetails(int id)
        {
            return _unitOfWork.Context.GetDbSet<PageNode>()
                .Include(x => x.PageTemplate)
                .Include(x => x.Properties.Select(y => y.PropertyTemplate))
                .FirstOrDefault(x => x.Id == id);
        }

        public PageNode CreateForSection(int sectionId, int pageTemplateId, string displayName, string urlName, bool commit = true)
        {
            Ensure.ArgumentIsNotNullOrEmpty(displayName, "displayName");
            Ensure.ArgumentIsNotNullOrEmpty(urlName, "urlName");

            var pageNodeDbSet = _unitOfWork.Context.GetDbSet<PageNode>();

            var parentSectionNode = _unitOfWork.Context.GetDbSet<SectionNode>()
                        .Include(x => x.SectionTemplate.PageTemplates.Select(y => y.PropertyTemplates))
                        .Include(x => x.PageNodes)
                        .FirstOrDefault(x => x.Id == sectionId);        //TODO: should use service instead of UnitOfWork directly, but issues with Ninject circular dependencies

            if (parentSectionNode == null)
                throw new ArgumentException(string.Format(Messages.SectionNodeNotFound, sectionId));

            var formattedUrlName = CmsUrlHelper.FormatUrlName(!string.IsNullOrEmpty(urlName) ? urlName : displayName);

            if (parentSectionNode.PageNodes != null && parentSectionNode.PageNodes.Any(s => s.UrlName == formattedUrlName))
                throw new ArgumentException(string.Format(Messages.PageNodeUrlNameMustBeUniqueWithinSection, formattedUrlName, sectionId));

            var pageTemplate = parentSectionNode.SectionTemplate.PageTemplates.FirstOrDefault(x => x.Id == pageTemplateId);         //TODO: should use service instead of UnitOfWork directly, but issues with Ninject circular dependencies

            if (pageTemplate == null)
                throw new ArgumentException(string.Format(Messages.PageTemplateNotFoundForSectionTemplate, pageTemplateId, parentSectionNode.SectionTemplateId));

            var pageNode = pageNodeDbSet.Create();
            pageNode.ParentSectionNode = parentSectionNode;
            pageNode.PageTemplate = pageTemplate;
            pageNode.DisplayName = displayName;
            pageNode.UrlName = formattedUrlName;
            pageNode.Order = parentSectionNode.PageNodes == null || !parentSectionNode.PageNodes.Any()
                                ? CmsConstants.FirstOrderNumber
                                : parentSectionNode.PageNodes.Count();
            pageNode.IsPublished = false;
            pageNode.CreatedOn = DateTime.UtcNow;
            pageNode.ModifiedOn = pageNode.CreatedOn;

            pageNodeDbSet.Add(pageNode);

            AddPropertiesForPageTemplatesPropertyTemplates(pageTemplate, pageNode);

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return pageNode;
        }

        public PageNode CreateForPage(int pageId, int pageTemplateId, string displayName, string urlName)
        {
            Ensure.ArgumentIsNotNullOrEmpty(displayName, "displayName");
            Ensure.ArgumentIsNotNullOrEmpty(urlName, "urlName");

            var pageNodeDbSet = _unitOfWork.Context.GetDbSet<PageNode>();
            var pageNodes = pageNodeDbSet
                            .Include(x => x.PageNodes)
                            .Include(x => x.PageTemplate.PageTemplates);

            var parentPage = pageNodes.FirstOrDefault(x => x.Id == pageId);

            if (parentPage == null)
                throw new ArgumentException(string.Format(Messages.PageNodeNotFound, pageId));

            var pageTemplate = parentPage.PageTemplate.PageTemplates.FirstOrDefault(x => x.Id == pageTemplateId);

            if (pageTemplate == null)
                throw new ArgumentException(string.Format(Messages.PageTemplateNotFoundForPageTemplate, pageTemplateId, parentPage.PageTemplateId));

            var formattedUrlName = CmsUrlHelper.FormatUrlName(!string.IsNullOrEmpty(urlName) ? urlName : displayName);

            if (parentPage.PageNodes != null && parentPage.PageNodes.Any(s => s.UrlName == formattedUrlName))
                throw new ArgumentException(string.Format(Messages.PageNodeUrlNameMustBeUniqueWithinPage, formattedUrlName, pageId));

            var pageNode = pageNodeDbSet.Create();

            pageNode.ParentPageNode = parentPage;
            pageNode.PageTemplate = pageTemplate;
            pageNode.DisplayName = displayName;
            pageNode.UrlName = formattedUrlName;
            pageNode.Order = parentPage.PageNodes == null || !parentPage.PageNodes.Any()
                                ? CmsConstants.FirstOrderNumber
                                : parentPage.PageNodes.Count();
            pageNode.IsPublished = false;
            pageNode.CreatedOn = DateTime.UtcNow;
            pageNode.ModifiedOn = pageNode.CreatedOn;

            pageNodeDbSet.Add(pageNode);

            AddPropertiesForPageTemplatesPropertyTemplates(pageTemplate, pageNode);

            _unitOfWork.Commit();
            return pageNode;
        }

        public void Update(int pageId, IEnumerable<Property> properties)
        {
            var page = GetById(pageId);

            if (page == null) throw new ArgumentException(Messages.PageNodeNotFound.Format(pageId));

            if (properties == null) throw new ArgumentException(Messages.PropertiesCannotBeNull);

            properties = properties.ToList();

            if (properties.Any(x => x.Id < 1)) throw new InvalidOperationException(Messages.PropertiesMustHaveIds);

            foreach (var property in properties)
            {
                var pageProperty = page.Properties.FirstOrDefault(x => x.Id == property.Id);

                if (pageProperty == null) throw new ArgumentException(Messages.PropertyNotFound.Format(property.Id));

                pageProperty.Text = property.Text;
            }

            _unitOfWork.Commit();
        }

        public void Delete(PageNode pageNode, bool commit = true)
        {
            if (pageNode == null)
                throw new ArgumentException(Messages.PageNodeCannotBeNull);

            DeletePageNode(pageNode, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            var pageNodeDbSet = _unitOfWork.Context.GetDbSet<PageNode>();
            var pageNodes = pageNodeDbSet
                        .Include(x => x.Properties)
                        .Include(x => x.PageNodes);

            var pageNode = pageNodes.FirstOrDefault(x => x.Id == id);

            if (pageNode == null)
                throw new ArgumentException(string.Format(Messages.PageNodeNotFound, id));

            DeletePageNode(pageNode, commit);
        }

        #region Private Helpers

        private void DeletePageNode(PageNode pageNode, bool commit = true)
        {
            var pageNodeDbSet = _unitOfWork.Context.GetDbSet<PageNode>();

            if (pageNode.PageNodes != null && pageNode.PageNodes.Any())
            {
                foreach (var node in pageNode.PageNodes.ToList())
                {
                    Delete(node, false);
                }
            }

            pageNodeDbSet.Remove(pageNode);

            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        private void AddPropertiesForPageTemplatesPropertyTemplates(PageTemplate pageTemplate, PageNode pageNode)
        {
            if (pageTemplate.PropertyTemplates != null && pageTemplate.PropertyTemplates.Any())
            {
                var propertyTemplates = pageTemplate.PropertyTemplates.ToList();
                foreach (var propertyTemplate in propertyTemplates)
                {
                    _propertyService.Create(pageNode, propertyTemplate, "", false);
                }
            }
        }

        #endregion
    }
}
