using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services.Extensions;
using CmsLite.Services.Helpers;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;

namespace CmsLite.Services
{
    public class PageNodeService : ServiceBase<PageNode>, IPageNodeService
    {
        private readonly IPropertyService _propertyService;

        public PageNodeService(IUnitOfWork unitOfWork, IPropertyService propertyService)
            : base(unitOfWork)
        {
            _propertyService = propertyService;
        }

        public PageNode GetWithDetails(int id)
        {
            return UnitOfWork.Context.GetDbSet<PageNode>()
                .Include(x => x.PageTemplate)
                .Include(x => x.Properties.Select(y => y.PropertyTemplate))
                .FirstOrDefault(x => x.Id == id);
        }

        public PageNode CreateForSection(int sectionId, int pageTemplateId, string displayName, string urlName, bool commit = true)
        {
            if (displayName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageNodeDisplayNameCannotBeNull);

            if (urlName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageNodeUrlNameCannotBeNull);

            var pageNodeDbSet = UnitOfWork.Context.GetDbSet<PageNode>();

            var parentSectionNode = UnitOfWork.Context.GetDbSet<SectionNode>()
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
                UnitOfWork.Commit();
            }

            return pageNode;
        }

        public PageNode CreateForPage(int pageId, int pageTemplateId, string displayName, string urlName)
        {
            if (displayName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageNodeDisplayNameCannotBeNull);

            if (urlName.IsNullOrEmpty())
                throw new ArgumentException(Messages.PageNodeUrlNameCannotBeNull);

            var pageNodeDbSet = UnitOfWork.Context.GetDbSet<PageNode>();
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

            UnitOfWork.Commit();
            return pageNode;
        }

        public void Update(PageNode pageNodeWithUpdates)
        {
            var pageDbSet = UnitOfWork.Context.GetDbSet<PageNode>();
            var page = pageDbSet.FirstOrDefault(x => x.Id == pageNodeWithUpdates.Id);

            if (page == null)
                throw new ArgumentException(string.Format("The page with id : {0} does not exist.", pageNodeWithUpdates.Id));

            foreach (var property in pageNodeWithUpdates.Properties)
            {
                if (property.Id < 1)
                    throw new ArgumentException("All properties must have an Id.");

                var pageProperty = page.Properties.FirstOrDefault(x => x.Id == property.Id);

                if (pageProperty == null)
                    throw new ArgumentException(string.Format("Sorry, the property with Id : {0} does not exist on this page.", property.Id));

                pageProperty.Text = property.Text;
            }

            UnitOfWork.Commit();
        }

        public void Delete(PageNode pageNode, bool commit = true)
        {
            if (pageNode == null)
                throw new ArgumentException(Messages.PageNodeCannotBeNull);

            DeletePageNode(pageNode, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            var pageNodeDbSet = UnitOfWork.Context.GetDbSet<PageNode>();
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
            var pageNodeDbSet = UnitOfWork.Context.GetDbSet<PageNode>();

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
                UnitOfWork.Commit();
            }
        }

        private void AddPropertiesForPageTemplatesPropertyTemplates(PageTemplate pageTemplate, PageNode pageNode)
        {
            if (pageTemplate.PropertyTemplates != null && pageTemplate.PropertyTemplates.Any())
            {
                var propertyTemplates = pageTemplate.PropertyTemplates.ToList();
                foreach (var propertyTemplate in propertyTemplates)
                {
                    _propertyService.Create(pageNode, propertyTemplate, commit: false);
                }
            }
        }

        #endregion
    }
}
