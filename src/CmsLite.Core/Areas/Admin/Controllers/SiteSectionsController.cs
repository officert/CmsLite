using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class SiteSectionsController : AdminBaseController
    {
        private readonly ISectionNodeService _sectionNodeService;
        private readonly ISectionTemplateService _sectionTemplateService;
        private readonly IPageNodeService _pageNodeService;
        private readonly IMappingEngine _mapper;

        public SiteSectionsController(ISectionNodeService sectionNodeService,
            IMappingEngine mapper,
            ISectionTemplateService sectionTemplateService,
            IPageNodeService pageNodeService)
        {
            _sectionNodeService = sectionNodeService;
            _mapper = mapper;
            _sectionTemplateService = sectionTemplateService;
            _pageNodeService = pageNodeService;
        }

        public ActionResult Index()
        {
            var sections = _sectionNodeService.GetAllWithDetails();
            var sectionTemplates = _sectionTemplateService.GetAll();

            var model = new SiteSectionsModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu(),
                Sections = _mapper.Map<IEnumerable<SectionNode>, IEnumerable<SectionNodeLightViewModel>>(sections),
                SectionTemplates = _mapper.Map<IEnumerable<SectionTemplate>, IEnumerable<SectionTemplateViewModel>>(sectionTemplates)
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult CreateSection(CreateSectionViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonError(JsonRequestBehavior.DenyGet);

            var newSectionNode =
                _mapper.Map<SectionNode, SectionNodeViewModel>(_sectionNodeService.Create(model.SectionTemplateId,
                    model.DisplayName, model.UrlName));

            return Json(newSectionNode, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteSection(DeleteSectionViewModel model)
        {
            if (!ModelState.IsValid) //TODO: need to get modelstate transfer attribute
                return JsonError(JsonRequestBehavior.DenyGet);

            _sectionNodeService.Delete(model.Id);

            return Json(JsonRequestBehavior.DenyGet);
        }

        public ActionResult EditPage(int id)
        {
            var model = _mapper.Map<PageNode, PageNodeViewModel>(_pageNodeService.GetWithDetails(id));

            model.SidebarMenu = GetAdminMenu();
            model.UserMenu = GetAdminUserMenu();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PageNodeViewModel model)
        {
            var pageWithEdits = new PageNode
            {
                Id = model.Id,
                Properties =
                    _mapper.Map<IEnumerable<PropertyViewModel>, IEnumerable<Property>>(model.Properties).ToList()
            };
            _pageNodeService.Update(pageWithEdits);
            return RedirectToAction("EditPage", new { id = model.Id });
        }

        [HttpPost]
        public JsonResult CreatePage(CreatePageViewModel model)
        {
            if ((!ModelState.IsValid && model.ParentSectionId < 1) ||
                (!ModelState.IsValid && model.ParentPageId < 1))
                return JsonError(JsonRequestBehavior.DenyGet);

            PageNodeViewModel newPageNode = null;

            if (model.ParentSectionId > 0)
            {
                newPageNode =
                    _mapper.Map<PageNode, PageNodeViewModel>(_pageNodeService.CreateForSection(model.ParentSectionId,
                        model.PageTemplateId, model.DisplayName, model.UrlName));
            }
            else if (model.ParentPageId > 0)
            {
                newPageNode =
                    _mapper.Map<PageNode, PageNodeViewModel>(_pageNodeService.CreateForPage(model.ParentPageId,
                        model.PageTemplateId, model.DisplayName, model.UrlName));
            }
            else
            {
                ModelState.AddModelError("", "ParentSectionId or ParentPageId must be provided.");
                return JsonError(JsonRequestBehavior.DenyGet);
            }

            return Json(newPageNode, JsonRequestBehavior.DenyGet);
        }
    }
}
