using System;
using System.Web;
using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Core.Attributes;
using CmsLite.Interfaces.Services;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    public class MediaController : AdminBaseController
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [ImportModelStateFromTempData]
        public ActionResult Index()
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

        [HttpPost, ExportModelStateToTempData]
        public ActionResult Index(HttpPostedFileBase file, string name)
        {
            if (file == null)
            {
                ModelState.AddModelError("FileNull", "You must provide a file to upload.");
                return RedirectToAction("Index");
            }

            var fileData = new byte[file.ContentLength];
            file.InputStream.Read(fileData, 0, file.ContentLength);

            _mediaService.Create(fileData, file.ContentType, name);
            return RedirectToAction("Index");
        }
    }
}
