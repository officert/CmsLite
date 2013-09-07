using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Integration.Services
{
    [TestFixture]
    [Category("Integration")]
    public class PropertyTemplateServiceFixture : ServiceBaseFixture
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

        #region Create

        [Test]
        public void Create_NoPropertyName_ThrowsException()
        {
            var propertyName = string.Empty;
            Assert.That(() => PropertyTemplateService.Create(0, propertyName, CmsPropertyType.RichTextEditor, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PropertyTemplatePropertyNameCannotBeNull));
        }

        [Test]
        public void Create_NoPageTemplateExistsForId_ThrowsException()
        {
            //arrange
            const int pageTemplateId = 99999;

            Assert.That(() => PropertyTemplateService.Create(pageTemplateId, "Foobar", CmsPropertyType.RichTextEditor, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void Create_DisplayNameIsNullOrEmpty_UsesPropertyNameAsDisplayName()
        {
            //arrange
            var displayName = string.Empty;
            const string propertyName = "FoobarProperty";

            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

            //act
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, propertyName, CmsPropertyType.ImagePicker, 0, displayName: displayName);

            //assert
            propertyTemplate.DisplayName.Should().Be.EqualTo(propertyName);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_TabOrderIsNull_Uses1AsTabOrder()
        {
            //arrange
            int? tabOrder = null;

            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

            //act
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, tabOrder);

            //assert
            propertyTemplate.TabOrder.Should().Be.EqualTo(1);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_TabNameIsNullOrEmpty_UsesDefaultTabName()
        {
            //arrange
            var tabName = string.Empty;

            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");

            //act
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null, tabName);

            //assert
            propertyTemplate.TabName.Should().Be.EqualTo(CmsConstants.PropertyTemplateDefaultTabName);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_ParentPageTemplateHasPageNodes_AddNewPropertyToEachPageNodeUsingPropertyTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //act
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null);

            //assert
            var updatedPageNode = PageNodeService.FindIncluding(x => x.Properties).FirstOrDefault(x => x.Id == pageNode.Id);
            updatedPageNode.Properties.Count.Should().Be.EqualTo(1);
            updatedPageNode.Properties.First().PropertyTemplateId.Should().Be.EqualTo(propertyTemplate.Id);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_PropertyTemplateNotFound_ThrowsException()
        {
            const int propertyTemplateId = 99999999;
            Assert.That(() => PropertyTemplateService.Delete(propertyTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PropertyTemplateNotFound, propertyTemplateId)));
        }

        [Test]
        public void Delete_DeletesThePropertyTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.Number, null);
            //act
            PropertyTemplateService.Delete(propertyTemplate.Id);

            //assert
            var deletedPropertyTemplate = UnitOfWork.Context.GetDbSet<PropertyTemplate>().FirstOrDefault(x => x.Id == propertyTemplate.Id);
            deletedPropertyTemplate.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_DeletesAnyPropertiesThatUseThePropertyTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "Foobar");
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.Number, null);
            //act
            PropertyTemplateService.Delete(propertyTemplate.Id);

            //assert
            var propertiesForDeletedPropertyTemplate = UnitOfWork.Context.GetDbSet<Property>().Where(x => x.PropertyTemplateId == propertyTemplate.Id);
            propertiesForDeletedPropertyTemplate.Should().Be.Empty();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion
    }
}
