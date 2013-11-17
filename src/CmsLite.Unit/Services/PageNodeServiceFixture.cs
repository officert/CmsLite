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
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Services
{
    [TestFixture]
    [Category("Unit")]
    public class PageNodeServiceFixture : ServiceBaseFixture
    {
        private IPageNodeService _pageNodeService;
        private Mock<IPagePropertyService> _propertyServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDbContext> _dbContextMock;

        protected override void PostFixtureSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dbContextMock = new Mock<IDbContext>();
            _unitOfWorkMock.Setup(x => x.Context).Returns(_dbContextMock.Object);
            _propertyServiceMock = new Mock<IPagePropertyService>();
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
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode { Id = 1 };
            var displayName = string.Empty;

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, displayName, "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "displayName")));
        }

        [Test]
        public void CreateForSection_UrlNameIsNullOrEmpty_ThrowsException()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode { Id = 1 };
            var urlName = string.Empty;

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "urlName")));
        }

        [Test]
        public void CreateForSection_UrlNameIsAlreadyUsedByAnotherPageNodeWithSameParentSectionNode_ThrowsException()
        {
            //arrange
            const string urlName = "Foobar";
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                PageNodes = new Collection<PageNode>
                {
                    new PageNode
                    {
                        UrlName = CmsUrlHelper.FormatUrlName(urlName)
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeUrlNameMustBeUniqueWithinSection, CmsUrlHelper.FormatUrlName(urlName), sectionNode.Id)));
        }

        [Test]
        public void CreateForSection_NoSectionNodeExistsForId_ThrowsException()
        {
            //arrange
            const int sectionNodeId = 99999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>());

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNodeId, 0, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.SectionNodeNotFound, sectionNodeId)));
        }

        [Test]
        public void CreateForSection_NoPageTemplateExistsInSectionTemplateForId_ThrowsException()
        {
            //arrange
            var sectionTemplate = new SectionTemplate();
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>()
                }
            };
            const int pageTemplateId = 99999;
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });

            //act + assert
            Assert.That(() => _pageNodeService.CreateForSection(sectionNode.Id, pageTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFoundForSectionTemplate, pageTemplateId, sectionTemplate.Id)));
        }

        [Test]
        public void CreateForSection_UrlNameGetsFormatted()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());
            const string urlName = "Foo bar";

            //act
            var pageNode = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", urlName);

            //assert
            pageNode.UrlName.Should().Be.EqualTo(CmsUrlHelper.FormatUrlName(urlName));
        }

        [Test]
        public void CreateForSection_NoOtherPageNodesExists_OrderIs0()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act
            var pageNode = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //assert
            pageNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);
        }

        [Test]
        public void CreateForSection_OtherPageNodesExists_OrderIsLastPageNode()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                },
                PageNodes = new Collection<PageNode>
                {
                    new PageNode { Order = 1 }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act
            var pageNode2 = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode2.Order.Should().Be.EqualTo(1); //because the parent section now has 2 page nodes
        }

        [Test]
        public void CreateForSection_SetsCreatedOnDate()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());


            //act
            var pageNode = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode.CreatedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void CreateForSection_SetsModifiedOnDate()
        {
            //arrange
            var pageTemplate = new PageTemplate { Id = 1 };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());


            //act
            var pageNode = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            pageNode.ModifiedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void CreateForSection_AddsNewPropertyForEachPropertyTemplateInThePageTemplate()
        {
            //arrange
            var pageTemplate = new PageTemplate
            {
                Id = 1,
                PropertyTemplates = new Collection<PagePropertyTemplate>
                {
                    new PagePropertyTemplate { Id = 1, CmsPropertyType = CmsPropertyType.RichTextEditor.ToString() },
                    new PagePropertyTemplate { Id = 2, CmsPropertyType = CmsPropertyType.ImagePicker.ToString() }
                }
            };
            var sectionNode = new SectionNode
            {
                Id = 1,
                SectionTemplate = new SectionTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        pageTemplate
                    }
                },
                PageNodes = new Collection<PageNode>
                {
                    new PageNode { Order = 1 }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<SectionNode>()).Returns(new InMemoryDbSet<SectionNode>
            {
                sectionNode
            });

            var propertyTemplates = pageTemplate.PropertyTemplates.ToList();

            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            _propertyServiceMock.Setup(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[0], "", false)).Returns(It.IsAny<PageProperty>);
            _propertyServiceMock.Setup(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[1], "", false)).Returns(It.IsAny<PageProperty>);

            //act
            var pageNode = _pageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar2", "foobar2");

            //assert
            _propertyServiceMock.Verify(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[0], "", false));
            _propertyServiceMock.Verify(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[1], "", false));
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
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "displayName")));
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
                .With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullOrEmptyMessageFormat, "urlName")));
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
        public void CreateForPage_NoPageNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 99999;
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>());

            //act + assert
            Assert.That(() => _pageNodeService.CreateForPage(pageNodeId, 0, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void CreateForPage_NoPageTemplateExistsInForId_ThrowsException()
        {
            //arrange
            var pageNode = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>()
                }
            };
            const int pageTemplateId = 9999999;
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNode
            });

            //act + assert
            Assert.That(() => _pageNodeService.CreateForPage(pageNode.Id, pageTemplateId, "Foobar", "foobar"),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageTemplateNotFoundForPageTemplate, pageTemplateId, pageNode.PageTemplate.Id)));
        }

        [Test]
        public void CreateForPage_UrlNameGetsFormatted()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate { Id = 44 }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });
            const string urlName = "Foo bar";

            //act
            var pageNode = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar", urlName);

            //assert
            pageNode.UrlName.Should().Be.EqualTo(CmsUrlHelper.FormatUrlName(urlName));
        }

        [Test]
        public void CreateForPage_NoOtherPageNodesExists_OrderIs0()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate { Id = 44 }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });

            //act
            var pageNode = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar", "foobar");

            //assert
            pageNode.Order.Should().Be.EqualTo(CmsConstants.FirstOrderNumber);
        }

        [Test]
        public void CreateForPage_OtherPageNodesExists_OrderIsLastPageNode()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate { Id = 44 }
                    }
                },
                PageNodes = new Collection<PageNode>
                {
                    new PageNode()
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });

            //act
            var childPageNode2 = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar2", "foobar2");

            //assert
            childPageNode2.Order.Should().Be.EqualTo(1); //because the parent section has 2 page nodes
        }

        [Test]
        public void CreateForPage_SetsCreatedOnDate()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate { Id = 44 }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });

            //act
            var pageNode = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar2", "foobar2");

            //assert
            pageNode.CreatedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void CreateForPage_SetsModifiedOnDate()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate { Id = 44 }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });

            //act
            var pageNode = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar2", "foobar2");

            //assert
            pageNode.ModifiedOn.Should().Not.Be.EqualTo(null);
        }

        [Test]
        public void CreateForPage_AddsNewPropertyForEachPropertyTemplateInThePageTemplate()
        {
            //arrange
            var pageNodeForSection = new PageNode
            {
                Id = 1,
                PageTemplate = new PageTemplate
                {
                    PageTemplates = new Collection<PageTemplate>
                    {
                        new PageTemplate
                        {
                            Id = 44,
                            PropertyTemplates = new Collection<PagePropertyTemplate>
                            {
                                new PagePropertyTemplate { Id = 1, CmsPropertyType = CmsPropertyType.RichTextEditor.ToString() },
                                new PagePropertyTemplate { Id = 2, CmsPropertyType = CmsPropertyType.ImagePicker.ToString() }
                            }
                        }
                    }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNodeForSection
            });
            var propertyTemplates = pageNodeForSection.PageTemplate.PageTemplates.First().PropertyTemplates.ToList();

            _propertyServiceMock.Setup(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[0], "", false)).Returns(It.IsAny<PageProperty>);
            _propertyServiceMock.Setup(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[1], "", false)).Returns(It.IsAny<PageProperty>);

            //act
            var pageNode = _pageNodeService.CreateForPage(pageNodeForSection.Id, pageNodeForSection.PageTemplate.PageTemplates.First().Id, "Foobar2", "foobar2");

            //assert
            _propertyServiceMock.Verify(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[0], "", false));
            _propertyServiceMock.Verify(x => x.Create(It.IsAny<PageNode>(), propertyTemplates[1], "", false));
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_NoPageNodeExistsForId_ThrowsException()
        {
            //arrange
            const int pageNodeId = 999999;

            //act+ assert
            Assert.That(() => _pageNodeService.Delete(pageNodeId),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(string.Format(Messages.PageNodeNotFound, pageNodeId)));
        }

        [Test]
        public void Delete_PageNodeIsNull_ThrowsException()
        {
            //arrange
            PageNode pageNode = null;

            //act+ assert
            Assert.That(() => _pageNodeService.Delete(pageNode),
                Throws.Exception.TypeOf<ArgumentException>()
                .With.Message.EqualTo(Messages.PageNodeCannotBeNull));
        }

        [Test]
        public void Delete_DeletesPageNode()
        {
            //arrange
            var pageNode = new PageNode { Id = 1 };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNode
            });

            //act
            _pageNodeService.Delete(pageNode.Id);

            //assert
            var deleltedPageNode = _dbContextMock.Object.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == pageNode.Id);
            deleltedPageNode.Should().Be.Null();
        }

        [Test]
        [Ignore("This test tests that the SQL cascade delete is working, not sure we can unit test this.")]
        public void Delete_DeletesAnyPropertiesThatBelongToThePageNode()
        {
            //arrange
            var sectionTemplate = SectionTemplateService.Create("Foobar");
            var sectionNode = SectionNodeService.CreateSectionNode(sectionTemplate.Id, "Foobar", "foobar");
            var pageTemplate = PageTemplateService.CreateForSectionTemplate(sectionTemplate.Id, "Foobar", "FoobarModel");
            var propertyTemplate1 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar1", CmsPropertyType.ImagePicker, null);
            var propertyTemplate2 = PropertyTemplateService.Create(pageTemplate.Id, "Foobar2", CmsPropertyType.RichTextEditor, null);
            var pageNode = PageNodeService.CreateForSection(sectionNode.Id, pageTemplate.Id, "Foobar", "foobar");

            //act + assert
            pageNode.Properties.Count.Should().Be.EqualTo(2); //verify the page node was created with some properties

            PageNodeService.Delete(pageNode);

            //assert
            var propertiesForDeletedPageNode = UnitOfWork.Context.GetDbSet<PageProperty>().Where(x => x.ParentPageNodeId == pageNode.Id);
            propertiesForDeletedPageNode.Should().Be.Empty();
        }

        [Test]
        public void Delete_DeletesAnyChildPageNodes()
        {
            //arrange
            var pageNode = new PageNode
            {
                Id = 1,
                PageNodes = new Collection<PageNode>
                {
                    new PageNode { Id = 2 }
                }
            };
            _dbContextMock.Setup(x => x.GetDbSet<PageNode>()).Returns(new InMemoryDbSet<PageNode>
            {
                pageNode
            });

            //act
            _pageNodeService.Delete(pageNode.Id);

            //assert
            var deleltedPageNode = _dbContextMock.Object.GetDbSet<PageNode>().FirstOrDefault(x => x.Id == pageNode.PageNodes.First().Id);
            deleltedPageNode.Should().Be.Null();
        }

        #endregion
    }
}
