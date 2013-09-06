using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services.Helpers;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;

namespace CmsLite.Services
{
    public class SectionNodeService : ServiceBase<SectionNode>, ISectionNodeService
    {
        private readonly ISectionTemplateService _sectionTemplateService; 
        private readonly IPageNodeService _pageNodeService;

        public SectionNodeService(IUnitOfWork unitOfWork, ISectionTemplateService sectionTemplateService, IPageNodeService pageNodeService)
            : base(unitOfWork)
        {
            _sectionTemplateService = sectionTemplateService;
            _pageNodeService = pageNodeService;
        }

        /// <summary>
        /// Gets all sections nodes in the database with the Page Nodes and their Page Templates
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SectionNode> GetAllWithDetails()
        {
            return UnitOfWork.Context.GetDbSet<SectionNode>()
                    .Include(x => x.PageNodes)
                    .Include(x => x.SectionTemplate.PageTemplates.Select(y => y.PageTemplates));
        }

        public SectionNode Create(int sectionTemplateId, string displayName, string urlName, bool commit = true)
        {
            if(displayName.IsNullOrEmpty())
                throw new ArgumentException(Messages.SectionNodeDisplayNameCannotBeNull);

            if(urlName.IsNullOrEmpty())
                throw new ArgumentException(Messages.SectionNodeUrlNameCannotBeNull);

            var sectionNodeDbSet = UnitOfWork.Context.GetDbSet<SectionNode>();
            var orderedSectionNodes = sectionNodeDbSet.OrderBy(x => x.Order);

            var formattedUrlName = CmsUrlHelper.FormatUrlName(!string.IsNullOrEmpty(urlName) ? urlName : displayName);

            if (sectionNodeDbSet.Any(s => s.UrlName == formattedUrlName))
                throw new ArgumentException(string.Format(Messages.SectionNodeUrlNameMustBeUnique, formattedUrlName));

            var sectionTemplate = _sectionTemplateService.Find(x => x.Id == sectionTemplateId);

            if (sectionTemplate == null)
                throw new ArgumentException(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId));

            var section = sectionNodeDbSet.Create();

            section.SectionTemplateId = sectionTemplate.Id;
            section.DisplayName = displayName;
            section.UrlName = formattedUrlName;
            section.Order = !orderedSectionNodes.Any() ? CmsConstants.FirstOrderNumber : orderedSectionNodes.Count();
            section.IsPublished = false;
            section.CreatedOn = DateTime.UtcNow;
            section.ModifiedOn = section.CreatedOn;

            sectionNodeDbSet.Add(section);

            //TODO: need to better understand the rules for automatically creating a page
            //_pageNodeService.CreateForSection(section, sectionTemplate.PageTemplates.First(x => x.ActionName == "Index").Id, section.DisplayName, section.DisplayName.ToLower());

            if (commit)
            {
                UnitOfWork.Commit();
            }

            return section;
        }

        public void Delete(int id, bool commit = true)
        {
            var sectionNodeDbSet = UnitOfWork.Context.GetDbSet<SectionNode>();

            var sectionNode = sectionNodeDbSet
                        .Include(x => x.PageNodes.Select(y => y.PageNodes))
                        .FirstOrDefault(x => x.Id == id);

            if(sectionNode == null)
                throw new ArgumentException(string.Format(Messages.SectionNodeNotFound, id));

            if (sectionNode.PageNodes != null && sectionNode.PageNodes.Any())
            {
                foreach (var pageNode in sectionNode.PageNodes.ToList())
                {
                    _pageNodeService.Delete(pageNode, false);
                }
            }

            sectionNodeDbSet.Remove(sectionNode);

            if (commit)
            {
                UnitOfWork.Commit();
            }
        }
    }
}