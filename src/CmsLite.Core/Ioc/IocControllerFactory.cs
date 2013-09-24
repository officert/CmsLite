using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core.Interfaces;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Resources;

namespace CmsLite.Core.Ioc
{
    public class IocControllerFactory : DefaultControllerFactory
    {
        private readonly Container _container;
        private readonly IDbContext _dbContext;

        public IocControllerFactory(Container container)
        {
            _container = container;
            _dbContext = _container.GetInstance<IDbContext>();
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>().Include(x => x.SectionTemplate);

            var controllerType = GetControllerType(requestContext, controllerName);

            if (controllerType == null) return null;

            if (IsDefinedInCurrentAssembly(controllerType))
            {
                return GetControllerInstance(requestContext, controllerType);
            }

            var section = sectionNodeDbSet.FirstOrDefault(x => x.UrlName == controllerName.ToLower());

            if (section == null)
                throw new ArgumentException(string.Format(Messages.SectionNodeWithControllerNameNotFound, controllerName));  //TODO : this is where 404 handler should be plugged in

            var cmsControllerType = GetControllerType(requestContext, section.SectionTemplate.ControllerName.ToLower().Replace("controller", ""));

            if (cmsControllerType == null)
                throw new ArgumentException(string.Format("No controller exists the name '{0}'.", controllerName));  //TODO : this is where 404 handler should be plugged in

            return GetControllerInstance(requestContext, cmsControllerType);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                       ? null
                       : (IController)_container.Get(controllerType);
        }

        #region Private Helpers

        internal static bool IsDefinedInCurrentAssembly(Type controllerType)
        {
            var currentAssemblyName = Assembly.GetExecutingAssembly().FullName;
            var clientAssemblyName = controllerType.Assembly.FullName;

            return currentAssemblyName == clientAssemblyName;
        }

        #endregion
    }
}