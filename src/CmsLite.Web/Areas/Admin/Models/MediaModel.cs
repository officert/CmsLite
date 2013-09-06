using System.Collections.Generic;
using CmsLite.Domains.Entities;
using CmsLite.Web.Areas.Admin.ViewModels;

namespace CmsLite.Web.Areas.Admin.Models
{
    public class MediaModel : AdminLayoutModel
    {
        public IEnumerable<File> Files { get; set; }
        public UploadFileViewModel UploadFile { get; set; }
    }
}