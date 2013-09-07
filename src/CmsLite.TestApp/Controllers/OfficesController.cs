using System.Web.Mvc;
using CmsLite.Core.Cms;
using CmsLite.Core.Cms.Attributes;
using CmsLite.Core.Interfaces;
using CmsLite.TestApp.Models.Pages;

namespace CmsLite.TestApp.Controllers
{
    [CmsSectionTemplate(Name = "Offices Template")]
    public class OfficesController : CmsController
    {
        public OfficesController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper) : base(actionInvoker, cmsModelHelper)
        {
        }

        [CmsPageTemplate(
            Name = "Offices Template",
            ModelType = typeof(OfficesModel),
            AllowedChildPageTemplates = new[] { "Office" }
        )]
        public override ActionResult Index()
        {
            var model = CmsModelHelper.GetModel<OfficesModel>(HttpContext);
            return View(model);
        }

        [CmsPageTemplate(
            Name = "Office Template",
            ModelType = typeof(OfficeModel),
            IconImageName = "office.png"
        )]
        public ActionResult Office()
        {
            var model = CmsModelHelper.GetModel<OfficeModel>(HttpContext);
            return View(model);
        }
    }
}
