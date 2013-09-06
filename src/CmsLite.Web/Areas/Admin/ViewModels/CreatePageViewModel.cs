using System.ComponentModel.DataAnnotations;

namespace CmsLite.Web.Areas.Admin.ViewModels
{
    public class CreatePageViewModel
    {
        public int ParentSectionId { get; set; }

        public int ParentPageId { get; set; }

        [Required(ErrorMessage = "You must select a page template.")]
        public int PageTemplateId { get; set; }

        [Required(ErrorMessage = "You must enter a name for your page.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Your page name must be between 1 and 255 characters")]
        [Display(Name = "Display Name :", Description = "Enter a name that will appear in the CMS.")]
        public string DisplayName { get; set; }

        [Display(Name = "Url Name :", Description = "Enter a url name to link to this page.")]
        public string UrlName { get; set; }
    }
}