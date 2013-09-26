using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        public AdminContextModel Context { get; set; }

        protected AdminMenu GetAdminMenu()
        {
            var menu = new AdminMenu
            {
                Links = new List<AdminNavLink>
                                                  {
                                                      new AdminNavLink { Url  =  Url.Action("Index", "Dashboard"), Text = "Dashboard", IconClass = "icon-home" },
                                                      new AdminNavLink { Url  =  Url.Action("Index", "Sections"), Text = "Site Sections", IconClass = "icon-book" },
                                                      new AdminNavLink { Url = Url.Action("Index", "Media"), Text = "Media", IconClass = "icon-hdd"},
                                                      new AdminNavLink { Url = Url.Action("Index", "Trash"), Text = "Trash", IconClass = "icon-trash"}
                                                  }
            };
            return menu;
        }

        protected AdminMenu GetAdminUserMenu()
        {
            var menu = new AdminMenu
            {
                Links = new List<AdminNavLink>
                                                  {
                                                      new AdminNavLink { Url  =  Url.Action("SignOut", "Account"), Text = "Sign Out", IconClass = "icon-signout" },
                                                      new AdminNavLink(),
                                                      new AdminNavLink { Url  =  Url.Action("Settings"), Text = "Settings", IconClass = "icon-gear" },
                                                  }
            };
            return menu;
        }

        #region Json Helpers

        /// <summary>
        /// Creates a JSON object with an Errors property that contains any ModelState errors. Also sets Http response status code to 400, Bad Request.
        /// </summary>
        protected JsonResult JsonError(JsonRequestBehavior jsonRequestBehavior)
        {
            var modelStateErrors = ModelState.Values.Select(x => x.Errors.Select(y => y.ErrorMessage));
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Errors = modelStateErrors }, jsonRequestBehavior);
        }

        /// <summary>
        /// Creates a JSON object with an Errors property that contains your errorMessage. Also sets Http response status code to 400, Bad Request.
        /// </summary>
        protected JsonResult JsonError(string errorMessage, JsonRequestBehavior jsonRequestBehavior)
        {
            var modelStateErrors = new List<string> { errorMessage };
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Errors = modelStateErrors }, jsonRequestBehavior);
        }

        #endregion
    }
}
