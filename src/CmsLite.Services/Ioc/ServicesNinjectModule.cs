using CmsLite.Interfaces.Services;
using IocLite;

namespace CmsLite.Services.Ioc
{
    public class ServicesNinjectModule : Registry
    {
        public override void Load()
        {
            For<IUserService>().Use<UserService>().InHttpRequestScope();

            For<IPageNodeService>().Use<PageNodeService>().InHttpRequestScope();
            For<ISectionNodeService>().Use<SectionNodeService>().InHttpRequestScope();
            For<ISectionTemplateService>().Use<SectionTemplateService>().InHttpRequestScope();
            For<IPageTemplateService>().Use<PageTemplateService>().InHttpRequestScope();
            For<IPagePropertyTemplateService>().Use<PagePropertyTemplateService>().InHttpRequestScope();
            For<IPagePropertyService>().Use<PagePropertyService>().InHttpRequestScope();

            For<IFileService>().Use<FileService>().InHttpRequestScope();
        }
    }
}
