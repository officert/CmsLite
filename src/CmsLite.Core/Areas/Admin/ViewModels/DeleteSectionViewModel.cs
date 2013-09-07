using System.ComponentModel.DataAnnotations;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class DeleteSectionViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}