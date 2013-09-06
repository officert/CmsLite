using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using CmsLite.Domains.Entities;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Integration.Services
{
    [TestFixture]
    public class PageTemplateServiceFixture : ServiceBaseFixture
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

        #region CreateForSectionTemplate

        [Test]
        public void CreateForSectionTemplate_NoActionName_ThrowsException()
        {
            var actionName = string.Empty;
            Assert.That(() => PageTemplateService.CreateForSectionTemplate(0, actionName, ""),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateActionNameCannotBeNull));
        }

        [Test]
        public void CreateForSectionTemplate_NoModelName_ThrowsException()
        {
            const string actionName = "foobar";
            var modelName = string.Empty;
            Assert.That(() => PageTemplateService.CreateForSectionTemplate(0, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateModelNameCannotBeNull));
        }

        [Test]
        public void CreateForSectionTemplate_NoSectionTemplateExistsForId_ThrowsException()
        {
            const int sectionTemplateId = 999999;
            const string actionName = "Foobar";
            const string modelName = "FoobarModel";

            Assert.That(() => PageTemplateService.CreateForSectionTemplate(sectionTemplateId, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void CreateForSectionTemplate_NameIsNullOrEmpty_UsesActionNameAsName()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("PageTemplateServiceFixtute", "Foobar");
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;

            //act
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, actionName, "Foobar", name);

            //assert
            pageTemplate.Name.Should().Be.EqualTo(actionName);
            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSectionTemplate_IconImageNameIsNullOrEmpty_UsesDefaultPageTemplateImage()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("PageTemplateServiceFixtute", "Foobar");
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            var iconImageName = string.Empty;

            //act
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, actionName, "FoobarModel", name, iconImageName);

            //assert
            pageTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.PageTemplateDefaultThumbnail);
            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region CreateForPageTemplate

        [Test]
        public void CreateForPageTemplate_NoActionName_ThrowsException()
        {
            var actionName = string.Empty;
            Assert.That(() => PageTemplateService.CreateForPageTemplate(0, actionName, ""),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateActionNameCannotBeNull));
        }

        [Test]
        public void CreateForPageTemplate_NoModelName_ThrowsException()
        {
            const string actionName = "foobar";
            var modelName = string.Empty;
            Assert.That(() => PageTemplateService.CreateForPageTemplate(0, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateModelNameCannotBeNull));
        }

        [Test]
        public void CreateForPageTemplate_NoPageTemplateExistsForId_ThrowsException()
        {
            const int pageTemplateId = 999999;
            const string actionName = "Foobar";
            const string modelName = "FoobarModel";

            Assert.That(() => PageTemplateService.CreateForPageTemplate(pageTemplateId, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void CreateForPageTemplate_NameIsNullOrEmpty_UsesActionNameAsName()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("PageTemplateServiceFixtute", "Foobar");
            var parentPageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;

            //act
            var pageTemplate = PageTemplateService.CreateForPageTemplate(parentPageTemplate.Id, actionName, "Foobar", name);

            //assert
            pageTemplate.Name.Should().Be.EqualTo(actionName);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPageTemplate_IconImageNameIsNullOrEmpty_UsesDefaultPageTemplateImage()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("PageTemplateServiceFixtute", "Foobar");
            var parentPageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            var iconImageName = string.Empty;

            //act
            var pageTemplate = PageTemplateService.CreateForPageTemplate(parentPageTemplate.Id, actionName, "FoobarModel", name, iconImageName);

            //assert
            pageTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.PageTemplateDefaultThumbnail);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_PageTemplateNotFound_ThrowsException()
        {
            const int pageTemplateId = 99999999;
            Assert.That(() => PageTemplateService.Delete(pageTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void Delete_PageTemplateIsNullThrowsException()
        {
            PageTemplate pageTemplate = null;

            Assert.That(() => PageTemplateService.Delete(pageTemplate),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateCannotBeNull));
        }

        [Test]
        public void Delete_DeletesThePageTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

            //act
            PageTemplateService.Delete(pageTemplate.Id);

            //assert
            var deletedPageTemplate = UnitOfWork.Context.GetDbSet<PageTemplate>().FirstOrDefault(x => x.Id == pageTemplate.Id);
            deletedPageTemplate.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_DeletesAnyPropertyTemplatesThatBelongToTheThePageTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null);

            //act
            PageTemplateService.Delete(pageTemplate.Id);

            //assert
            var deletedPropertyTemplate = UnitOfWork.Context.GetDbSet<PropertyTemplate>().FirstOrDefault(x => x.Id == propertyTemplate.Id);
            deletedPropertyTemplate.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_DeletesAnyPageNodesThatUseThePageTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var section = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");
            var pageNode = PageNodeService.CreateForSection(section.Id, pageTemplate.Id, "Foobar", "foobar");

            //act
            PageTemplateService.Delete(pageTemplate.Id);

            //assert
            var deletedPageNode = UnitOfWork.Context.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == pageNode.Id);
            deletedPageNode.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion
    }
}
