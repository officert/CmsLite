using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Resources;
using Ninject;

namespace CmsLite.Web.Ioc
{
    public class IocControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;
        private readonly IDbContext _dbContext;

        public IocControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
            _dbContext = _kernel.Get<IDbContext>();
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Type controllerType;

            var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>().Include(x => x.SectionTemplate);

            //var defaultRoute = RouteTable.Routes.OfType<Route>().FirstOrDefault(x => x.Url == "{controller}/{action}/{id}");

            //if(defaultRoute == null)
            //    throw new ArgumentException("You must provide a default route. Add a route to the MVC route table with the url : \"{controller}/{action}/{id}\"");

            //var defaultRouteControllerName = defaultRoute.Defaults["controller"].ToString().ToLower();

            if (!IsCmsSection(controllerName))
            {
                var section = sectionNodeDbSet.FirstOrDefault(x => x.UrlName == controllerName.ToLower());

                if (section == null)
                    throw new ArgumentException(string.Format(Messages.SectionNodeWithControllerNameNotFound, controllerName));  //TODO : this is where 404 handler should be plugged in

                controllerType = GetControllerType(requestContext, section.SectionTemplate.ControllerName.ToLower().Replace("controller", ""));

                if (controllerType == null)
                    throw new ArgumentException(string.Format("No controller exists the name '{0}'.", controllerName));  //TODO : this is where 404 handler should be plugged in
            }
            else
            {
                controllerType = GetControllerType(requestContext, controllerName);
            }

            return controllerType == null ? null : (IController)_kernel.Get(controllerType);
        }

        #region Private Helpers

        internal static bool IsCmsSection(string name)
        {
            if (name.ToLower() == "admin" || name.ToLower() == "account")
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}