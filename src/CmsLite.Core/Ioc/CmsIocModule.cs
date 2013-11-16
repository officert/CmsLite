using System.Web.Mvc;
using AutoMapper;
using CmsLite.Core.Authetication;
using CmsLite.Core.Helpers;
using CmsLite.Core.Interfaces;
using CmsLite.Core.Templating;
using CmsLite.Interfaces.Authentication;
using CmsLite.Interfaces.Templating;
using IocLite;

namespace CmsLite.Core.Ioc
{
    public class CmsIocModule : Registry
    {
        public override void Load()
        {
            For<ICmsLiteHttpContext>().Use<CmsHttpContext>().InHttpRequestScope();

            For<IAuthenticationProvider>().Use<SimpleAuthenticationProvider>().InSingletonScope();

            For<IMappingEngine>().Use(Mapper.Engine);

            For<ITemplateEngine>().Use<TemplateEngine>().InHttpRequestScope();

            For<IActionInvoker>().Use<CmsActionInvoker>().InHttpRequestScope();

            For<ICmsModelHelper>().Use<CmsModelHelper>().InHttpRequestScope();
        }
    }
}
