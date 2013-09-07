namespace CmsLite.Core.Areas.Admin.Models
{
    public class AdminLayoutModel
    {
        public AdminContextModel Context { get; set; }
        public AdminMenu SidebarMenu { get; set; }
        public AdminMenu UserMenu { get; set; }
    }
}