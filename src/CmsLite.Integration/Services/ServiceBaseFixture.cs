using System;
using System.Collections.Generic;
using CmsLite.Core.Ioc;
using CmsLite.Data.Ioc;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Services.Ioc;
using IocLite;
using IocLite.Interfaces;
using NUnit.Framework;

namespace CmsLite.Integration.Services
{
    [TestFixture]
    [Category("Integration")]
    public abstract class ServiceBaseFixture : IDisposable
    {
        protected IContainer Container;
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
            Container = new Container();
            Container.Register(new List<IRegistry>
                             {
                                 new CmsIocModule(),
                                 new ServicesNinjectModule(),
                                 new DataNinectModule()
                             });


            UnitOfWork = Container.Resolve<IUnitOfWork>();
            SectionTemplateService = Container.Resolve<ISectionTemplateService>();
            PageTemplateService = Container.Resolve<IPageTemplateService>();
            SectionNodeService = Container.Resolve<ISectionNodeService>();
            PageNodeService = Container.Resolve<IPageNodeService>();
            PropertyTemplateService = Container.Resolve<IPropertyTemplateService>();
            PropertyService = Container.Resolve<IPropertyService>();

            PostFixtureSetup();
        }

        protected virtual void PostFixtureSetup()
        {
        }

        public void Dispose()
        {
            //if(Container != null) Container.Dispose();
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
