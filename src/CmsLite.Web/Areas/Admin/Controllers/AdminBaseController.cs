using System.Web.Mvc;
using CmsLite.Web.Areas.Admin.Models;

namespace CmsLite.Web.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        public AdminContextModel Context { get; set; }
    }
}
