﻿using System;
using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;
using CmsLite.Core.Interfaces;
using CmsLite.Integration.MvcFiles.Models;

namespace CmsLite.Integration.MvcFiles.Controllers
{
    public class TestController3NoCmsBaseSectionTemplateAttr : CmsBaseController
    {
        //NOTE: because this controller does NOT have the CmsSectionTemplate attribute it will not be found by the FileManager

        public TestController3NoCmsBaseSectionTemplateAttr(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper) : base(actionInvoker, cmsModelHelper)
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
