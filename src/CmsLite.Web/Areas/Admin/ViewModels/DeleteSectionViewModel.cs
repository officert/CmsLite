using System.ComponentModel.DataAnnotations;

namespace CmsLite.Web.Areas.Admin.ViewModels
{
    public class DeleteSectionViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}