using System;
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
    public class PropertyTemplateServiceFixture : ServiceBaseFixture
    {
        private IPropertyTemplateService _propertyTemplateService;
        private Mock<IPropertyService> _propertyServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
            _propertyServiceMock = new Mock<IPropertyService>();
            _propertyTemplateService = new PropertyTemplateService(_unitOfWorkMock.Object, _propertyServiceMock.Object);
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

        #region Create

        [Test]
        public void Create_NoPropertyName_ThrowsException()
        {
            var propertyName = string.Empty;
            Assert.That(() => _propertyTemplateService.Create(0, propertyName, CmsPropertyType.RichTextEditor, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PropertyTemplatePropertyNameCannotBeNull));
        }

        [Test]
        public void Create_NoPageTemplateExistsForId_ThrowsException()
        {
            //arrange
            const int pageTemplateId = 99999;
            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>());

            Assert.That(() => _propertyTemplateService.Create(pageTemplateId, "Foobar", CmsPropertyType.RichTextEditor, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFound, pageTemplateId)));
        }

        [Test]
        public void Create_DisplayNameIsNullOrEmpty_UsesPropertyNameAsDisplayName()
        {
            //arrange
            var displayName = string.Empty;
            const string propertyName = "FoobarProperty";

            var pageTemplate = new PageTemplate { Id = 1 };

            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                pageTemplate
            });
            _dbContextMock.Setup(x => x.GetDbSet<PropertyTemplate>()).Returns(new InMemoryDbSet<PropertyTemplate>());

            //act
            var propertyTemplate = _propertyTemplateService.Create(pageTemplate.Id, propertyName, CmsPropertyType.ImagePicker, 0, displayName: displayName);

            //assert
            propertyTemplate.DisplayName.Should().Be.EqualTo(propertyName);
        }

        [Test]
        public void Create_TabNameIsNullOrEmpty_UsesDefaultTabName()
        {
            //arrange
            var tabName = string.Empty;

            var pageTemplate = new PageTemplate { Id = 1 };

            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                pageTemplate
            });
            _dbContextMock.Setup(x => x.GetDbSet<PropertyTemplate>()).Returns(new InMemoryDbSet<PropertyTemplate>());

            //act
            var propertyTemplate = _propertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null, tabName);

            //assert
            propertyTemplate.TabName.Should().Be.EqualTo(CmsConstants.PropertyTemplateDefaultTabName);
        }

        [Test]
        public void Create_ParentPageTemplateHasPageNodes_AddNewPropertyToEachPageNodeUsingPropertyTemplate()
        {
            //arrange
            var pageTemplate = new PageTemplate
            {
                Id = 1,
                PageNodes = new Collection<PageNode>
                {
                    new PageNode
                    {
                        Id = 1, 
                        Properties = new Collection<Property>()
                    }
                }
            };

            _dbContextMock.Setup(x => x.GetDbSet<PageTemplate>()).Returns(new InMemoryDbSet<PageTemplate>
            {
                pageTemplate
            });
            _dbContextMock.Setup(x => x.GetDbSet<PropertyTemplate>()).Returns(new InMemoryDbSet<PropertyTemplate>());

            //act
            var propertyTemplate = _propertyTemplateService.Create(pageTemplate.Id, "Foobar", CmsPropertyType.ImagePicker, null);

            _propertyServiceMock.Setup(x => x.Create(pageTemplate.PageNodes.First(), propertyTemplate, "", false)).Returns(It.IsAny<Property>);

            //assert
            _propertyServiceMock.Verify(x => x.Create(pageTemplate.PageNodes.First(), propertyTemplate, "", false));
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_PropertyTemplateNotFound_ThrowsException()
        {
            const int propertyTemplateId = 99999999;
            Assert.That(() => _propertyTemplateService.Delete(propertyTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PropertyTemplateNotFound, propertyTemplateId)));
        }

        [Test]
        public void Delete_DeletesThePropertyTemplate()
        {
            //arrange
            var propertyTemplate = new PropertyTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<PropertyTemplate>()).Returns(new InMemoryDbSet<PropertyTemplate>
            {
                propertyTemplate
            });

            //act
            _propertyTemplateService.Delete(propertyTemplate.Id);

            //assert
            var deletedPropertyTemplate = _dbContextMock.Object.GetDbSet<PropertyTemplate>().FirstOrDefault(x => x.Id == propertyTemplate.Id);
            deletedPropertyTemplate.Should().Be.Null();
        }

        [Test]
        [Ignore("This test tests the SQL cascade, not sure we can unit test this.")]
        public void Delete_DeletesAnyPropertiesThatUseThePropertyTemplate()
        {
            //arrange
            var propertyTemplate = new PropertyTemplate
            {
                Id = 1,
                Properties = new Collection<Property>
                {
                    new Property { Id = 44 }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PropertyTemplate>()).Returns(new InMemoryDbSet<PropertyTemplate>
            {
                propertyTemplate
            });

            //act
            _propertyTemplateService.Delete(propertyTemplate.Id);

            //assert
            var propertiesForDeletedPropertyTemplate = UnitOfWork.Context.GetDbSet<Property>().Where(x => x.PropertyTemplateId == propertyTemplate.Id);
            propertiesForDeletedPropertyTemplate.Should().Be.Empty();
        }

        #endregion
    }
}
