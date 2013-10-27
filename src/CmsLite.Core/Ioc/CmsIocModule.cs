using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Authetication;
using CmsLite.Core.Helpers;
using CmsLite.Core.Interfaces;
using CmsLite.Core.Templating;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Templating;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CmsLite.Core.Ioc
{
    public class CmsIocModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICmsLiteHttpContext>().To<CmsHttpContext>().InRequestScope();
            Bind<IAuthenticationProvider>().To<SimpleAuthenticationProvider>().InSingletonScope();
            Bind<IMappingEngine>().ToMethod(ctx => Mapper.Engine);
            Bind<IMappingEngine>().ToMethod(ctx => Mapper.Engine);

            Bind<ITemplateEngine>().To<TemplateEngine>();

            Bind<IActionInvoker>().To<CmsActionInvoker>();

            Bind<ICmsModelHelper>().To<CmsModelHelper>();
        }
    }
}
