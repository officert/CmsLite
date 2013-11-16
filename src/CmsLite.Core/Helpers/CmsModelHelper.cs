using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CmsLite.Core.Attributes;
using CmsLite.Core.Constants;
using CmsLite.Core.Extensions;
using CmsLite.Core.Interfaces;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;

namespace CmsLite.Core.Helpers
{
    public class CmsModelHelper : ICmsModelHelper
    {
        private readonly ISectionNodeService _sectionNodeService;

        public CmsModelHelper(ISectionNodeService sectionNodeService)
        {
            _sectionNodeService = sectionNodeService;
        }

        public T GetModel<T>(HttpContextBase httpContextBase) where T : class , new()
        {
            var currentRouteData = httpContextBase.Request.RequestContext.RouteData;

            if (currentRouteData == null)
                throw new ArgumentException("The RouteData Collection cannot be null.");

            var routeDataValues = currentRouteData.Values.ToList();

            var cmsRouteDataValues = GetCmsRouteDataValues(routeDataValues);

            var controllerName = cmsRouteDataValues[0].Value.ToString();

            var sectionNode = _sectionNodeService.GetByUrlName(controllerName);

            if (sectionNode == null) throw new ArgumentException(string.Format(Messages.SectionNodeNotFoundForUrlName, controllerName));

            PageNode pageNode = null;

            for (var i = 0; i < cmsRouteDataValues.Count; i++)
            {
                if (i == 0) continue;  //skip the controller route data value

                pageNode = i == 1
                            ? NodeHelper.GetActionPageNode(sectionNode, cmsRouteDataValues[i].Value.ToString())
                            : NodeHelper.GetActionPageNode(pageNode, cmsRouteDataValues[i].Value.ToString());

                if (pageNode == null)
                    throw new ArgumentException(string.Format("The page with the url name : {0} was not found.", cmsRouteDataValues[i].Value));
            }

            var modelName = pageNode.PageTemplate.ModelName;

            var modelType = Assembly.GetCallingAssembly().GetModels().FirstOrDefault(x => x.Name == modelName);
            var modelInstance = (T) Activator.CreateInstance(modelType);

            var props = modelType.GetMembers();
            var modelProperties = props.Where(x => x.GetCustomAttributes(typeof(CmsModelPropertyAttribute), false).Length > 0).OfType<PropertyInfo>().ToList();

            foreach (var property in pageNode.Properties)
            {
                var modelProperty = modelProperties.FirstOrDefault(x => x.Name == property.PropertyTemplate.PropertyName);
                if(modelProperty != null)
                {
                    modelProperty.SetValue(modelInstance, property.Text, BindingFlags.Public, null, null, null);
                }
            }

            return modelInstance;
        }

        private static List<KeyValuePair<string, object>> GetCmsRouteDataValues(IEnumerable<KeyValuePair<string, object>> routeDataValues)
        {
            var cmsRouteDataValues = new List<KeyValuePair<string, object>>();

            cmsRouteDataValues = routeDataValues.Where(x => x.Key == CmsRoutingConstants.RouteDataNameForSection).ToList();

            cmsRouteDataValues.AddRange(routeDataValues.Where(x => x.Key.StartsWith(CmsRoutingConstants.RouteDataNameForAction)).OrderBy(x => x.Key));

            return cmsRouteDataValues;
        }
    }

    //internal struct CmsRouteData
    //{
    //    public string ControllerName { get; set; }
    //    public string ActionName { get; set; }
    //}
}
