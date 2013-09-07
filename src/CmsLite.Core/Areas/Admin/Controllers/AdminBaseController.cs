using System.Web.Mvc;
using CmsLite.Core.Areas.Admin.Models;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        public AdminContextModel Context { get; set; }
    }
}
