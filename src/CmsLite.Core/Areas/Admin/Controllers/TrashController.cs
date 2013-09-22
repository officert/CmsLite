using System.Web.Mvc;

namespace CmsLite.Core.Areas.Admin.Controllers
{
    public class TrashController : AdminBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
