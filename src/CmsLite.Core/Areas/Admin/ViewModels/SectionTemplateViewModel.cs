using System.Collections.Generic;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class SectionTemplateViewModel
    {
        public int Id { get; set; }

        public string ControllerName { get; set; }

        public string Name { get; set; }

        public virtual IEnumerable<PageTemplateViewModel> PageTemplates { get; set; }
    }
}
