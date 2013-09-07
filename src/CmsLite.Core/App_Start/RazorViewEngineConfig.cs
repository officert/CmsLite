using System.Web.Mvc;

namespace CmsLite.Core.App_Start
{
    public static class RazorViewEngineConfig
    {
        public static void Configure()
        {
            ViewEngines.Engines.Add(new RazorViewEngine
            {
                PartialViewLocationFormats = new[]
                                      {
                                          "~/Views/{1}/{0}.cshtml",
                                          "~/Views/Shared/{0}.cshtml",
                                          "~/Areas/Admin/Views/{0}.cshtml"
                                      }
            });
        }
    }
}