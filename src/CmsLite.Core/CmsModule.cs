using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core.App_Start;
using CmsLite.Core.Ioc;
using CmsLite.Data.Ioc;
using CmsLite.Interfaces.Templating;
using CmsLite.Services.Ioc;
using IocLite;
using IocLite.Interfaces;
using IContainer = IocLite.Interfaces.IContainer;

namespace CmsLite.Core
{
    public class CmsModule
    {
        private ITemplateEngine _templateEngine;
        private readonly IContainer _container;

        public CmsModule()
        {
            _container = new Container();
        }

        public void Init()
        {
            var cmsRegistry = new CmsIocModule();
            var dataRegistry = new DataNinectModule();

            _container.Register(new List<IRegistry>
            {
                cmsRegistry,
                new ServicesNinjectModule(),
                dataRegistry
            });
            MenuGen.MenuGen.Init(x => x.Container.Register(new List<IRegistry>
            {
                //dataRegistry
            }));

            //IocConfig.Configure(_container);
            AutoMapperConfiguration.Configure();
            RazorViewEngineConfig.Configure();
            RouteConfig.RegisterRoutes(RouteTable.Routes);      //TODO: need to think more about what routes to add to a user's MVC project

            _templateEngine = (ITemplateEngine)_container.Resolve<ITemplateEngine>();
            _templateEngine.GenerateTemplates(Assembly.GetCallingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_container));
        }
    }

    //internal class NinjectContainerAdapter : IContainerAdapter
    //{
    //    private readonly IKernel _kernel;

    //    public NinjectContainerAdapter(IKernel kernel)
    //    {
    //        _kernel = kernel;
    //    }

    //    public object TryResolve(Type type)
    //    {
    //        return _kernel.TryGet(type);
    //    }
    //}
}
