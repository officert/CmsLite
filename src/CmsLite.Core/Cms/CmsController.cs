using System;
using System.Web.Mvc;
using CmsLite.Core.Interfaces;

namespace CmsLite.Core.Cms
{
    public class CmsController : Controller
    {
        protected readonly ICmsModelHelper CmsModelHelper;

        public CmsController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper)
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
