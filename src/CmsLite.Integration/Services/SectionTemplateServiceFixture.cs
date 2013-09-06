using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using Ninject;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Integration.Services
{
    [TestFixture]
    public class SectionTemplateServiceFixture : ServiceBaseFixture
    {
        private List<int> _createdSectionTemplateIds;

        protected override void PostFixtureSetup()
        {
            _createdSectionTemplateIds = new List<int>();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            CleanupSectionTemplates(_createdSectionTemplateIds);
            _createdSectionTemplateIds.Clear();
        }

        #region Create Tests

        [Test]
        public void Create_NoControllerName_ThrowsException()
        {
            var controllerName = string.Empty;
            Assert.That(() => SectionTemplateService.Create(controllerName, "Test", "Foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.SectionTemplateControllerNameCannotBeNull));
        }

        [Test]
        public void Create_SectionTemplateWithControllerNameAlreadyExists_ThrowsException()
        {
            const string controllerName = "Foobar";
            var sectionTemplate = SectionTemplateService.Create(controllerName);

            Assert.That(() => SectionTemplateService.Create(controllerName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateControllerNameMustBeUnique, controllerName)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_NameIsNullOrEmpty_UsesControllerNameAsName()
        {
            //arrange
            var name = string.Empty;
            const string controllerName = "SectionTemplateServiceFixtureController";

            //act
            var sectionTemplate = SectionTemplateService.Create(controllerName, name, "");

            //assert
            sectionTemplate.Name.Should().Be.EqualTo(controllerName);
            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_IconImageNameIsNullOrEmpty_UsesDefaultSectionTemplateImage()
        {
            //arrange
            var name = string.Empty;
            const string controllerName = "SectionTemplateServiceFixtureController";
            var iconImageName = string.Empty;

            //act
            var sectionTemplate = SectionTemplateService.Create(controllerName, name, iconImageName);

            //assert
            sectionTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.SectionTemplateDefaultThumbnail);
            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete Tests

        [Test]
        public void Delete_SectionTemplateNotFound_ThrowsException()
        {
            const int sectionTemplateId = 99999999;
            Assert.That(() => SectionTemplateService.Delete(sectionTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void Delete_DeletesTheSectionTemplate()
        {
            //arrange
            const string controllerName = "SectionTemplateServiceFixtureController";
            var sectionTemplate = SectionTemplateService.Create(controllerName, "");

            //act
            SectionTemplateService.Delete(sectionTemplate.Id);

            //assert
            var deletedSectionTemplate = UnitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplate.Id);
            deletedSectionTemplate.Should().Be.Null();
        }

        /// <summary>
        /// This test verifies that the SQL cascade delete on page template's FK to section template is working
        /// </summary>
        [Test]
        public void Delete_DeletesPageTemplatesThatBelongToTheSectionTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

            //act
            SectionTemplateService.Delete(sectionTemplate.Id);

            //assert
            var createdSectionTemplate = UnitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplate.Id);

            var createdPageTemplate = UnitOfWork.Context.GetDbSet<PageTemplate>().FirstOrDefault(x => x.Id == pageTemplate.Id);

            createdSectionTemplate.Should().Be.Null();
            createdPageTemplate.Should().Be.Null();
        }

        /// <summary>
        /// This test verifies that the SQL cascade delete on section node's FK to section template is working
        /// </summary>
        [Test]
        public void Delete_DeletesSectionNodesThatUseTheSectionTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("FooBar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");

            //act
            SectionTemplateService.Delete(sectionTemplate.Id);

            //assert
            var createdSectionTemplate = UnitOfWork.Context.GetDbSet<SectionTemplate>().FirstOrDefault(x => x.Id == sectionTemplate.Id);
            var createdSectionNode = UnitOfWork.Context.GetDbSet<SectionNode>().FirstOrDefault(x => x.Id == sectionNode.Id);

            createdSectionTemplate.Should().Be.Null();
            createdSectionNode.Should().Be.Null();
        }

        #endregion
    }
}
