using System.Web.Mvc;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class PropertyViewModel
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string CmsPropertyName { get; set; }

        public string Description { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public int Order { get; set; }

        public bool Required { get; set; }

        public string TabName { get; set; }
    }
}