using System.Collections.Generic;
using CmsLite.Core.Ioc;
using CmsLite.Data.Ioc;
using CmsLite.Services.Ioc;
using Ninject;
using Ninject.Modules;

namespace CmsLite.Core.App_Start
{
    public static class IocConfig
    {
        public static void Configure(IKernel kernel)
        {
            kernel.Load(new List<INinjectModule>
                             {
                                 new CmsIocModule(),
                                 new ServicesNinjectModule(),
                                 new DataNinectModule()
                             });
        }
    }
}