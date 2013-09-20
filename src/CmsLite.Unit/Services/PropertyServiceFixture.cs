using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public class PropertyServiceFixture : ServiceBaseFixture
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
        public void Create_NoPageNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 99999;

            //act + assert
            Assert.That(() => PropertyService.Create(pageNodeId, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void Create_NoPropertyTemplateExistsOnPageTemplateForId_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");
            const int propertyTemplateId = 99999;

            //act + assert
            Assert.That(() => PropertyService.Create(pageNode.Id, propertyTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PropertyTemplateNotFoundForPageTemplate, propertyTemplateId, pageTemplate.Id)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_PageNodeIsNull_ThrowsException()
        {
            //arrange
            PageNode pageNode = null;

            //act + assert
            Assert.That(() => PropertyService.Create(pageNode, null),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeCannotBeNull));
        }

        [Test]
        public void Create_PropertyTemplateIsNull_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            PropertyTemplate propertyTemplate = null;

            //act + assert
            Assert.That(() => PropertyService.Create(pageNode, propertyTemplate),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PropertyTemplateCannotBeNull));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_OtherPropertiesExistOnPageNode_OrderIsLastProperty()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null);
            var property1 = PropertyService.Create(pageNode.Id, propertyTemplate.Id);

            //act
            var property2 = PropertyService.Create(pageNode.Id, propertyTemplate.Id);

            //assert
            property2.Order.Should().Be.EqualTo(2);     //because creating a new property template automatically adds a property to any
                                                        //page nodes that use the page template the property template belongs to this is 2
                                                        //1) auto created property
                                                        //2) property 1
                                                        //3) property 2 - the order for this property should be 2

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_DeletesProperty()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");
            var propertyTemplate = PropertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null);
            var property = PropertyService.Create(pageNode.Id, propertyTemplate.Id);

            //act
            PropertyService.Delete(property.Id);

            //assert
            var deletedProperty = UnitOfWork.Context.GetDbSet<Property>().FirstOrDefault(x => x.Id == property.Id);
            deletedProperty.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion
    }
}
