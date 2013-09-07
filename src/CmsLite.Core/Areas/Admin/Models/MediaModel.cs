using System.Collections.Generic;
using CmsLite.Core.Areas.Admin.ViewModels;
using CmsLite.Domains.Entities;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class MediaModel : AdminLayoutModel
    {
        public IEnumerable<File> Files { get; set; }
        public UploadFileViewModel UploadFile { get; set; }
    }
}