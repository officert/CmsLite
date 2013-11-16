using CmsLite.Interfaces.Data;
using IocLite;

namespace CmsLite.Data.Ioc
{
    public class DataNinectModule : Registry
    {
        public override void Load()
        {
            For<IUnitOfWork>().Use<UnitOfWork>().InHttpRequestScope();
            For<IDbContext>().Use<CmsDbContext>().InHttpRequestScope();
        }
    }
}
