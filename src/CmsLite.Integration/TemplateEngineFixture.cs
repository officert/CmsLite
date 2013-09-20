using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using CmsLite.Core.Ioc;
using CmsLite.Data.Ioc;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Content;
using CmsLite.Interfaces.Data;
using CmsLite.Services.Ioc;
using NUnit.Framework;
using Ninject;
using Ninject.Modules;
using SharpTestsEx;

namespace CmsLite.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class TemplateEngineFixture : IDisposable
    {
        private IKernel _kernel;
        private IDbContext _dbContext;
        private IFileManager _fileManager;
        private Assembly _assembly;
        private const int NumValidControllersInCurrentProject = 1;      //as new 'valid' controllers are added to this project, you will need to bump up this number
        private const int NumValidActionsOnController = 2;              //as new 'valid' action are added to the TestController1_Valid.cs, you will need to bump up this number
        public const int NumValidModelsInCurrentProject = 1;
        private const int NumValidPropertiesOnModel = 1;                //as new 'valid' properties are added to the HomeModel.cs, you will need to bump up this number

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new List<INinjectModule>
                             {
                                 new CmsIocModule(),
                                 new DataNinectModule(),
                                 new ServicesNinjectModule()
                             });
            _dbContext = _kernel.Get<IDbContext>();

            _fileManager = _kernel.Get<IFileManager>();

            _assembly = Assembly.GetExecutingAssembly();

            DeleteAllSectionTemplates();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteAllSectionTemplates();
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            _kernel.Dispose();
        }

        #region Creating SectionTemplates

        [Test]
        public void ProcessMvcFiles_ControllerDoesNotSubclassCmsController_DoesNotCreateSectionTemplate()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var sectionTemplates = GetSectionTemplates();
            sectionTemplates.Count().Should().Be.EqualTo(NumValidControllersInCurrentProject);
        }

        [Test]
        public void ProcessMvcFiles_ControllerDoesHaveCmsSectionTemplateAttribute_DoesNotCreateSectionTemplate()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var sectionTemplates = GetSectionTemplates();
            sectionTemplates.Count().Should().Be.EqualTo(NumValidControllersInCurrentProject);
        }

        [Test]
        public void ProcessMvcFiles_ControllerHasAttributeAndSubClassCmsController_CreatesSectionTemplate()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplates = GetSectionTemplates();
            validTemplates.Count().Should().Be.EqualTo(NumValidControllersInCurrentProject);
        }

        [Test]
        public void ProcessMvcFiles_ControllerCmsAttributeHasName_CreatesSectionTemplateUsingNameAsTemplateName()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplates = GetSectionTemplates();
            validTemplates.FirstOrDefault(x => x.ControllerName == "TestController1_Valid").Name.Should().Be.EqualTo("TestController1");
        }

        [Test]
        public void ProcessMvcFiles_ControllerWithActions_CreatesSectionTemplateWithPageTemplateForEachAction()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplates = GetSectionTemplates();
            validTemplates.FirstOrDefault(x => x.ControllerName == "TestController1_Valid").PageTemplates.Count.Should().Be.EqualTo(NumValidActionsOnController);
        }

        #endregion

        #region Deleting SectionTemplates

        [Test]
        public void ProcessMvcFiles_NoControllerExistsForSectionTemplate_DeletesSectionTemplate()
        {
            //arrange
            const string controllerName = "Foobar";
            var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
            var sectionTemplate = sectionTemplateDbSet.Create();
            sectionTemplate.ControllerName = controllerName;
            sectionTemplate.Name = "barfoo";
            sectionTemplateDbSet.Add(sectionTemplate);
            _dbContext.SaveChanges();

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var cleanedUpSectionTemplates = GetSectionTemplates().Where(x => x.ControllerName == controllerName);
            cleanedUpSectionTemplates.Count().Should().Be.EqualTo(0);
        }

        //[Test]
        //public void SynchronizeControllerWithSectionTemplates_DeletingSectionTemplate_RemovesAllPageTemplatesParentedBySectionTemplate()
        //{
        //    //arrange
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();

        //    const string controllerName = "Foobar";
        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "barfoo";
        //    sectionTemplate.PageTemplates = new Collection<PageTemplate>
        //                                        {
        //                                            new PageTemplate { ActionName = "Blah1", DisplayName = "Blah Blah1", ModelTypeName = "Home" },
        //                                            new PageTemplate { ActionName = "Blah2", DisplayName = "Blah Blah2", ModelTypeName = "Home" }
        //                                        };
        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    var createdSectionTemplate = sectionTemplateDbSet.FirstOrDefault(x => x.ControllerName == controllerName);
        //    sectionTemplateDbSet.Remove(createdSectionTemplate);
        //    _dbContext.SaveChanges();

        //    //assert
        //    var deletedSectionTemplate = sectionTemplateDbSet.FirstOrDefault(x => x.ControllerName == controllerName);
        //    deletedSectionTemplate.Should().Be.Null();
        //    var pageTemplatesForDeletedController = _dbContext.GetDbSet<PageTemplate>().Include(x => x.ParentSectionTemplate).Where(x => x.ParentSectionTemplate.ControllerName == controllerName);
        //    pageTemplatesForDeletedController.Should().Be.Empty();
        //}

        [Test]
        public void SynchronizeControllerWithSectionTemplates_DeletingSectionTemplate_RemovesAllSectionNodesUsingSectionTemplate()
        {
            //arrange
            var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();

            const string controllerName = "Foobar";
            var sectionTemplate = sectionTemplateDbSet.Create();
            sectionTemplate.ControllerName = controllerName;
            sectionTemplate.Name = "barfoo";
            sectionTemplate.SectionNodes = new Collection<SectionNode>
                                               {
                                                   new SectionNode
                                                       {
                                                           DisplayName = "pow1",
                                                           UrlName = "pow1",
                                                           Order = 1
                                                       },
                                                   new SectionNode
                                                       {
                                                           DisplayName = "pow2",
                                                           UrlName = "pow2",
                                                           Order = 2
                                                       }
                                               };
            sectionTemplateDbSet.Add(sectionTemplate);
            _dbContext.SaveChanges();

            //act
            var createdSectionTemplate = sectionTemplateDbSet.FirstOrDefault(x => x.ControllerName == controllerName);
            sectionTemplateDbSet.Remove(createdSectionTemplate);
            _dbContext.SaveChanges();

            //assert
            var deletedSectionTemplate = sectionTemplateDbSet.FirstOrDefault(x => x.ControllerName == controllerName);
            deletedSectionTemplate.Should().Be.Null();
            var sectionNodesWithDeletedSectionTemplate = _dbContext.GetDbSet<SectionNode>().Include(x => x.SectionTemplate).Where(x => x.SectionTemplate.ControllerName == controllerName);
            sectionNodesWithDeletedSectionTemplate.Should().Be.Empty();
        }

        #endregion

        #region Updating SectionTemplates

        [Test]
        public void ProcessMvcFiles_SectionTemplateAlreadyExists_UpdatesTemplateName()
        {
            //arrange
            const string controllerName = "TestController1_Valid";
            var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
            var sectionTemplate = sectionTemplateDbSet.Create();
            sectionTemplate.ControllerName = controllerName;
            sectionTemplate.Name = "TemplateName";
            sectionTemplateDbSet.Add(sectionTemplate);
            _dbContext.SaveChanges();

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplate = GetSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
            validTemplate.Name.Should().Be.EqualTo("TestController1");
        }

        #region Deleting PageTemplates

        //[Test]
        //public void SynchronizeControllerWithSectionTemplates_NoActionExistsForPageTemplate_DeletesPageTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();
        //    pageTemplate.ActionName = "foobar";
        //    pageTemplate.DisplayName = "foobar";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SynchronizeControllerWithSectionTemplates(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    validTemplate.PageTemplates.Count.Should().Be.EqualTo(NumValidActionsOnController);
        //}

        //[Test]
        //public void SynchronizeControllerWithSectionTemplates_DeletingPageTemplate_RemovesAllPageNodesUsingPageTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>();
        //    var pageNodeDbSet = _dbContext.GetDbSet<PageNode>();

        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    var sectionNode = sectionNodeDbSet.CreateForSectionTemplate();
        //    sectionNode.SectionTemplate = sectionTemplate;
        //    sectionNode.UrlName = "foobar";

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();
        //    pageTemplate.ActionName = "foobar";
        //    pageTemplate.DisplayName = "foobar";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;

        //    var pageNode = pageNodeDbSet.CreateForSectionTemplate();
        //    pageNode.UrlName = "pagenode";
        //    pageNode.DisplayName = "pagenode";
        //    pageNode.ParentSectionNode = sectionNode;
        //    pageNode.PageTemplate = pageTemplate;

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SynchronizeControllerWithSectionTemplates(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    validTemplate.PageTemplates.Count.Should().Be.EqualTo(NumValidActionsOnController);

        //    pageNodeDbSet.Any(x => x.DisplayName == "foobar").Should().Be.False();
        //}

        #endregion

        #region Creating PageTemplates

        [Test]
        public void ProcessMvcFiles_ActionWithoutCmsPageTemplateAttribute_DoesNotCreatePageTemplate()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplates = GetSectionTemplates();
            validTemplates.Count().Should().Be.EqualTo(NumValidControllersInCurrentProject);
        }

        [Test]
        public void ProcessMvcFiles_ActionWithoutActionResultReturnValue_DoesNotCreatePageTemplate()
        {
            //arrange

            //act
            _fileManager.ProcessMvcFiles(_assembly);

            //assert
            var validTemplates = GetSectionTemplates();
            validTemplates.Count().Should().Be.EqualTo(NumValidControllersInCurrentProject);
        }

        #endregion

        #region Updating PageTemplates

        //[Test]
        //public void SynchronizeControllerWithSectionTemplates_PageTemplateAlreadyExists_UpdatesDisplayName()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();
        //    pageTemplate.ActionName = "AboutMe";                //create a pagetemplate using an ActionName of a real action, give it a different display name than what is in the PageTemplate attribute
        //    pageTemplate.DisplayName = "foobar";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SynchronizeControllerWithSectionTemplates(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe").DisplayName.Should().Be.EqualTo("About Me Page");
        //}

        //[Test]
        //public void SynchronizeControllerWithSectionTemplates_ChangingPageTemplateModelType_UpdatesModelNameOnPageTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.DisplayName = "foobar";
        //    pageTemplate.ModelTypeName = "FoobarModel";                                     //create a page template with a Model name that is different that the Model name on the actual controller
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;
        //    pageTemplate.PropertyTemplates = new Collection<PropertyTemplate>
        //                                         {
        //                                             new PropertyTemplate
        //                                                 {
        //                                                     DisplayName = "foobar property 1",
        //                                                     PropertyName = "foobar1",
        //                                                     CmsPropertyType = CmsPropertyTypes.RichTextEditor
        //                                                 },
        //                                             new PropertyTemplate
        //                                                 {
        //                                                     DisplayName = "foobar property 2",
        //                                                     PropertyName = "foobar2",
        //                                                     CmsPropertyType = CmsPropertyTypes.RichTextEditor
        //                                                 }
        //                                         };

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SynchronizeControllerWithSectionTemplates(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe").ModelTypeName.Should().Be.EqualTo("HomeModel");
        //}

        //#region Deleting PropertyTemplates

        //[Test]
        //public void SyncMvcFiles_NoProperyExistsForPropertyTemplate_DeletesPropertyTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();                      //create a page template that matches the actual Action on our valid Controller
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;
        //    pageTemplate.PropertyTemplates = new Collection<PropertyTemplate>
        //                                         {
        //                                             new PropertyTemplate       //add a property template that does NOT actually exist on the controller, that way it should run through the delete path
        //                                                 {
        //                                                     DisplayName = "foobar property 1",
        //                                                     PropertyName = "foobar",
        //                                                     CmsPropertyType = CmsPropertyTypes.RichTextEditor
        //                                                 }
        //                                         };

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagePropertyTemplates = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe").PropertyTemplates;
        //    pagePropertyTemplates.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);

        //    var propertyTemplate = pagePropertyTemplates.FirstOrDefault();
        //    propertyTemplate.PropertyName.Should().Be.EqualTo("BannerTextLeft");
        //}

        //[Test]
        //public void SyncMvcFiles_DeletingPropertyTemplate_RemovesAllPropertiesUsingPropertyTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>();

        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var sectionNode = sectionNodeDbSet.CreateForSectionTemplate();
        //    sectionNode.SectionTemplate = sectionTemplate;
        //    sectionNode.DisplayName = "foobarsection";
        //    sectionNode.UrlName = "foobarsection";

        //    HackToInstantiatePageNodesCollection(sectionNode);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();                      //create a page template that matches the actual Action on our valid Controller
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;
        //    pageTemplate.PropertyTemplates = new Collection<PropertyTemplate>
        //                                         {
        //                                             new PropertyTemplate       //add a property template that does NOT actually exist on the controller, that way it should run through the delete path
        //                                                 {
        //                                                     DisplayName = "foobar property 1",
        //                                                     PropertyName = "foobar",
        //                                                     CmsPropertyType = CmsPropertyTypes.RichTextEditor
        //                                                 }
        //                                         };
        //    var newpageNode = new PageNode
        //                          {
        //                              DisplayName = "Foobar Page",
        //                              UrlName = "FoobarPage",
        //                              PageTemplate = pageTemplate,
        //                              Order = 1,
        //                              Properties =
        //                                  new Collection<Property>
        //                                  //Property uses a Property Template that will be deleted, so this property should also get deleted
        //                                      {
        //                                          new Property
        //                                              {
        //                                                  Text = "sdnfasnlfkalksdf",
        //                                                  PropertyTemplate = pageTemplate.PropertyTemplates.First()
        //                                              }
        //                                      }
        //                          };
        //    sectionNode.PageNodes.Add(newpageNode);

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    var pageNode = pagetemplate.PageNodes.First();
        //    pageNode.Properties.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);   //this isn't zero becuase our model has a valid property template which create a property on the page node
        //}

        //#endregion

        //#region Creating PropertyTemplates

        //[Test]
        //public void SyncMvcFiles_PropertyWithoutCmsModelPropertyAttribute_DoesNotCreatePropertyTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    pagetemplate.PropertyTemplates.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);   //this isn't zero becuase our model has a valid property template which create a property on the page node
        //}

        //[Test]
        //public void SyncMvcFiles_PrivateProperty_DoesNotCreatePropertyTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    pagetemplate.PropertyTemplates.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);   //this isn't zero becuase our model has a valid property template which create a property on the page node
        //}

        //[Test]
        //public void SyncMvcFiles_ParentPageTemplateHasPageNode_CreatesNewPropertiesOnPageNode()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>();

        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var sectionNode = sectionNodeDbSet.CreateForSectionTemplate();
        //    sectionNode.SectionTemplate = sectionTemplate;
        //    sectionNode.DisplayName = "foobarsection";
        //    sectionNode.UrlName = "foobarsection";

        //    HackToInstantiatePageNodesCollection(sectionNode);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();                      //create a page template that matches the actual Action on our valid Controller
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;

        //    var newpageNode = new PageNode                                      //create a page node with no properties. because the page template it uses already exists it will get updated
        //    {                                                                   //and a new property should be added for each valid property template on the page template
        //        DisplayName = "Foobar Page",
        //        UrlName = "FoobarPage",
        //        PageTemplate = pageTemplate,
        //        Order = 1
        //    };
        //    sectionNode.PageNodes.Add(newpageNode);

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    var pageNode = pagetemplate.PageNodes.First();
        //    pageNode.Properties.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);
        //}

        //#endregion

        //#region Updating PropertyTemplates

        //[Test]
        //public void SyncMvcFiles_PropertyTemplateAlreadyExists_UpdatesPropertyTemplateProperties()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();

        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();                      //create a page template that matches the actual Action on our valid Controller
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;

        //    HackToInstantiatePropertyTemplatesCollection(pageTemplate);

        //    pageTemplate.PropertyTemplates.Add(new PropertyTemplate
        //                                           {
        //                                               PropertyName = "BannerTextLeft", //this is the name of a valid property on the HomeModel, so it should get updated,
        //                                               CmsPropertyType = CmsPropertyTypes.RichTextEditor,
        //                                               DisplayName = "foobar",
        //                                               TabName = "foobar",
        //                                               TabOrder = 99,
        //                                               Description = "foobar",
        //                                               Required = true
        //                                           });

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    var propertyTemplate = pagetemplate.PropertyTemplates.FirstOrDefault(x => x.PropertyName == "BannerTextLeft");

        //    propertyTemplate.Should().Not.Be.Null();

        //    propertyTemplate.DisplayName.Should().Be.EqualTo("Banner Text Left");
        //    propertyTemplate.TabName.Should().Be.EqualTo("Banner");
        //    propertyTemplate.TabOrder.Should().Be.EqualTo(1);
        //    propertyTemplate.Description.Should().Be.EqualTo("Text to go inside of the banner.");
        //    propertyTemplate.Required.Should().Be.EqualTo(false);
        //}

        //[Test]
        //public void SyncMvcFiles_ChangingPropertyTemplatePropertyType_ResetsTextOfAnyPropertiesUsingThatPropertyTemplate()
        //{
        //    //arrange
        //    const string controllerName = "TestController1_Valid";
        //    var sectionTemplateDbSet = _dbContext.GetDbSet<SectionTemplate>();
        //    var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
        //    var sectionNodeDbSet = _dbContext.GetDbSet<SectionNode>();

        //    var sectionTemplate = sectionTemplateDbSet.CreateForSectionTemplate();
        //    sectionTemplate.ControllerName = controllerName;
        //    sectionTemplate.Name = "TemplateName";

        //    HackToInstantiatePageTemplatesCollection(sectionTemplate);

        //    var sectionNode = sectionNodeDbSet.CreateForSectionTemplate();
        //    sectionNode.SectionTemplate = sectionTemplate;
        //    sectionNode.DisplayName = "foobarsection";
        //    sectionNode.UrlName = "foobarsection";

        //    HackToInstantiatePageNodesCollection(sectionNode);

        //    var pageTemplate = pageTemplateDbSet.CreateForSectionTemplate();                      //create a page template that matches the actual Action on our valid Controller
        //    pageTemplate.ActionName = "AboutMe";
        //    pageTemplate.ModelTypeName = "HomeModel";
        //    pageTemplate.ParentSectionTemplate = sectionTemplate;
        //    pageTemplate.PropertyTemplates = new Collection<PropertyTemplate>
        //                                         {
        //                                            new PropertyTemplate        //create a property template that matches the property in the Home Model, that way it will get updated
        //                                            {
        //                                                PropertyName = "BannerTextLeft",
        //                                                CmsPropertyType = CmsPropertyTypes.ImagePicker  //use a different property type
        //                                            }
        //                                         };
        //    var newpageNode = new PageNode
        //    {
        //        DisplayName = "Foobar Page",
        //        UrlName = "FoobarPage",
        //        PageTemplate = pageTemplate,
        //        Order = 1,
        //        Properties =
        //            new Collection<Property>
        //            {
        //                new Property
        //                    {
        //                        Text = "sdnfasnlfkalksdf",
        //                        PropertyTemplate = pageTemplate.PropertyTemplates.First()       //create a property using the property template that will get updated
        //                    }
        //            }
        //    };
        //    sectionNode.PageNodes.Add(newpageNode);

        //    sectionTemplate.PageTemplates.Add(pageTemplate);

        //    sectionTemplateDbSet.Add(sectionTemplate);
        //    _dbContext.SaveChanges();

        //    //act
        //    _fileManager.SyncMvcFiles(_assembly);

        //    //assert
        //    var validTemplate = GetTestSectionTemplates().FirstOrDefault(x => x.ControllerName == controllerName);
        //    var pagetemplate = validTemplate.PageTemplates.FirstOrDefault(x => x.ActionName == "AboutMe");

        //    pagetemplate.PropertyTemplates.FirstOrDefault(x => x.PropertyName == "BannerTextLeft").CmsPropertyType.Should().Be.EqualTo(CmsPropertyTypes.RichTextEditor);

        //    var pageNode = pagetemplate.PageNodes.First();
        //    pageNode.Properties.Count.Should().Be.EqualTo(NumValidPropertiesOnModel);   //this isn't zero becuase our model has a valid property template which create a property on the page node
        //    pageNode.Properties.FirstOrDefault().Text.Should().Be.NullOrEmpty();
        //}

        //#endregion

        #endregion

        #endregion

        #region Private Helpers

        private void DeleteAllSectionTemplates()
        {
            var dbContext = _kernel.Get<IDbContext>();
            var sectionTemplateDbSet = dbContext.GetDbSet<SectionTemplate>();
            foreach (var sectionTemplate in sectionTemplateDbSet)
            {
                sectionTemplateDbSet.Remove(sectionTemplate);
            }
            dbContext.SaveChanges();
        }

        private IEnumerable<SectionTemplate> GetSectionTemplates()
        {
            var dbContext = _kernel.Get<IDbContext>();
            return dbContext.GetDbSet<SectionTemplate>()
                .Include(x => x.PageTemplates);
        }

        private void HackToInstantiatePageTemplatesCollection(SectionTemplate sectionTemplate)
        {
            //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
            if (sectionTemplate.PageTemplates == null)
            {
                var pageTemplateDbSet = _dbContext.GetDbSet<PageTemplate>();
                var newPageTemplate = pageTemplateDbSet.Create();
                newPageTemplate.ParentSectionTemplate = sectionTemplate;
                pageTemplateDbSet.Add(newPageTemplate);
                pageTemplateDbSet.Remove(newPageTemplate);
            }
        }

        private void HackToInstantiatePageNodesCollection(SectionNode sectionNode)
        {
            //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
            if (sectionNode.PageNodes == null)
            {
                var pageNodeDbSet = _dbContext.GetDbSet<PageNode>();
                var newPageNode = pageNodeDbSet.Create();
                newPageNode.ParentSectionNode = sectionNode;
                pageNodeDbSet.Add(newPageNode);
                pageNodeDbSet.Remove(newPageNode);
            }
        }

        #endregion
    }
}
