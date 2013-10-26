﻿using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;
using MenuGen.Attributes;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : AdminBaseController
    {
        [MenuNode(Key = "Dashboard", Text = "Dashboard")]
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
