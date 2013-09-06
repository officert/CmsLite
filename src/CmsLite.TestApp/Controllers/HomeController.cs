using System.Web.Mvc;
using CmsLite.TestApp.Models.Pages;
using CmsLite.Web.Cms;
using CmsLite.Web.Cms.Attributes;
using CmsLite.Web.Interfaces;

namespace CmsLite.TestApp.Controllers
{
    [CmsSectionTemplate(Name = "Home Template")]
    public class HomeController : CmsController
    {
        public HomeController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper) : base(actionInvoker, cmsModelHelper)
        {
        }

        [CmsPageTemplate(
            Name = "Home Template",
            ModelType = typeof(HomeModel)
        )]
        public override ActionResult Index()
        {
            var model = CmsModelHelper.GetModel<HomeModel>(HttpContext);
            return View(model);
        }
    }
}
