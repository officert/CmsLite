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
    public class SectionNodeService : ISectionNodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISectionTemplateService _sectionTemplateService; 
        private readonly IPageNodeService _pageNodeService;

        public SectionNodeService(IUnitOfWork unitOfWork, ISectionTemplateService sectionTemplateService, IPageNodeService pageNodeService)
        {
            _unitOfWork = unitOfWork;
            _sectionTemplateService = sectionTemplateService;
            _pageNodeService = pageNodeService;
        }

        public SectionNode GetById(int id)
        {
            return _unitOfWork.Context.GetDbSet<SectionNode>().FirstOrDefault(x => x.Id == id);
        }

        public SectionNode GetByUrlName(string urlName)
        {
            if(urlName.IsNullOrEmpty()) throw new ArgumentException("urlName");

            return _unitOfWork.Context.GetDbSet<SectionNode>().FirstOrDefault(x => x.UrlName.ToLower() == urlName.ToLower());
        }

        /// <summary>
        /// Gets all sections nodes in the database with the Page Nodes and their Page Templates.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SectionNode> GetAllWithDetails()
        {
            return _unitOfWork.Context.GetDbSet<SectionNode>()
                    .Include(x => x.PageNodes)
                    .Include(x => x.SectionTemplate.PageTemplates.Select(y => y.PageTemplates));
        }

        public IEnumerable<SectionNode> GetAll(bool includeTrashed = false)
        {
            var sectionNodes = _unitOfWork.Context.GetDbSet<SectionNode>().AsQueryable();

            if (!includeTrashed)
            {
                sectionNodes = sectionNodes.Where(x => !x.InTrash);
            }

            return sectionNodes;
        }

        public IEnumerable<SectionNode> GetAllTrashed()
        {
            return _unitOfWork.Context.GetDbSet<SectionNode>().Where(x => x.InTrash);
        }

        public SectionNode Create(int sectionTemplateId, string displayName, string urlName, bool commit = true)
        {
            if(displayName.IsNullOrEmpty())
                throw new ArgumentException(Messages.SectionNodeDisplayNameCannotBeNull);

            if(urlName.IsNullOrEmpty())
                throw new ArgumentException(Messages.SectionNodeUrlNameCannotBeNull);

            var sectionNodeDbSet = _unitOfWork.Context.GetDbSet<SectionNode>();
            var orderedSectionNodes = sectionNodeDbSet.OrderBy(x => x.Order);

            var formattedUrlName = CmsUrlHelper.FormatUrlName(!string.IsNullOrEmpty(urlName) ? urlName : displayName);

            if (sectionNodeDbSet.Any(s => s.UrlName == formattedUrlName))
                throw new ArgumentException(string.Format(Messages.SectionNodeUrlNameMustBeUnique, formattedUrlName));

            var sectionTemplate = _sectionTemplateService.GetById(sectionTemplateId);

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
            section.InTrash = false;

            sectionNodeDbSet.Add(section);

            //TODO: need to better understand the rules for automatically creating a page
            //_pageNodeService.CreateForSection(section, sectionTemplate.PageTemplates.First(x => x.ActionName == "Index").Id, section.DisplayName, section.DisplayName.ToLower());

            if (commit)
            {
                _unitOfWork.Commit();
            }

            return section;
        }

        public void Trash(int id, bool commit = false)
        {
            var sectionNodeDbSet = _unitOfWork.Context.GetDbSet<SectionNode>();

            var sectionNode = sectionNodeDbSet.FirstOrDefault(x => x.Id == id);

            if (sectionNode == null) throw new ArgumentException(string.Format(Messages.SectionNodeNotFound, id));

            if (sectionNode.InTrash) throw new InvalidOperationException(string.Format(Messages.SectionNodeInTrashAlready, id));

            sectionNode.InTrash = true;

            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        public void Delete(int id, bool commit = true)
        {
            var sectionNodeDbSet = _unitOfWork.Context.GetDbSet<SectionNode>();

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
                _unitOfWork.Commit();
            }
        }
    }
}