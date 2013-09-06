using System;
using System.Web.Mvc;
using CmsLite.Web.Areas.Admin.Controllers;
using CmsLite.Web.Areas.Admin.Models;

namespace CmsLite.Web.Areas.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class LayoutModelAttribute : ActionFilterAttribute
    {
        private readonly IAdminModelFactory _modelFactory;

        public LayoutModelAttribute(IAdminModelFactory modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as AdminBaseController;
            if (controller != null)
            {
                (controller).Context = _modelFactory.Create<AdminContextModel>();
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model;
            var controller = filterContext.Controller as AdminBaseController;

            var model = viewModel as AdminLayoutModel;
            if (model != null)
            {
                (model).Context = controller != null && controller.Context != null
                    ? controller.Context
                    : _modelFactory.Create<AdminContextModel>();
            }

            base.OnResultExecuting(filterContext);
        }
    }
}