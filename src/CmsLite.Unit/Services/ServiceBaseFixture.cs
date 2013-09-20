using System;
using System.Collections.Generic;
using CmsLite.Core.Ioc;
using CmsLite.Data.Ioc;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Services.Ioc;
using Ninject;
using Ninject.Modules;
using NUnit.Framework;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public abstract class ServiceBaseFixture : IDisposable
    {
        protected IKernel Kernel;
        protected IUnitOfWork UnitOfWork;
        protected ISectionTemplateService SectionTemplateService;
        protected IPageTemplateService PageTemplateService;
        protected ISectionNodeService SectionNodeService;
        protected IPageNodeService PageNodeService;
        protected IPropertyTemplateService PropertyTemplateService;
        protected IPropertyService PropertyService;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            Kernel = new StandardKernel();
            Kernel.Load(new List<INinjectModule>
                             {
                                 new CmsIocModule(),
                                 new ServicesNinjectModule(),
                                 new DataNinectModule()
                             });

            UnitOfWork = Kernel.Get<IUnitOfWork>();
            SectionTemplateService = Kernel.Get<ISectionTemplateService>();
            PageTemplateService = Kernel.Get<IPageTemplateService>();
            SectionNodeService = Kernel.Get<ISectionNodeService>();
            PageNodeService = Kernel.Get<IPageNodeService>();
            PropertyTemplateService = Kernel.Get<IPropertyTemplateService>();
            PropertyService = Kernel.Get<IPropertyService>();

            PostFixtureSetup();
        }

        protected virtual void PostFixtureSetup()
        {
        }

        public void Dispose()
        {
            if(Kernel != null) Kernel.Dispose();
        }

        protected void CleanupSectionTemplates(IEnumerable<int> sectionTemplateIds)
        {
            foreach (var sectionTemplateId in sectionTemplateIds)
            {
                SectionTemplateService.Delete(sectionTemplateId);
            }
        }
    }
}
