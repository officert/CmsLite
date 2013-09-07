using System.Collections.Generic;
using CmsLite.Core.Areas.Admin.ViewModels;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class SiteSectionsModel : AdminLayoutModel
    {
        public IEnumerable<SectionViewModel> Sections { get; set; }
        public IEnumerable<SectionTemplateViewModel> SectionTemplates { get; set; }
    }
}