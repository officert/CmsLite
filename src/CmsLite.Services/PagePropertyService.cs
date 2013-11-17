using System;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Utilities;
using CmsLite.Utilities.Cms;

namespace CmsLite.Services
{
    public class PagePropertyService : IPagePropertyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PagePropertyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PageProperty Create(PageNode pageNode, PagePropertyTemplate propertyTemplate, string text = null, bool commit = true)
        {
            Ensure.ArgumentIsNotNull(pageNode, "pageNode");
            Ensure.ArgumentIsNotNull(propertyTemplate, "propertyTemplate");

            return CreateProperty(pageNode, propertyTemplate, text, commit);
        }

        public PageProperty Create(int pageNodeId, int propertyTemplateId, string text = null, bool commit = true)
        {
            var pageNode = _unitOfWork.Context.GetDbSet<PageNode>()
                        .Include(x => x.PageTemplate.PropertyTemplates)
                        .FirstOrDefault(x => x.Id == pageNodeId);

            if (pageNode == null)
                throw new ArgumentException(string.Format(Messages.PageNodeNotFound, pageNodeId));

            var propertyTemplate = pageNode.PageTemplate.PropertyTemplates.FirstOrDefault(x => x.Id == propertyTemplateId);

            if (propertyTemplate == null)
                throw new ArgumentException(string.Format(Messages.PropertyTemplateNotFoundForPageTemplate, propertyTemplateId, pageNode.PageTemplate.Id));

            return CreateProperty(pageNode, propertyTemplate, text, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            if (id < 1) throw new ArgumentException("id");

            var propertyDbSet = _unitOfWork.Context.GetDbSet<PageProperty>();
            var property = propertyDbSet.FirstOrDefault(x => x.Id == id);

            if (property == null)
                throw new ArgumentException(string.Format(Messages.PropertyNotFound, id));

            propertyDbSet.Remove(property);

            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        #region Private Helpers

        private PageProperty CreateProperty(PageNode pageNode, PagePropertyTemplate propertyTemplate, string text = null, bool commit = true)
        {
            Ensure.ArgumentIsNotNull(pageNode, "pageNode");
            Ensure.ArgumentIsNotNull(propertyTemplate, "propertyTemplate");

            var propertyDbSet = _unitOfWork.Context.GetDbSet<PageProperty>();

            var property = propertyDbSet.Create();
            property.PropertyTemplate = propertyTemplate;
            property.ParentPageNode = pageNode;
            property.Text = text;
            property.Order = pageNode.Properties == null || !pageNode.Properties.Any()
                            ? CmsConstants.FirstOrderNumber
                            : pageNode.Properties.Count();

            propertyDbSet.Add(property);

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return property;
        }

        #endregion
    }
}
