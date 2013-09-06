using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CmsLite.Web.Areas.Admin.ViewModels
{
    public class CreateSectionViewModel
    {
        [Display(Name = "Template :", Description = "Select a template to use for this section.")]
        public IEnumerable<SelectListItem> SectionTemplates { get; set; }

        [Required(ErrorMessage = "You must select a section template.")]
        public int SectionTemplateId { get; set; }

        [Required(ErrorMessage = "You must enter a name for your section.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "You section name must be between 1 and 255 characters.")]
        [Display(Name = "Display Name :", Description = "Enter a name that will appear in the CMS.")]
        public string DisplayName { get; set; }

        [Display(Name = "Url Name :", Description = "Enter a url name to link to this page.")]
        public string UrlName { get; set; }
    }
}