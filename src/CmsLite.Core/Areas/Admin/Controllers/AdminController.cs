using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : AdminBaseController
    {
        private readonly ISectionNodeService _sectionNodeService;
        private readonly IPageNodeService _pageNodeService;
        private readonly ISectionTemplateService _sectionTemplateService;
        private readonly IPageTemplateService _pageTemplateService;
        private readonly IMediaService _mediaService;
        private readonly IMappingEngine _mapper;

        public AdminController(ISectionNodeService sectionNodeService,
            IPageNodeService pageNodeService,
            IMappingEngine mapper,
            ISectionTemplateService sectionTemplateService,
            IPageTemplateService pageTemplateService,
            IMediaService mediaService)
        {
            _sectionNodeService = sectionNodeService;
            _pageNodeService = pageNodeService;
            _mapper = mapper;
            _sectionTemplateService = sectionTemplateService;
            _pageTemplateService = pageTemplateService;
            _mediaService = mediaService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            var model = new DashboardModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu()
            };
            return View(model);
        }

        public ActionResult SiteSections()
        {
            var sections = _sectionNodeService.GetAllWithDetails();
            var sectionTemplates = _sectionTemplateService.GetAll();

            var model = new SiteSectionsModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu(),
                Sections = _mapper.Map<IEnumerable<SectionNode>, IEnumerable<SectionViewModel>>(sections),
                SectionTemplates =
                    _mapper.Map<IEnumerable<SectionTemplate>, IEnumerable<SectionTemplateViewModel>>(sectionTemplates)
            };
            return View(model);
        }

        #region Sections

        [HttpPost]
        public JsonResult CreateSection(CreateSectionViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new JsonResponse {Success = false}, JsonRequestBehavior.DenyGet);

            var newSectionNode =
                _mapper.Map<SectionNode, SectionViewModel>(_sectionNodeService.Create(model.SectionTemplateId,
                    model.DisplayName, model.UrlName));

            return Json(newSectionNode, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult DeleteSection(DeleteSectionViewModel model)
        {
            if (!ModelState.IsValid) //TODO: need to get modelstate transfer attribute
                RedirectToAction("SiteSections");

            _sectionNodeService.Delete(model.Id);

            return Json(new {Success = true}, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Pages

        public ActionResult EditPage(int id)
        {
            var model = _mapper.Map<PageNode, PageViewModel>(_pageNodeService.GetWithDetails(id));

            model.SidebarMenu = GetAdminMenu();
            model.UserMenu = GetAdminUserMenu();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PageViewModel model)
        {
            var pageWithEdits = new PageNode
            {
                Id = model.Id,
                Properties =
                    _mapper.Map<IEnumerable<PropertyViewModel>, IEnumerable<Property>>(model.Properties).ToList()
            };
            _pageNodeService.Update(pageWithEdits);
            return RedirectToAction("EditPage", new {id = model.Id});
        }

        [HttpPost]
        public JsonResult CreatePage(CreatePageViewModel model)
        {
            if ((!ModelState.IsValid && model.ParentSectionId < 1) ||
                (!ModelState.IsValid && model.ParentPageId < 1))
                return Json(new JsonResponse {Success = false}, JsonRequestBehavior.DenyGet);

            PageViewModel newPageNode = null;

            if (model.ParentSectionId > 0)
            {
                newPageNode =
                    _mapper.Map<PageNode, PageViewModel>(_pageNodeService.CreateForSection(model.ParentSectionId,
                        model.PageTemplateId, model.DisplayName, model.UrlName));
            }
            else if (model.ParentPageId > 0)
            {
                newPageNode =
                    _mapper.Map<PageNode, PageViewModel>(_pageNodeService.CreateForPage(model.ParentPageId,
                        model.PageTemplateId, model.DisplayName, model.UrlName));
            }
            else
            {
                return
                    Json(
                        new JsonResponse
                        {
                            Success = false,
                            Message = "ParentSectionId or ParentPageId must be provided."
                        }, JsonRequestBehavior.DenyGet);
            }

            return Json(newPageNode, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Media

        public ActionResult Media()
        {
            var model = new MediaModel
            {
                Files = _mediaService.GetAll(),
                UploadFile = new UploadFileViewModel(),
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Media(HttpPostedFileBase file, string name)
        {
            if (file == null)
                throw new ArgumentException("You must provide a file to upload.");

            var fileData = new byte[file.ContentLength];
            file.InputStream.Read(fileData, 0, file.ContentLength);

            _mediaService.Create(fileData, file.ContentType, name);
            return RedirectToAction("Media");
        }

        #endregion

        #region Trash

        public ActionResult Trash()
        {
            return View();
        }

        #endregion

        #region Images

        public void MediaThumbnails(int id)
        {
            var file = _mediaService.GetAll().FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                GetImage(file.FileData, 120, 120, false, false);
            }
        }

        #endregion

        #region Settings

        public ActionResult Settings()
        {
            var model = new AdminLayoutModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu()
            };
            return View(model);
        }

        #endregion

        #region Styles

        public ActionResult Styles()
        {
            var model = new AdminLayoutModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu()
            };
            return View(model);
        }

        #endregion

        #region Private Helpers

        private static void GetImage(byte[] imageContents, int height, int width, bool preserveAspectRatio, bool preventEnlarge)
        {
            if (imageContents == null)
                throw new ArgumentException("The image data cannot be null.");

            var webImage = new WebImage(imageContents);
            webImage.Resize(width, height, preserveAspectRatio, preventEnlarge).Crop(1, 1).Write();
        }

        private AdminMenu GetAdminMenu()
        {
            var menu = new AdminMenu
            {
                Links = new List<AdminNavLink>
                                                  {
                                                      new AdminNavLink { Url  =  Url.Action("Dashboard"), Text = "Dashboard", IconClass = "icon-home" },
                                                      new AdminNavLink { Url  =  Url.Action("SiteSections"), Text = "Site Sections", IconClass = "icon-book" },
                                                      new AdminNavLink { Url = Url.Action("Media"), Text = "Media", IconClass = "icon-hdd"}
                                                  }
            };
            return menu;
        }

        private AdminMenu GetAdminUserMenu()
        {
            var menu = new AdminMenu
            {
                Links = new List<AdminNavLink>
                                                  {
                                                      new AdminNavLink { Url  =  Url.Action("SignOut", "Account"), Text = "Sign Out", IconClass = "icon-signout" },
                                                      new AdminNavLink(),
                                                      new AdminNavLink { Url  =  Url.Action("Settings"), Text = "Settings", IconClass = "icon-gear" },
                                                  }
            };
            return menu;
        }

        #endregion
    }

    public class JsonResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
