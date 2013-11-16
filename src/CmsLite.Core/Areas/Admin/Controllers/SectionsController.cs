using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Areas.Admin.Menus;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using MenuGen.Attributes;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class SectionsController : AdminBaseController
    {
        private readonly ISectionNodeService _sectionNodeService;
        private readonly ISectionTemplateService _sectionTemplateService;
        private readonly IMappingEngine _mapper;

        public SectionsController(ISectionNodeService sectionNodeService,
            IMappingEngine mapper,
            ISectionTemplateService sectionTemplateService)
        {
            _sectionNodeService = sectionNodeService;
            _mapper = mapper;
            _sectionTemplateService = sectionTemplateService;
        }

        [MenuNode(Key = "SiteSections", Text = "Site Sections", Menus = new[] { typeof(AdminSidebarMenu) })]
        public ActionResult Index()
        {
            var sections = _sectionNodeService.GetAllWithDetails();
            var sectionTemplates = _sectionTemplateService.GetAllSectionTemplates();

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
    }
}
