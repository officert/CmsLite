//using System;
//using System.Collections.Generic;
//using System.Linq;
//using CmsLite.Domains.Entities;
//using CmsLite.Resources;
//using CmsLite.Utilities.Cms;
//using NUnit.Framework;
//using SharpTestsEx;

//namespace CmsLite.Integration.Services
//{
//    [TestFixture]
//    [Category("Integration")]
//    public class SectionTemplateServiceFixture : ServiceBaseFixture
//    {
//        private List<int> _createdSectionTemplateIds;

//        protected override void PostFixtureSetup()
//        {
//            _createdSectionTemplateIds = new List<int>();
//        }

//        [TestFixtureTearDown]
//        public void FixtureTearDown()
//        {
//            Dispose();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            CleanupSectionTemplates(_createdSectionTemplateIds);
//            _createdSectionTemplateIds.Clear();
//        }

//        #region Delete Tests

//        /// <summary>
//        /// This test verifies that the SQL cascade delete on page template's FK to section template is working
//        /// </summary>
//        [Test]
//        public void Delete_DeletesPageTemplatesThatBelongToTheSectionTemplate()
//        {
//            //arrange
//            var sectionTemplate = SectionTemplateService.Create("Foobar");
//            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

//            //act
//            SectionTemplateService.Delete(sectionTemplate.Id);

//            //assert
//            var createdSectionTemplate = UnitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplate.Id);

//            var createdPageTemplate = UnitOfWork.Context.GetDbSet<PageTemplate>().FirstOrDefault(x => x.Id == pageTemplate.Id);

//            createdSectionTemplate.Should().Be.Null();
//            createdPageTemplate.Should().Be.Null();
//        }

//        /// <summary>
//        /// This test verifies that the SQL cascade delete on section node's FK to section template is working
//        /// </summary>
//        [Test]
//        public void Delete_DeletesSectionNodesThatUseTheSectionTemplate()
//        {
//            //arrange
//            var sectionTemplate = SectionTemplateService.Create("FooBar");
//            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");

//            //act
//            SectionTemplateService.Delete(sectionTemplate.Id);

//            //assert
//            var createdSectionTemplate = UnitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplate.Id);
//            var createdSectionNode = UnitOfWork.Context.GetDbSet<SectionNode>().FirstOrDefault(x => x.Id == sectionNode.Id);

//            createdSectionTemplate.Should().Be.Null();
//            createdSectionNode.Should().Be.Null();
//        }

//        #endregion
//    }
//}
