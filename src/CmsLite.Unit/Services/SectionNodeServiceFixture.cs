using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Resources;
using CmsLite.Services.Helpers;
using CmsLite.Utilities.Cms;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public class SectionNodeServiceFixture : ServiceBaseFixture
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
        public void Create_DisplayNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var displayName = string.Empty;

            //act + assert
            Assert.That(() => SectionNodeService.Create(sectionTemplate.Id, displayName, "Foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.SectionNodeDisplayNameCannotBeNull));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_UrlNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var urlName = string.Empty;

            //act + assert
            Assert.That(() => SectionNodeService.Create(sectionTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.SectionNodeUrlNameCannotBeNull));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_UrlNameIsAlreadyUsedByAnotherSectionNode_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            const string urlName = "Foobar";
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foo", urlName);

            //act + assert
            Assert.That(() => SectionNodeService.Create(sectionTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeUrlNameMustBeUnique, formattedUrlName)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_NoSectionTemplateExistsForId_ThrowsException()
        {
            //arrange
            const int sectionTemplateId = 999999;

            //act + assert
            Assert.That(() => SectionNodeService.Create(sectionTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void Create_UrlNameGetsFormatted()
        {
            //arrange
            const string urlName = "Foobar";
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);
            var sectionTemplate = SectionTemplateService.Create("Foobar");

            //act
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", urlName);

            //assert
            sectionNode.UrlName.Should().Be.EqualTo(formattedUrlName);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_NoOtherSectionNodesExists_OrderIs0()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");

            //act
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "Foobar");

            //assert
            sectionNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_OtherSectionNodesExists_OrderIsLastSectionNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode1 = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar1");

            //act
            var sectionNode2 = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode2.Order.Should().Be.EqualTo(1);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_SetsCreatedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");

            //act
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.CreatedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Create_SetsModifiedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");

            //act
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.ModifiedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_DeletesTheSectionNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("SectionNodeServiceFixture", "Foobar", "");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Node", "node");


            //act
            SectionNodeService.Delete(sectionNode.Id);

            //assert
            var foundSectionNode = SectionNodeService.Find(x => x.Id == sectionNode.Id);
            foundSectionNode.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int sectionNodeId = 999999;

            //act + assert
            Assert.That(() => SectionNodeService.Delete(sectionNodeId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeNotFound, sectionNodeId)));
        }

        [Test]
        public void Delete_DeletesPageNodesThatBelongToTheSectionNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //act
            SectionNodeService.Delete(sectionNode.Id);

            //assert
            var deletedPageNode = UnitOfWork.Context.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == pageNode.Id);

            deletedPageNode.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion
    }
}
