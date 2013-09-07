using System;
using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;
using CmsLite.Core.Interfaces;
using CmsLite.Integration.MvcFiles.Models;

namespace CmsLite.Integration.MvcFiles.Controllers
{
    [CmsSectionTemplate(Name = "TestController1")]
    public class TestController1_Valid : CmsBaseController
    {
        public TestController1_Valid(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper)
            : base(actionInvoker, cmsModelHelper)
        {
        }

        [CmsPageTemplate(
            Name = "About Me Page", 
            ModelType = typeof(HomeModel))]
        public ActionResult AboutMe()
        {
            throw new NotImplementedException();
        }

        [CmsPageTemplate(
            ModelType = typeof(HomeModel))]
        public ActionResult HelloWorld()
        {
            throw new NotImplementedException();
        }

        public ActionResult Foobar()                //because this method does not have the CmsPageTemplate attribute it won't be used as a PageTemplate
        {
            throw new NotImplementedException();
        }

        public void BarFoo()                        //because this method does not return ActionResult is won't be used as a PageTemplate
        {
            throw new NotImplementedException();
        }
    }
}
