using System.Collections.Generic;
using CmsLite.Core.Areas.Admin.Models;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class PageNodeLightViewModel : AdminLayoutModel
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string DisplayName { get; set; }

        public string UrlName { get; set; }

        public string ActionName { get; set; }

        public string ParentSectionUrlName { get; set; }

        public string ModelType { get; set; }

        public string EditUrl { get; set; }

        public IEnumerable<PageTemplateViewModel> PageTemplates { get; set; }

        public IEnumerable<PageNodeViewModel> PageNodes { get; set; }

        public string IconImageName { get; set; }
    }
}