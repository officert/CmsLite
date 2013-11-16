using System;
using System.Linq;
using System.Web.Mvc;
using CmsLite.Core.Constants;
using CmsLite.Core.Helpers;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;

namespace CmsLite.Core
{
    public class CmsActionInvoker : ControllerActionInvoker
    {
        private readonly ISectionNodeService _sectionNodeService;
        private readonly IPageNodeService _pageNodeService;

        public CmsActionInvoker(ISectionNodeService sectionNodeService, IPageNodeService pageNodeService)
        {
            _sectionNodeService = sectionNodeService;
            _pageNodeService = pageNodeService;
        }

        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            if (controllerContext == null) return false;

            var currentRouteData = controllerContext.RouteData;

            if (currentRouteData == null) return false;

            var routeDataValues = currentRouteData.Values.ToList();

            var controllerName = currentRouteData.GetRequiredString("controller");

            var sectionNode = _sectionNodeService.GetByUrlName(controllerName);

            if (sectionNode == null) throw new ArgumentException(string.Format(Messages.SectionNodeNotFoundForUrlName, controllerName));

            PageNode pageNode = null;

            currentRouteData.Values[CmsRoutingConstants.RouteDataNameForSection] = sectionNode.UrlName;

            for (var i = 0; i < routeDataValues.Count; i++)
            {
                if (i == 0) continue;  //skip the controller route data value

                pageNode = i == 1
                            ? NodeHelper.GetActionPageNode(sectionNode.PageNodes, routeDataValues[i].Value.ToString())
                            : NodeHelper.GetActionPageNode(pageNode.PageNodes, routeDataValues[i].Value.ToString());

                currentRouteData.Values[string.Format(CmsRoutingConstants.RouteDataNameForAction + "-{0}", i)] = pageNode.UrlName;
            }

            //set the original controller and action values back to normal so MVC can find the appropriate view
            //also set new values to store the cms version of the controller and action
            currentRouteData.Values["controller"] = sectionNode.SectionTemplate.ControllerName.Replace("Controller", "");
            currentRouteData.Values["action"] = pageNode.PageTemplate.ActionName;

            return base.InvokeAction(controllerContext, pageNode.PageTemplate.ActionName);
        }
    }
}
