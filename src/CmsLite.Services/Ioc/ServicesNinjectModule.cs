using CmsLite.Interfaces.Services;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CmsLite.Services.Ioc
{
    public class ServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>().InRequestScope();

            Bind<IPageNodeService>().To<PageNodeService>().InRequestScope();
            Bind<ISectionNodeService>().To<SectionNodeService>();
            Bind<ISectionTemplateService>().To<SectionTemplateService>().InRequestScope();
            Bind<IPageTemplateService>().To<PageTemplateService>().InRequestScope();
            Bind<IPropertyTemplateService>().To<PropertyTemplateService>().InRequestScope();
            Bind<IPropertyService>().To<PropertyService>().InRequestScope();

            Bind<IMediaService>().To<MediaService>().InRequestScope();
        }
    }
}
