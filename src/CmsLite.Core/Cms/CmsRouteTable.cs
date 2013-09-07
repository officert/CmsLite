using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Routing;

namespace CmsLite.Core.Cms
{
    public static class CmsRouteTable
    {
        public static ICollection<Route> Routes = new Collection<Route>();

        public static void MapRoute(string url, RouteValueDictionary defaults = null)
        {
            Routes.Add(new Route(url, defaults, null));
        }
    }
}
