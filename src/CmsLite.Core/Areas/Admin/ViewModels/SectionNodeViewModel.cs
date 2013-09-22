using System.Collections.Generic;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class SectionNodeViewModel
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string DisplayName { get; set; }

        public string UrlName { get; set; }

        public ICollection<PageNodeViewModel> PageNodes { get; set; }

        public IEnumerable<PageTemplateViewModel> PageTemplates { get; set; }

        public string IconImageName { get; set; }
    }
}