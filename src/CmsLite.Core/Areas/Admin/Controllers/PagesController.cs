using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Areas.Admin.Menus;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Core.Attributes;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using MenuGen.Attributes;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class PagesController : AdminBaseController
    {
        private readonly IPageNodeService _pageNodeService;
        private readonly IMappingEngine _mapper;

        public PagesController(IMappingEngine mappingEngine, IPageNodeService pageNodeService)
        {
            _mapper = mappingEngine;
            _pageNodeService = pageNodeService;
        }

        [ImportModelStateFromTempData]
        [MenuNode(Key = "EditPage", ParentKey = "SiteSections", Text = "EditPage", Menus = new[] { typeof(AdminSidebarMenu) })]
        public ActionResult EditPage(int id)
        {
            var model = _mapper.Map<PageNode, PageNodeViewModel>(_pageNodeService.GetByIdWithDetails(id));

            model.SidebarMenu = GetAdminMenu();
            model.UserMenu = GetAdminUserMenu();

            return View(model);
        }

        [HttpPost, ExportModelStateToTempData]
        public ActionResult EditPage(EditPageNodeViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("EditPage", new { id = model.Id });

            _pageNodeService.Update(model.Id, _mapper.Map<IEnumerable<PropertyViewModel>, IEnumerable<Property>>(model.Properties));

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
