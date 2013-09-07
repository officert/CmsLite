using System;
using System.Web.Mvc;
using CmsLite.Core.Cms.Attributes;
using CmsLite.Integration.MvcFiles.Models;

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
