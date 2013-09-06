using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.TestApp.App_Start;
using CmsLite.Web.Cms;

namespace CmsLite.TestApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : CmsApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Start();
        }
    }
}