using System.ComponentModel.DataAnnotations;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "You must provide a file to upload.")]
        public byte[] FileData { get; set; }

        [Required(ErrorMessage = "Mime Type is required to upload a file.")]
        public string MimeType { get; set; }

        public string Name { get; set; }
    }
}