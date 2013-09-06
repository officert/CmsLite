using System.Web.Mvc;

namespace CmsLite.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Account_default",
                "Account/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
