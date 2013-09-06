using System.Collections.Generic;
using CmsLite.Web.Areas.Admin.Models;

namespace CmsLite.Web.Areas.Admin.ViewModels
{
    public class PageViewModel : AdminLayoutModel
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string DisplayName { get; set; }

        public string UrlName { get; set; }

        public string ActionName { get; set; }

        public string ParentSectionUrlName { get; set; }

        public string ModelType { get; set; }

        public string EditUrl { get; set; }

        public IEnumerable<PropertyViewModel> Properties { get; set; }

        public IEnumerable<PageTemplateViewModel> PageTemplates { get; set; }

        public IEnumerable<PageViewModel> PageNodes { get; set; }

        public string IconImageName { get; set; }
    }
}