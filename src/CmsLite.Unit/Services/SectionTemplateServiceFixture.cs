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
    [Category("Integration")]
    public class SectionTemplateServiceFixture : ServiceBaseFixture
    {
        private ISectionTemplateService _sectionTemplateService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;
        private Mock<IPageTemplateService> _pageTemplateServiceMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _pageTemplateServiceMock = new Mock<IPageTemplateService>();
            _sectionTemplateService = new SectionTemplateService(_unitOfWorkMock.Object, _pageTemplateServiceMock.Object);

            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Dispose();
        }

        #region Create Tests

        [Test]
        public void Create_NoControllerName_ThrowsException()
        {
            var controllerName = string.Empty;
            Assert.That(() => _sectionTemplateService.Create(controllerName, "Test", "Foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.SectionTemplateControllerNameCannotBeNull));
        }

        [Test]
        public void Create_SectionTemplateWithControllerNameAlreadyExists_ThrowsException()
        {
            const string controllerName = "Foobar";
            var sectionTemplate = new SectionTemplate { ControllerName = controllerName };
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>
            {
                sectionTemplate
            });

            Assert.That(() => _sectionTemplateService.Create(controllerName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateControllerNameMustBeUnique, controllerName)));
        }

        [Test]
        public void Create_NameIsNullOrEmpty_UsesControllerNameAsName()
        {
            //arrange
            var name = string.Empty;
            const string controllerName = "SectionTemplateServiceFixtureController";
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>());

            //act
            var sectionTemplate = _sectionTemplateService.Create(controllerName, name, "");

            //assert
            sectionTemplate.Name.Should().Be.EqualTo(controllerName);
        }

        [Test]
        public void Create_IconImageNameIsNullOrEmpty_UsesDefaultSectionTemplateImage()
        {
            //arrange
            var name = string.Empty;
            const string controllerName = "SectionTemplateServiceFixtureController";
            var iconImageName = string.Empty;

            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>());

            //act
            var sectionTemplate = _sectionTemplateService.Create(controllerName, name, iconImageName);

            //assert
            sectionTemplate.IconImageName.Should().Be.EqualTo(CmsConstants.SectionTemplateDefaultThumbnail);
        }

        #endregion

        #region Delete Tests

        [Test]
        public void Delete_SectionTemplateNotFound_ThrowsException()
        {
            const int sectionTemplateId = 99999999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>());

            Assert.That(() => _sectionTemplateService.Delete(sectionTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void Delete_DeletesTheSectionTemplate()
        {
            //arrange
            const string controllerName = "SectionTemplateServiceFixtureController";
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(new InMemoryDbSet<SectionTemplate>());

            var sectionTemplate = _sectionTemplateService.Create(controllerName, "");

            //act
            _sectionTemplateService.Delete(sectionTemplate.Id);

            //assert
            var deletedSectionTemplate = _sectionTemplateService.Find(x => x.Id == sectionTemplate.Id);
            deletedSectionTemplate.Should().Be.Null();
        }

        [Test]
        public void Delete_DeletesPageTemplatesThatBelongToTheSectionTemplate()
        {
            //arrange
            var sectionTemplate = new SectionTemplate
            {
                Id = 1,
                PageTemplates = new Collection<PageTemplate>
                {
                    new PageTemplate
                    {
                        Id = 99
                    }
                }
            };
            var sectionTemplateDbSet = new InMemoryDbSet<SectionTemplate> {sectionTemplate};
            var pageTemplateDbSet = new InMemoryDbSet<PageTemplate> { sectionTemplate.PageTemplates.First() };
            _dbContextMock.Setup(x => x.GetDbSet<SectionTemplate>()).Returns(sectionTemplateDbSet);
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(pageTemplateDbSet);

            //act
            _sectionTemplateService.Delete(sectionTemplate.Id);

            //assert
            _pageTemplateServiceMock.Verify(x => x.Delete(sectionTemplate.PageTemplates.First(), false));
        }

        #endregion
    }
}
