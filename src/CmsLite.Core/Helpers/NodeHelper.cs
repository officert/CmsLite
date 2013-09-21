﻿using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Domains.Interfaces;
using CmsLite.Utilities.Cms;

namespace CmsLite.Core.Helpers
{
    public static class NodeHelper
    {
        public static SectionNode GetControllerSectionNode(IEnumerable<SectionNode> sectionNodes, string urlName)
        {
            var sectionNode = sectionNodes.FirstOrDefault(x => x.UrlName == urlName.ToLower());

            if (sectionNode == null)
                throw new ArgumentException(string.Format("No section with the url name {0} found.", urlName));

            return sectionNode;
        }

        public static PageNode GetActionPageNode(INode parentNode, string urlName)
        {
            //if the action name is index use the first page for this section, otherwise find the page by action name

            var pageNode = urlName.ToLower() == "index" ? parentNode.PageNodes.FirstOrDefault(x => x.Order == CmsConstants.FirstOrderNumber) : parentNode.PageNodes.FirstOrDefault(x => x.UrlName == urlName.ToLower());

            if (pageNode == null)
                throw new ArgumentException(string.Format("No page with the url name {0} found.", urlName.ToLower()));

            return pageNode;
        }
    }
}
