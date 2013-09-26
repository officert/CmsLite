using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core.App_Start;
using CmsLite.Core.Interfaces;
using CmsLite.Core.Ioc;
using CmsLite.Interfaces.Templating;

namespace CmsLite.Core
{
    public class CmsModule
    {
        private ITemplateEngine _templateEngine;
        private readonly Container _container;

        public CmsModule()
        {
            _container = new Container();
        }

        public void Init()
        {
            IocConfig.Configure(_container);
            AutoMapperConfiguration.Configure();
            RazorViewEngineConfig.Configure();
            RouteConfig.RegisterRoutes(RouteTable.Routes);      //TODO: need to think more about what routes to add to a user's MVC project

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_container));                           //setup ninject as the default MVC controller factory

            _templateEngine = _container.GetInstance<ITemplateEngine>();
            _templateEngine.GenerateTemplates(Assembly.GetCallingAssembly());
        }
    }
}
