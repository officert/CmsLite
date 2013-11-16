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
    public class PropertyServiceFixture : ServiceBaseFixture
    {
        private IPropertyService _propertyService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);

            _propertyService = new PropertyService(_unitOfWorkMock.Object);
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
        public void Create_NoPageNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 99999;
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act + assert
            Assert.That(() => _propertyService.Create(pageNodeId, 0),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void Create_NoPropertyTemplateExistsOnPageTemplateForId_ThrowsException()
        {
            //arrange
            const int propertyTemplateId = 99999;
            var pageNode = new PageNode
            {
                Id = 22,
                PageTemplate = new PageTemplate
                {
                    Id = 1,
                    PropertyTemplates = new Collection<PropertyTemplate>()
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNode
            });

            //act + assert
            Assert.That(() => _propertyService.Create(pageNode.Id, propertyTemplateId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PropertyTemplateNotFoundForPageTemplate, propertyTemplateId, pageNode.PageTemplate.Id)));
        }

        [Test]
        public void Create_PageNodeIsNull_ThrowsException()
        {
            //arrange
            PageNode pageNode = null;
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act + assert
            Assert.That(() => _propertyService.Create(pageNode, null),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeCannotBeNull));
        }

        [Test]
        public void Create_PropertyTemplateIsNull_ThrowsException()
        {
            //arrange
            var pageNode = new PageNode{ Id = 1};

            PropertyTemplate propertyTemplate = null;

            //act + assert
            Assert.That(() => _propertyService.Create(pageNode, propertyTemplate),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PropertyTemplateCannotBeNull));
        }

        [Test]
        public void Create_OtherPropertiesExistOnPageNode_OrderIsLastProperty()
        {
            //arrange
            var pageNode = new PageNode
            {
                Id = 1,
                Properties = new Collection<Property>
                {
                    new Property { Id = 1, Order = 1 },
                    new Property { Id = 2, Order = 2 }
                },
                PageTemplate = new PageTemplate
                {
                    PropertyTemplates = new Collection<PropertyTemplate>()
                    {
                        new PropertyTemplate
                        {
                            Id = 1,
                            CmsPropertyType = CmsPropertyType.RichTextEditor.ToString()
                        }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<Property>()).Returns(new InMemoryDbSet<Property>());

            //act
            var property = _propertyService.Create(pageNode.Id, pageNode.PageTemplate.PropertyTemplates.First().Id);

            //assert
            property.Order.Should().Be.EqualTo(2);     //because creating a new property template automatically adds a property to any
                                                        //page nodes that use the page template the property template belongs to this is 2
                                                        //1) auto created property
                                                        //2) property 1
                                                        //3) property 2 - the order for this property should be 2
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_IdIsLessThan1_ThrowsException()
        {
            //arrange
            const int propertyId = 0;

            //act + assert
            Assert.That(() => _propertyService.Delete(propertyId), Throws.ArgumentException.With.Message.EqualTo("id"));
        }

        [Test]
        public void Delete_DeletesProperty()
        {
            //arrange
            var property = new Property { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<Property>()).Returns(new InMemoryDbSet<Property>
            {
                property
            });

            //act
            _propertyService.Delete(property.Id);

            //assert
            var deletedProperty = _dbContextMock.Object.GetDbSet<Property>().FirstOrDefault(x => x.Id == property.Id);
            deletedProperty.Should().Be.Null();
        }

        #endregion
    }
}
