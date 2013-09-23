using System;
using System.Web.Helpers;
using System.Web.Mvc;
using CmsLite.Interfaces.Services;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminImagesController : AdminBaseController
    {
        private readonly IMediaService _mediaService;

        public AdminImagesController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        #region Images

        public void MediaThumbnails(int id)
        {
            if(id < 1) throw new ArgumentException("Id cannot be 0");

            var file = _mediaService.Find(x => x.Id == id);
            if (file != null)
            {
                GetImage(file.FileData, 120, 120, false, false);
            }
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

        #endregion
    }
}
