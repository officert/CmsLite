using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CmsLite.Core.Constants;
using CmsLite.Core.Helpers;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;

namespace CmsLite.Core
{
    public class CmsActionInvoker : ControllerActionInvoker, IActionInvoker
    {
        private readonly IDbContext _dbContext;

        public CmsActionInvoker(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            var currentRouteData = controllerContext.RouteData;
            if (currentRouteData == null)
                throw new ArgumentException("The RouteData Collection cannot be null.");

            var routeDataValues = currentRouteData.Values.ToList();

            var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>()
                                .Include(x => x.SectionTemplate)
                                .Include(x => x.PageNodes.Select(y => y.PageTemplate));

            var sectionNode = NodeHelper.GetControllerSectionNode(sectionNodeDbSet, routeDataValues[0].Value.ToString());
            PageNode pageNode = null;

            currentRouteData.Values[CmsRoutingConstants.RouteDataNameForSection] = sectionNode.UrlName;

            for (var i = 0; i < routeDataValues.Count; i++)
            {
                if (i == 0) continue;  //skip the controller route data value

                pageNode = i == 1
                            ? NodeHelper.GetActionPageNode(sectionNode, routeDataValues[i].Value.ToString())
                            : NodeHelper.GetActionPageNode(pageNode, routeDataValues[i].Value.ToString());

                currentRouteData.Values[string.Format(CmsRoutingConstants.RouteDataNameForAction + "{0}", i)] = pageNode.UrlName;
            }

            //set the original controller and action values back to normal so MVC can find the appropriate view
            //also set new values to store the cms version of the controller and action
            currentRouteData.Values["controller"] = sectionNode.SectionTemplate.ControllerName.Replace("Controller", "");
            currentRouteData.Values["action"] = pageNode.PageTemplate.ActionName;

            //PopulateRouteDataWithCmsValues(currentRouteData, sectionNode, pageNode);

            return base.InvokeAction(controllerContext, pageNode.PageTemplate.ActionName);
        }

        //private static void PopulateRouteDataWithCmsValues(RouteData routeData, SectionNode sectionNode, PageNode pageNode)
        //{
        //    var routeDataValues = routeData.Values.ToList();

        //    for (var i = 0; i < routeDataValues.Count; i++)
        //    {
        //        var routeValue = routeDataValues[i].Value.ToString();

        //        routeData.Values[CmsConstants.RouteDataKey] = string.Format(CmsConstants.RouteDataPrefix, i, sectionNode.UrlName);
        //    }
        //}
    }
}
