using System.Web.Mvc;
using CmsLite.TestApp.Models.Pages;
using CmsLite.Web.Cms;
using CmsLite.Web.Cms.Attributes;
using CmsLite.Web.Interfaces;

namespace CmsLite.TestApp.Controllers
{
    [CmsSectionTemplate(Name = "Employees Template")]
    public class EmployeesController : CmsController
    {
        public EmployeesController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper)
            : base(actionInvoker, cmsModelHelper)
        {
        }

        [CmsPageTemplate(
            Name = "Employees Template",
            ModelType = typeof(EmployeesModel),
            AllowedChildPageTemplates = new[] { "Employee" }
        )]
        public override ActionResult Index()
        {
            var model = CmsModelHelper.GetModel<EmployeesModel>(HttpContext);
            return View(model);
        }

        [CmsPageTemplate(
            Name = "Employee Template",
            ModelType = typeof(EmployeeModel)
        )]
        public ActionResult Employee()
        {
            var model = CmsModelHelper.GetModel<EmployeeModel>(HttpContext);
            return View(model);
        }
    }
}
