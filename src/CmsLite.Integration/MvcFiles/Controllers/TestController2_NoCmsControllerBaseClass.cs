using System;
using System.Web.Mvc;
using CmsLite.Integration.MvcFiles.Models;
using CmsLite.Web.Cms.Attributes;

namespace CmsLite.Integration.MvcFiles.Controllers
{
    [CmsSectionTemplate(Name = "Home Section")]
    public class TestController2_NoCmsControllerBaseClass : Controller
    {
        //NOTE: because this controller does NOT subclass CmsController it will not be found by the FileManager

        [CmsPageTemplate(
            Name = "Home", 
            ModelType = typeof(HomeModel))]
        public ActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}
