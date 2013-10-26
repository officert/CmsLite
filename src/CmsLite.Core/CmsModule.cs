using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core.App_Start;
using CmsLite.Core.Ioc;
using CmsLite.Interfaces.Templating;
using MenuGen.Ioc;
using Ninject;

namespace CmsLite.Core
{
    public class CmsModule
    {
        private ITemplateEngine _templateEngine;
        private readonly IKernel _kernel;

        public CmsModule()
        {
            _kernel = new StandardKernel();
        }

        public void Init()
        {
            IocConfig.Configure(_kernel);
            AutoMapperConfiguration.Configure();
            RazorViewEngineConfig.Configure();
            RouteConfig.RegisterRoutes(RouteTable.Routes);      //TODO: need to think more about what routes to add to a user's MVC project

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_kernel));                           //setup ninject as the default MVC controller factory

            _templateEngine = _kernel.Get<ITemplateEngine>();
            _templateEngine.GenerateTemplates(Assembly.GetCallingAssembly());

            var menuGen = new MenuGen.MenuGen();
            menuGen.Init(x =>
            {
                //x.ContainerAdapter = new NinjectContainerAdapter(_kernel);
            });
        }
    }

    internal class NinjectContainerAdapter : IContainerAdapter
    {
        private readonly IKernel _kernel;

        public NinjectContainerAdapter(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object GetInstance(System.Type type)
        {
            return _kernel.Get(type);
        }

        public IEnumerable<object> GetInstances(System.Type type)
        {
            return _kernel.GetAll(type);
        }
    }
}
