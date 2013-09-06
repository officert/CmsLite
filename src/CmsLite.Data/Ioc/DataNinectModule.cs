using CmsLite.Interfaces.Data;
using Ninject.Modules;
using Ninject.Web.Common;

namespace CmsLite.Data.Ioc
{
    public class DataNinectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>().InSingletonScope();
            Bind<IDbContext>().To<CmsDbContext>().InRequestScope();
        }
    }
}
