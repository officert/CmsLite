using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services;
using CmsLite.Services.Helpers;
using CmsLite.Utilities.Cms;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    [Ignore("Need to convert to Unit Tests")]
    public class PageNodeServiceFixture : ServiceBaseFixture
    {
        private List<int> _createdSectionTemplateIds;
        private IPageNodeService _pageNodeService;
        private Mock<IPropertyService> _propertyServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
            _propertyServiceMock = new Mock<IPropertyService>();
            _pageNodeService = new PageNodeService(_unitOfWorkMock.Object, _propertyServiceMock.Object);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Dispose();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region CreateForSection

        [Test]
        public void CreateForSection_DisplayNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var displayName = string.Empty;

            //act + assert
            Assert.That(() => PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, displayName, "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeDisplayNameCannotBeNull));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_UrlNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var urlName = string.Empty;

            //act + assert
            Assert.That(() => PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeUrlNameCannotBeNull));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_UrlNameIsAlreadyUsedByAnotherPageNodeWithSameParentSectionNode_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            const string urlName = "Foobar";
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName);

            //act + assert
            Assert.That(() => PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeUrlNameMustBeUniqueWithinSection, CmsUrlHelper.FormatUrlName(urlName), sectionNode.Id)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int sectionNodeId = 99999;

            //act + assert
            Assert.That(() => PageNodeService.CreateForSection(sectionNodeId, 0, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeNotFound, sectionNodeId)));
        }

        [Test]
        public void CreateForSection_NoPageTemplateExistsInForId_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            const int pageTemplateId = 99999;

            //act + assert
            Assert.That(() => PageNodeService.CreateForSection(sectionNode.Id, pageTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFoundForSectionTemplate, pageTemplateId, sectionTemplate.Id)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_UrlNameGetsFormatted()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            const string urlName = "Foo bar";

            //act
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName);

            //assert
            pageNode.UrlName.Should().Be.EqualTo(CmsUrlHelper.FormatUrlName(urlName));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_NoOtherPageNodesExists_OrderIs0()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //assert
            pageNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_OtherPageNodesExists_OrderIsLastPageNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode1 = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar1", "foobar1");

            //act
            var pageNode2 = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode2.Order.Should().Be.EqualTo(1); //because the parent section has 2 page nodes

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_SetsCreatedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode.CreatedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_SetsModifiedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode.ModifiedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForSection_AddsNewPropertyForEachPropertyTemplateInThePageTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var propertyTemplate1 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar1", CmsPropertyType.RichTextEditor, null);
            var propertyTemplate2 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar2", CmsPropertyType.ImagePicker, null);

            //act
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode.Properties.Count.Should().Be.EqualTo(2);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region CreateForPage

        [Test]
        public void CreateForPage_DisplayNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var pageNode = new PageNode { Id = 1 };
            var displayName = string.Empty;

            //act + assert
            Assert.That(() => _pageNodeService.CreateForPage(pageNode.Id, pageTemplate.Id, displayName, "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeDisplayNameCannotBeNull));
        }

        [Test]
        public void CreateForPage_UrlNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode { Id = 1 };
            var urlName = string.Empty;

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeUrlNameCannotBeNull));
        }

        [Test]
        public void CreateForPage_UrlNameIsAlreadyUsedByAnotherPageNodeWithSameParentSectionNode_ThrowsException()
        {
            //arrange
            var pageNodeForSection = new PageNode { Id = 1 };
            var pageTemplateForPage = new PageTemplate { Id = 1 };
            const string urlName = "Foobar";
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);
            pageNodeForSection.PageNodes = new Collection<PageNode>
            {
                new PageNode { UrlName = formattedUrlName}
            };
            pageNodeForSection.PageTemplate = new PageTemplate
            {
                PageTemplates = new Collection<PageTemplate>
                {
                    pageTemplateForPage
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });

            //act + assert
            Assert.That(() => _pageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeUrlNameMustBeUniqueWithinPage, CmsUrlHelper.FormatUrlName(urlName), pageNodeForSection.Id)));
        }

        [Test]
        public void CreateForPage_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 99999;

            //act + assert
            Assert.That(() => _pageNodeService.CreateForPage(pageNodeId, 0, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void CreateForPage_NoPageTemplateExistsInForId_ThrowsException()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");
            const int pageTemplateId = 9999999;

            //act + assert
            Assert.That(() => PageNodeService.CreateForPage(pageNode.Id, pageTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFoundForPageTemplate, pageTemplateId, pageNode.PageTemplate.Id)));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_UrlNameGetsFormatted()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");
            const string urlName = "Foo bar";

            //act
            var pageNode = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar", urlName);

            //assert
            pageNode.UrlName.Should().Be.EqualTo(CmsUrlHelper.FormatUrlName(urlName));

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_NoOtherPageNodesExists_OrderIs0()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar", "foobar");

            //assert
            pageNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_OtherPageNodesExists_OrderIsLastPageNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");
            var childPageNode1 = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar1", "foobar1");

            //act
            var childPageNode2 = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar2", "foobar2");

            //assert
            childPageNode2.Order.Should().Be.EqualTo(1); //because the parent section has 2 page nodes

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_SetsCreatedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar2", "foobar2");

            //assert
            pageNode.CreatedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_SetsModifiedOnDate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");

            //act
            var pageNode = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar2", "foobar2");

            //assert
            pageNode.ModifiedOn.Should().Not.Be.EqualTo(null);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void CreateForPage_AddsNewPropertyForEachPropertyTemplateInThePageTemplate()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNodeForSection = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");
            var propertyTemplate1 = PropertyTemplateService.Create(pageTemplateForPage.Id, "Foobar1", CmsPropertyType.RichTextEditor, null);
            var propertyTemplate2 = PropertyTemplateService.Create(pageTemplateForPage.Id, "Foobar2", CmsPropertyType.ImagePicker, null);

            //act
            var pageNode = PageNodeService.CreateForPage(pageNodeForSection.Id, pageTemplateForPage.Id, "Foobar2", "foobar2");

            //assert
            pageNode.Properties.Count.Should().Be.EqualTo(2);

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_NoPageNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 999999;

            //act+ assert
            Assert.That(() => PageNodeService.Delete(pageNodeId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void Delete_PageNodeIsNull_ThrowsException()
        {
            //arrange
            PageNode pageNode = null;

            //act+ assert
            Assert.That(() => PageNodeService.Delete(pageNode),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeCannotBeNull));
        }

        [Test]
        public void Delete_DeletesPageNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //act
            PageNodeService.Delete(pageNode.Id);

            //assert
            var deleltedPageNode = UnitOfWork.Context.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == pageNode.Id);
            deleltedPageNode.Should().Be.Null();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_DeletesAnyPropertiesThatBelongToThePageNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var propertyTemplate1 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar1", CmsPropertyType.ImagePicker, null);
            var propertyTemplate2 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar2", CmsPropertyType.RichTextEditor, null);
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //act + assert
            pageNode.Properties.Count.Should().Be.EqualTo(2); //verify the page node was created with some properties

            PageNodeService.Delete(pageNode);

            //assert
            var propertiesForDeletedPageNode = UnitOfWork.Context.GetDbSet<Property>().Where(x => x.ParentPageNodeId == pageNode.Id);
            propertiesForDeletedPageNode.Should().Be.Empty();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        [Test]
        public void Delete_DeletesAnyChildPageNodes()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.Create(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplateForSection = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var pageTemplateForPage = PageTemplateService.CreateForPageTemplate(pageTemplateForSection.Id, "Foobar", "FoobarModel");
            var parentPageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplateForSection.Id, "Foobar", "foobar");
            var childPageNode1 = PageNodeService.CreateForPage(parentPageNode.Id, pageTemplateForPage.Id, "Foobar1", "foobar1");
            var childPageNode2 = PageNodeService.CreateForPage(parentPageNode.Id, pageTemplateForPage.Id, "Foobar2", "foobar2");

            //act
            PageNodeService.Delete(parentPageNode.Id);

            //assert
            var childNodesForDeletedPageNode = UnitOfWork.Context.GetDbSet<PageNode>().Where(x => x.ParentPageNodeId == parentPageNode.Id);
            childNodesForDeletedPageNode.Should().Be.Empty();

            _createdSectionTemplateIds.Add(sectionTemplate.Id);
        }

        #endregion
    }
}
