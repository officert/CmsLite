using System.Collections.Generic;
using CmsLite.Web.Areas.Admin.ViewModels;

namespace CmsLite.Web.Areas.Admin.Models
{
    public class SiteSectionsModel : AdminLayoutModel
    {
        public IEnumerable<SectionViewModel> Sections { get; set; }
        public IEnumerable<SectionTemplateViewModel> SectionTemplates { get; set; }
    }
}