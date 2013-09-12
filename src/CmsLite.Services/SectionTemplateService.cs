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
    public class SectionTemplateService : ServiceBase<SectionTemplate>, ISectionTemplateService
    {
        private readonly IPageTemplateService _pageTemplateService;

        public SectionTemplateService(IUnitOfWork unitOfWork, IPageTemplateService pageTemplateService)
            : base(unitOfWork)
        {
            _pageTemplateService = pageTemplateService;
        }

        public SectionTemplate Create(string controllerName, string name = null, string iconImageName = null, bool commit = true)
        {
            if (controllerName.IsNullOrEmpty())
                throw new ArgumentException(Messages.SectionTemplateControllerNameCannotBeNull);

            var sectionTemplateDbSet = UnitOfWork.Context.GetDbSet<SectionTemplate>();

            if(sectionTemplateDbSet.Any(x => x.ControllerName == controllerName))
                throw new ArgumentException(string.Format(Messages.SectionTemplateControllerNameMustBeUnique, controllerName));

            var sectionTemplate = sectionTemplateDbSet.Create();
            sectionTemplate.ControllerName = controllerName;
            sectionTemplate.Name = name.IsNullOrEmpty()
                            ? controllerName
                            : name;
            sectionTemplate.IconImageName = iconImageName.IsNullOrEmpty()
                            ? CmsConstants.SectionTemplateDefaultThumbnail
                            : iconImageName;

            sectionTemplateDbSet.Add(sectionTemplate);

            if (commit)
            {
                UnitOfWork.Commit();
            }

            return sectionTemplate;
        }

        public SectionTemplate Update(SectionTemplate sectionTemplate, string name, string iconImageName = null, bool commit = true)
        {
            return UpdateSectionTemplate(sectionTemplate, name, iconImageName, commit);
        }

        public SectionTemplate Update(int id, string name, string iconImageName = null,  bool commit = true)
        {
            var sectionTemplateDbSet = UnitOfWork.Context.GetDbSet<SectionTemplate>();
            var sectionTemplate = sectionTemplateDbSet.Include(x => x.PageTemplates).FirstOrDefault(x => x.Id == id);

            return UpdateSectionTemplate(sectionTemplate, name, iconImageName, commit);
        }

        public void Delete(int id, bool commit = true)
        {
            var sectionTemplateDbSet = UnitOfWork.Context.GetDbSet<SectionTemplate>();
            var sectionTemplate = sectionTemplateDbSet.Include(x => x.PageTemplates).FirstOrDefault(x => x.Id == id);

            if(sectionTemplate == null)
                throw new ArgumentException(string.Format(Messages.SectionTemplateNotFound, id));

            if (sectionTemplate.PageTemplates != null && sectionTemplate.PageTemplates.Any())
            {
                foreach (var pageTemplate in sectionTemplate.PageTemplates.ToList())
                {
                    _pageTemplateService.Delete(pageTemplate, false);
                }
            }

            sectionTemplateDbSet.Remove(sectionTemplate);

            if (commit)
            {
                UnitOfWork.Commit();
            }
        }

        #region Private Helpers

        private SectionTemplate UpdateSectionTemplate(SectionTemplate sectionTemplate, string name, string iconImageName = null, bool commit = true)
        {
            if (sectionTemplate == null)
                throw new ArgumentException(Messages.SectionTemplateCannotBeNull);

            sectionTemplate.Name = !name.IsNullOrEmpty()
                                    ? name
                                    : sectionTemplate.Name;
            sectionTemplate.IconImageName = !iconImageName.IsNullOrEmpty()
                                    ? iconImageName
                                    : sectionTemplate.IconImageName;

            if (commit)
            {
                UnitOfWork.Commit();
            }

            return sectionTemplate;
        }

        #endregion
    }
}
