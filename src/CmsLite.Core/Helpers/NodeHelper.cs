using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Utilities;
using CmsLite.Utilities.Cms;

namespace CmsLite.Core.Helpers
{
    public static class NodeHelper
    {
        public static PageNode GetActionPageNode(IEnumerable<PageNode> pageNodes, string urlName)
        {
            Ensure.ArgumentIsNotNull(pageNodes, "nodes");
            Ensure.ArgumentIsNotNullOrEmpty(urlName, "urlName");

            //if the action name is index use the first page for this section, otherwise find the page by action name

            var pageNode = urlName.ToLower() == "index" ? pageNodes.FirstOrDefault(x => x.Order == CmsConstants.FirstOrderNumber) : pageNodes.FirstOrDefault(x => x.UrlName == urlName.ToLower());

            if (pageNode == null)
                throw new ArgumentException(string.Format("No page with the url name {0} found.", urlName.ToLower()));

            return pageNode;
        }
    }
}
