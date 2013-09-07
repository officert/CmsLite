using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;
using CmsLite.Core.Interfaces;
using CmsLite.TestApp.Models.Pages;

namespace CmsLite.TestApp.Controllers
{
    [CmsSectionTemplate(Name = "Employees Template")]
    public class EmployeesController : CmsBaseController
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
