using System.Web;
using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Core.Attributes;
using CmsLite.Interfaces.Services;
using MenuGen.Attributes;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class MediaController : AdminBaseController
    {
        private readonly IFileService _mediaService;

        public MediaController(IFileService mediaService)
        {
            _mediaService = mediaService;
        }

        [ImportModelStateFromTempData]
        [MenuNode(Key = "Media", Text = "Media")]
        public ActionResult Index()
        {
            var model = new MediaModel
            {
                Files = _mediaService.GetAllFiles(),
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
