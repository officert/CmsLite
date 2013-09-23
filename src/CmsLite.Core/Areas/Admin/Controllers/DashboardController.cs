using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : AdminBaseController
    {
        public ActionResult Index()
        {
            var model = new DashboardModel
            {
                SidebarMenu = GetAdminMenu(),
                UserMenu = GetAdminUserMenu()
            };
            return View(model);
        }
    }
}
