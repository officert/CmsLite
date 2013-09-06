using System;
using System.Web.Mvc;
using CmsLite.Integration.MvcFiles.Models;
using CmsLite.Web.Cms;
using CmsLite.Web.Cms.Attributes;
using CmsLite.Web.Interfaces;

namespace CmsLite.Integration.MvcFiles.Controllers
{
    public class TestController3_NoCmsSectionTemplateAttr : CmsController
    {
        //NOTE: because this controller does NOT have the CmsSectionTemplate attribute it will not be found by the FileManager

        public TestController3_NoCmsSectionTemplateAttr(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper) : base(actionInvoker, cmsModelHelper)
        {
        }

        [CmsPageTemplate(
            Name = "Home",
            ModelType = typeof(HomeModel))]
        public new ActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}
