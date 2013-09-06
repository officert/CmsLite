using System;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;

namespace CmsLite.Services
{
    public class PropertyService : ServiceBase<Property>, IPropertyService
    {
        public PropertyService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Property Create(PageNode pageNode, PropertyTemplate propertyTemplate, string text = null, bool commit = true)
        {
            if(pageNode == null)
                throw new ArgumentException(Messages.PageNodeCannotBeNull);

            if(propertyTemplate == null)
                throw new ArgumentException(Messages.PropertyTemplateCannotBeNull);

            return CreateProperty(pageNode, propertyTemplate, text, commit);
        }

        public Property Create(int pageNodeId, int propertyTemplateId, string text = null, bool commit = true)
        {
            var pageNode = UnitOfWork.Context.GetDbSet<PageNode>()
                        .Include(x => x.PageTemplate.PropertyTemplates)
                        .FirstOrDefault(x => x.Id == pageNodeId);

            if (pageNode == null)
                throw new ArgumentException(string.Format(Messages.PageNodeNotFound, pageNodeId));

            var propertyTemplate = pageNode.PageTemplate.PropertyTemplates.FirstOrDefault(x => x.Id == propertyTemplateId);

            if (propertyTemplate == null)
                throw new ArgumentException(string.Format(Messages.PropertyTemplateNotFoundForPageTemplate, propertyTemplateId, pageNode.PageTemplateId));

            return CreateProperty(pageNode, propertyTemplate, text, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            var propertyDbSet = UnitOfWork.Context.GetDbSet<Property>();
            var property = propertyDbSet.FirstOrDefault(x => x.Id == id);

            if (property == null)
                throw new ArgumentException(string.Format(Messages.PropertyNotFound, id));

            propertyDbSet.Remove(property);

            if (commit)
            {
                UnitOfWork.Commit();
            }
        }

        #region Private Helpers

        private Property CreateProperty(PageNode pageNode, PropertyTemplate propertyTemplate, string text = null, bool commit = true)
        {
            var propertyDbSet = UnitOfWork.Context.GetDbSet<Property>();

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
                UnitOfWork.Commit();
            }

            return property;
        }

        #endregion
    }
}
