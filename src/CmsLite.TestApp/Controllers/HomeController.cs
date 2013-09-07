using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;
using CmsLite.Core.Interfaces;
using CmsLite.TestApp.Models.Pages;

namespace CmsLite.TestApp.Controllers
{
    [CmsSectionTemplate(Name = "Home Template")]
    public class HomeController : CmsBaseController
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
