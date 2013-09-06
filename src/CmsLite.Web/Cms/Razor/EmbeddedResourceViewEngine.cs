using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsLite.Web.Cms.Razor
{
    public class EmbeddedResourceViewEngine : RazorViewEngine
    {
        public EmbeddedResourceViewEngine()
        {
            ViewLocationFormats = new[]
                                      {
                                          "~/Views/{1}/{0}.cshtml",
                                          "~/Views/Shared/{0}.cshtml",
                                          "~/Areas/Admin/Views/{0}.cshtml"
                                      };    
            PartialViewLocationFormats = ViewLocationFormats;
        }
    }
}