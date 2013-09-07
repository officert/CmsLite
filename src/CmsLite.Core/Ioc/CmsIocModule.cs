using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Authetication;
using CmsLite.Core.Cms;
using CmsLite.Core.Cms.Helpers;
using CmsLite.Core.Interfaces;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Content;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CmsLite.Core.Ioc
{
    public class CmsIocModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICmsLiteHttpContext>().To<CmsLiteHttpContext>().InRequestScope();
            Bind<IAuthentication>().To<CmsLiteAuthenticationProvider>().InSingletonScope();
            Bind<IMappingEngine>().ToMethod(ctx => Mapper.Engine);
            Bind<IFileManager>().To<FileManager>();

            Bind<IActionInvoker>().To<CmsActionInvoker>();

            Bind<ICmsModelHelper>().To<ModelHelper>();
        }
    }
}
