using System;
using System.Collections.ObjectModel;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using CmsLite.Services;
using CmsLite.Services.Helpers;
using CmsLite.Utilities;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public class SectionNodeServiceFixture : ServiceBaseFixture
    {
        private ISectionNodeService _sectionNodeService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;
        private Mock<ISectionTemplateService> _sectionTemplateServiceMock;
        private Mock<IPageNodeService> _pageNodeServiceMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
            _sectionTemplateServiceMock = new Mock<ISectionTemplateService>();
            _pageNodeServiceMock = new Mock<IPageNodeService>();

            _sectionNodeService = new SectionNodeService(_unitOfWorkMock.Object, _sectionTemplateServiceMock.Object, _pageNodeServiceMock.Object);
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

        #region GetAll

        [Test]
        public void GetAll_DoesNotIncludeTrashedNodes()
        {
            //arrange
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                new SectionNode { Id = 1, InTrash = true },
                new SectionNode { Id = 2, InTrash = true },
                new SectionNode { Id = 3, InTrash = false },
                new SectionNode { Id = 4, InTrash = false }
            });

            //act
            var sectionNodes = _sectionNodeService.GetAll();

            //assert
            sectionNodes.ToList().ForEach(x => x.InTrash.Should().Be.False());
        }

        [Test]
        public void GetAll_IncludeTrashed_IncludesTrashedNodes()
        {
            //arrange
            var sectionNodeDbSet = new InMemoryDbSet<SectionNode>
            {
                new SectionNode {Id = 1, InTrash = true},
                new SectionNode {Id = 2, InTrash = true},
                new SectionNode {Id = 3, InTrash = false},
                new SectionNode {Id = 4, InTrash = false}
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(sectionNodeDbSet);

            //act
            var sectionNodes = _sectionNodeService.GetAll(true);

            //assert
            sectionNodes.Count().Should().Equals(sectionNodeDbSet.Count());
        }

        #endregion

        #region GetAllTrashed

        [Test]
        public void GetAllTrashed_ReturnsOnlyTrashedNodes()
        {
            //arrange
            var sectionNodeDbSet = new InMemoryDbSet<SectionNode>
            {
                new SectionNode {Id = 1, InTrash = true},
                new SectionNode {Id = 2, InTrash = true},
                new SectionNode {Id = 3, InTrash = false},
                new SectionNode {Id = 4, InTrash = false}
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(sectionNodeDbSet);

            //act
            var sectionNodes = _sectionNodeService.GetAllTrashed();

            //assert
            sectionNodes.ToList().ForEach(x => x.InTrash.Should().Be.True());
        }

        #endregion

        #region CreateForSectionTemplate

        [Test]
        public void Create_DisplayNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            var displayName = string.Empty;

            //act + assert
            Assert.That(() => _sectionNodeService.CreateSectionNode(sectionTemplate.Id, displayName, "Foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "displayName")));
        }

        [Test]
        public void Create_UrlNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            var urlName = string.Empty;

            //act + assert
            Assert.That(() => _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "urlName")));
        }

        [Test]
        public void Create_UrlNameIsAlreadyUsedByAnotherSectionNode_ThrowsException()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            const string urlName = "Foobar";
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                new SectionNode
                {
                    UrlName = formattedUrlName
                }
            });
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);


            //act + assert
            Assert.That(() => _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeUrlNameMustBeUnique, formattedUrlName)));
        }

        [Test]
        public void Create_NoSectionTemplateExistsForId_ThrowsException()
        {
            //arrange
            const int sectionTemplateId = 999999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns((SectionTemplate)null);

            //act + assert
            Assert.That(() => _sectionNodeService.CreateSectionNode(sectionTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionTemplateNotFound, sectionTemplateId)));
        }

        [Test]
        public void Create_UrlNameGetsFormatted()
        {
            //arrange
            const string urlName = "Foobar";
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);
            var sectionTemplate = new SectionTemplate{ Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", urlName);

            //assert
            sectionNode.UrlName.Should().Be.EqualTo(formattedUrlName);
        }

        [Test]
        public void Create_NoOtherSectionNodesExists_OrderIs0()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "Foobar");

            //assert
            sectionNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);
        }

        [Test]
        public void Create_OtherSectionNodesExists_OrderIsLastSectionNode()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                new SectionNode
                {
                    Id = 1
                }
            });
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.Order.Should().Be.EqualTo(1);
        }

        [Test]
        public void Create_SetsCreatedOnDate()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.CreatedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void Create_SetsModifiedOnDate()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.ModifiedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void Create_SetsInTrashToFalse()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            var sectionNode = _sectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "foobar2");

            //assert
            sectionNode.InTrash.Should().Be.False();
        }

        #endregion

        #region Trash

        [Test]
        public void Trash_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int sectionNodeId = 999999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());

            //act + assert
            Assert.That(() => _sectionNodeService.Trash(sectionNodeId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeNotFound, sectionNodeId)));
        }

        [Test]
        public void Trash_IsAlreadyInTrash_ThrowsInvalidOperationException()
        {
            //arrange
            var sectionNode = new SectionNode
            {
                Id = 1,
                InTrash = true
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });

            //act+assert
            Assert.That(() => _sectionNodeService.Trash(sectionNode.Id),
                    Throws.Exception.TypeOf<InvalidOperationException>().With.Message.EqualTo(string.Format(Messages.SectionNodeInTrashAlready, sectionNode.Id)));
        }

        [Test]
        public void Trash_SetsIsTrashedToTrue()
        {
            //arrange
            var sectionNode = new SectionNode
            {
                Id = 1,
                InTrash = false
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });

            //act
            _sectionNodeService.Trash(sectionNode.Id);

            //assert
            _sectionNodeService.GetById(sectionNode.Id).InTrash.Should().Be.True();
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_DeletesTheSectionNode()
        {
            //arrange
            var sectionTemplate = new SectionTemplate { Id = 1 };
            var sectionNode = new SectionNode {Id = 99};
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _sectionTemplateServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(sectionTemplate);

            //act
            _sectionNodeService.Delete(sectionNode.Id);

            //assert
            var foundSectionNode = _sectionNodeService.GetById(sectionNode.Id);
            foundSectionNode.Should().Be.Null();
        }

        [Test]
        public void Delete_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int sectionNodeId = 999999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());

            //act + assert
            Assert.That(() => _sectionNodeService.Delete(sectionNodeId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeNotFound, sectionNodeId)));
        }

        [Test]
        public void Delete_DeletesPageNodesThatBelongToTheSectionNode()
        {
            //arrange
            var sectionNode = new SectionNode
            {
                Id = 1,
                PageNodes = new Collection<PageNode>
                {
                    new PageNode
                    {
                        Id = 1
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act
            _sectionNodeService.Delete(sectionNode.Id);

            //assert
            _pageNodeServiceMock.Verify(x => x.Delete(sectionNode.PageNodes.First(), false));
        }

        #endregion
    }
}
