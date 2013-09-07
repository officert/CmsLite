using System.Collections.Generic;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class AdminMenu
    {
        public IEnumerable<AdminNavLink> Links { get; set; }     
    }

    public class AdminNavLink
    {
        public string Url { get; set; }
        public string Text { get; set; }
        public string IconClass { get; set; }
    }
}