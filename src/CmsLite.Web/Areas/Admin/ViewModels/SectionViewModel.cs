using System.Collections.Generic;

namespace CmsLite.Web.Areas.Admin.ViewModels
{
    public class SectionViewModel
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string DisplayName { get; set; }

        public string UrlName { get; set; }

        public ICollection<PageViewModel> PageNodes { get; set; }

        public IEnumerable<PageTemplateViewModel> PageTemplates { get; set; }

        public string IconImageName { get; set; }
    }
}