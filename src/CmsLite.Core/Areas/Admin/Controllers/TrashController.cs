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
    public class TrashController : AdminBaseController
    {        
        private readonly ISectionNodeService _sectionNodeService;
        private readonly IMappingEngine _mapper;

        public TrashController(ISectionNodeService sectionNodeService,
            IMappingEngine mapper)
        {
            _sectionNodeService = sectionNodeService;
            _mapper = mapper;
        }

        [MenuNode(Key = "Trash", Text = "Trash", Menus = new[] { typeof(AdminSidebarMenu) })]
        public ActionResult Index()
        {
            var sections = _sectionNodeService.GetAllTrashed();

            var model = new TrashModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu(),
                TrashedSections = _mapper.Map<IEnumerable<SectionNode>, IEnumerable<SectionNodeLightViewModel>>(sections)
            };
            return View(model);
        }
    }
}
