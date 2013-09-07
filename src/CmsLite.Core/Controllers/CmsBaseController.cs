using System;
using System.Web.Mvc;
using CmsLite.Core.Interfaces;

namespace CmsLite.Core.Controllers
{
    public class CmsBaseController : Controller
    {
        protected readonly ICmsModelHelper CmsModelHelper;

        public CmsBaseController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper)
        {
            ActionInvoker = actionInvoker;
            CmsModelHelper = cmsModelHelper;
        }

        public virtual ActionResult Index()
        {
            throw new NotImplementedException("You must provide your own implementation of the Index method.");
        }
    }
}
