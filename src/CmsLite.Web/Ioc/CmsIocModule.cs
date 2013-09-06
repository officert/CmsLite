using System.Web.Mvc;
using AutoMapper;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Content;
using CmsLite.Web.Authetication;
using CmsLite.Web.Cms;
using CmsLite.Web.Cms.Helpers;
using CmsLite.Web.Interfaces;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CmsLite.Web.Ioc
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
