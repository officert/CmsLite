using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services;
using CmsLite.Utilities.Cms;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public class PageTemplateServiceFixture : ServiceBaseFixture
    {
        private List<int> _createdSectionTemplateIds;

        private IPageTemplateService _pageTemplateService;
        private Mock<IPageNodeService> _pageNodeServiceMock;
        private Mock<IPropertyTemplateService> _propertyTemplateServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
            _pageNodeServiceMock = new Mock<IPageNodeService>();
            _propertyTemplateServiceMock = new Mock<IPropertyTemplateService>();
            _pageTemplateService = new PageTemplateService(_unitOfWorkMock.Object, _pageNodeServiceMock.Object, _propertyTemplateServiceMock.Object);
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

        #region CreateForSectionTemplate

        [Test]
        public void CreateForSectionTemplate_NoActionName_ThrowsException()
        {
            var actionName = string.Empty;
            Assert.That(() => _pageTemplateService.CreateForSectionTemplate(0, actionName, ""),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateActionNameCannotBeNull));
        }

        [Test]
        public void CreateForSectionTemplate_NoModelName_ThrowsException()
        {
            const string actionName = "foobar";
            var modelName = string.Empty;
            Assert.That(() => _pageTemplateService.CreateForSectionTemplate(0, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateModelNameCannotBeNull));
        }

        [Test]
        public void CreateForSectionTemplate_NoSectionTemplateExistsForId_ThrowsException()
        {
            const int sectionTemplateId = 999999;
            const string actionName = "Foobar";
            const string modelName = "FoobarModel";

            Assert.That(() => _pageTemplateService.CreateForSectionTemplate(sectionTemplateId, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void CreateForSectionTemplate_NameIsNullOrEmpty_UsesActionNameAsName()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>());
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>
            {
                sectionTemplate
            });

            //act
            var pageTemplate = _pageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, actionName, "Foobar", name);

            //assert
            pageTemplate.Name.Should().Be.EqualTo(actionName);
        }

        [Test]
        public void CreateForSectionTemplate_IconImageNameIsNullOrEmpty_UsesDefaultPageTemplateImage()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            var iconImageName = string.Empty;
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>());
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>
            {
                sectionTemplate
            });

            //act
            var pageTemplate = _pageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, actionName, "FoobarModel", name, iconImageName);

            //assert
            pageTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.PageTemplateDefaultThumbnail);
        }

        #endregion

        #region CreateForPageTemplate

        [Test]
        public void CreateForPageTemplate_NoActionName_ThrowsException()
        {
            var actionName = string.Empty;
            Assert.That(() => _pageTemplateService.CreateForPageTemplate(0, actionName, ""),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateActionNameCannotBeNull));
        }

        [Test]
        public void CreateForPageTemplate_NoModelName_ThrowsException()
        {
            const string actionName = "foobar";
            var modelName = string.Empty;
            Assert.That(() => _pageTemplateService.CreateForPageTemplate(0, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateModelNameCannotBeNull));
        }

        [Test]
        public void CreateForPageTemplate_NoPageTemplateExistsForId_ThrowsException()
        {
            const int pageTemplateId = 999999;
            const string actionName = "Foobar";
            const string modelName = "FoobarModel";
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>());


            Assert.That(() => _pageTemplateService.CreateForPageTemplate(pageTemplateId, actionName, modelName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void CreateForPageTemplate_NameIsNullOrEmpty_UsesActionNameAsName()
        {
            //arrange
            var parentPageTemplate = new PageTemplate { Id = 1 };
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                parentPageTemplate
            });

            //act
            var pageTemplate = _pageTemplateService.CreateForPageTemplate(parentPageTemplate.Id, actionName, "Foobar", name);

            //assert
            pageTemplate.Name.Should().Be.EqualTo(actionName);
        }

        [Test]
        public void CreateForPageTemplate_IconImageNameIsNullOrEmpty_UsesDefaultPageTemplateImage()
        {
            //arrange
            var parentPageTemplate = new PageTemplate { Id = 1 };
            const string actionName = "PageTemplateServiceFixtureController";
            var name = string.Empty;
            var iconImageName = string.Empty;
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                parentPageTemplate
            });

            //act
            var pageTemplate = _pageTemplateService.CreateForPageTemplate(parentPageTemplate.Id, actionName, "FoobarModel", name, iconImageName);

            //assert
            pageTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.PageTemplateDefaultThumbnail);
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_PageTemplateNotFound_ThrowsException()
        {
            const int pageTemplateId = 99999999;
            Assert.That(() => _pageTemplateService.Delete(pageTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void Delete_PageTemplateIsNullThrowsException()
        {
            PageTemplate pageTemplate = null;

            Assert.That(() => _pageTemplateService.Delete(pageTemplate),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageTemplateCannotBeNull));
        }

        [Test]
        public void Delete_DeletesThePageTemplate()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                pageTemplate
            });

            //act
            _pageTemplateService.Delete(pageTemplate.Id);

            //assert
            var deletedPageTemplate = _dbContextMock.Object.GetDbSet<PageTemplate>().FirstOrDefault(x => x.Id == pageTemplate.Id);
            deletedPageTemplate.Should().Be.Null();
        }

        [Test]
        public void Delete_DeletesAnyPageNodesThatUseThePageTemplate()
        {
            //arrange
            var pageTemplate = new PageTemplate
            {
                Id = 1,
                PageNodes = new Collection<PageNode>
                {
                    new PageNode{ Id = 99 }
                }
            };
            var pageTemplateDbSet = new InMemoryDbSet<PageTemplate> { pageTemplate };
            var sectionNodeDbSet = new InMemoryDbSet<PageNode> { pageTemplate.PageNodes.First() };
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(pageTemplateDbSet);
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(sectionNodeDbSet);

            //act
            _pageTemplateService.Delete(pageTemplate.Id);

            //assert
            _pageNodeServiceMock.Verify(x => x.Delete(pageTemplate.PageNodes.First(), false));
        }

        #endregion
    }
}
