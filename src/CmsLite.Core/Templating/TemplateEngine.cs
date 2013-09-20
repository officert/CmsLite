using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Controllers;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Interfaces.Templating;
using CmsLite.Resources;
using CmsLite.Utilities.Cms;
using CmsLite.Utilities.Extensions;

namespace CmsLite.Core.Templating
{
    public class TemplateEngine : ITemplateEngine
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISectionNodeService _sectionNodeService;
        private readonly ISectionTemplateService _sectionTemplateService;
        private readonly IPageNodeService _pageNodeService;
        private readonly IPageTemplateService _pageTemplateService;
        private readonly IPropertyTemplateService _propertyTemplateService;

        public TemplateEngine(IUnitOfWork unitOfWork,
            ISectionNodeService sectionNodeService,
            ISectionTemplateService sectionTemplateService,
            IPageNodeService pageNodeService,
            IPageTemplateService pageTemplateService,
            IPropertyTemplateService propertyTemplateService)
        {
            _unitOfWork = unitOfWork;
            _sectionNodeService = sectionNodeService;
            _sectionTemplateService = sectionTemplateService;
            _pageNodeService = pageNodeService;
            _pageTemplateService = pageTemplateService;
            _propertyTemplateService = propertyTemplateService;
        }

        #region Implementation of IMvcFileManager

        public void ProcessMvcFiles(Assembly assembly)
        {
            var sectionTemplates = _unitOfWork.Context.GetDbSet<SectionTemplate>()
                                                .Include(x => x.PageTemplates)
                                                .Include(x => x.SectionNodes.Select(sn => sn.PageNodes.Select(pn => pn.Properties)));

            ProcessControllers(assembly, sectionTemplates);

            _unitOfWork.Commit();

            PostProcessActions(assembly, sectionTemplates);

            _unitOfWork.Commit();
        }

        public Type GetModelType(Assembly assembly, string modelTypeName)
        {
            var modelInstance = GetModels(assembly).FirstOrDefault(x => x.Name == modelTypeName);

            if (modelInstance == null)
                throw new ArgumentException(string.Format(Messages.ModelTypeNotFound, modelTypeName));

            return modelInstance;
        }

        #endregion

        private void ProcessControllers(Assembly assembly, IQueryable<SectionTemplate> sectionTemplates)
        {
            //find all the Cms controllers
            var controllerTypes = GetControllers(assembly).ToList();

            var controllerNames = controllerTypes.Select(y => y.Name).ToList();
            var sectionTemplateControllerNames = sectionTemplates.Select(x => x.ControllerName).ToList();

            var sectionTemplatesToRemove = sectionTemplates.Where(x => !controllerNames.Contains(x.ControllerName)).ToList();
            var newControllerTypes = controllerTypes.Where(x => !sectionTemplateControllerNames.Contains(x.Name)).ToList();
            var sectionTemplatesToUpdate = sectionTemplates.Where(x => controllerNames.Contains(x.ControllerName)).ToList();

            //remove any templates that don't have a controller anymore
            RemoveSectionTemplatesWithNoExistingController(sectionTemplatesToRemove);

            //create templates for any controllers that don't have a template with that controller name
            CreateSectionTemplatesForNewControllers(assembly, newControllerTypes);

            //update any templates that still have a controller
            UpdateSectionTemplates(assembly, sectionTemplatesToUpdate, controllerTypes);
        }

        private void PostProcessActions(Assembly assembly, IQueryable<SectionTemplate> sectionTemplates)
        {
            if (sectionTemplates != null && sectionTemplates.Any())
            {
                var pageTemplateGroupings = sectionTemplates.SelectMany(x => x.PageTemplates).GroupBy(x => x.ParentSectionTemplate.ControllerName).ToList();
                var controllers = GetControllers(assembly).ToList();

                if (pageTemplateGroupings.Any())
                {
                    foreach (var pageTemplateGroup in pageTemplateGroupings)
                    {
                        var count = pageTemplateGroupings.Count;
                        var controller = controllers.FirstOrDefault(x => x.Name == pageTemplateGroup.First().ParentSectionTemplate.ControllerName);
                        var actions = GetActionsForController(controller);

                        var grouping = pageTemplateGroup;
                        foreach (var pageTemplate in grouping)
                        {
                            HackToInstantiateAllowedChildPageTemplatesCollection(pageTemplate);

                            var templateAction = actions.FirstOrDefault(x => x.Name == pageTemplate.ActionName);

                            if (templateAction == null) break;

                            var pageTemplateAttribute = (CmsPageTemplateAttribute)templateAction.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).FirstOrDefault();

                            var allowedChildPageTemplateName = pageTemplateAttribute.AllowedChildPageTemplates;

                            if (allowedChildPageTemplateName == null) break;

                            foreach (var pageTemplateName in allowedChildPageTemplateName)
                            {
                                var pageTemplateToAdd = pageTemplateGroup.First(x => x.ActionName == pageTemplateName);

                                if (pageTemplateToAdd == null) throw new ArgumentException(string.Format("The action {0} does not exist on the controller {1}.", pageTemplateName, controller.Name));

                                pageTemplate.PageTemplates.Add(pageTemplateToAdd);
                            }
                        }
                    }
                }
            }
        }

        #region Add/Update/Delete Section Templates

        private void RemoveSectionTemplatesWithNoExistingController(IEnumerable<SectionTemplate> sectionTemplates)
        {
            if (sectionTemplates != null && sectionTemplates.Any())
            {
                foreach (var sectionTemplate in sectionTemplates.ToList())
                {
                    _sectionTemplateService.Delete(sectionTemplate.Id, false);
                }
            }
        }

        private void CreateSectionTemplatesForNewControllers(Assembly assembly, IEnumerable<Type> controllers)
        {
            if (controllers != null && controllers.Any())
            {
                foreach (var controller in controllers)
                {
                    var attribute = (CmsSectionTemplateAttribute)controller.GetCustomAttributes(typeof(CmsSectionTemplateAttribute), false).FirstOrDefault();

                    var sectionTemplate = _sectionTemplateService.Create(controller.Name, attribute.Name, commit: false);

                    var controllerActions = GetActionsForController(controller);

                    CreatePageTemplatesForActions(sectionTemplate, controllerActions, assembly);
                }
            }
        }

        private void UpdateSectionTemplates(Assembly assembly, IEnumerable<SectionTemplate> sectionTemplates, IEnumerable<Type> controllers)
        {
            if (sectionTemplates != null && sectionTemplates.Any())
            {
                foreach (var sectionTemplate in sectionTemplates)
                {
                    var templateController = controllers.FirstOrDefault(x => x.Name == sectionTemplate.ControllerName);

                    if (templateController == null)
                        throw new ArgumentException(string.Format(Messages.ControllerNotFound, sectionTemplate.ControllerName));

                    var controllerActions = GetActionsForController(templateController).ToList();

                    var controllerActionNames = controllerActions.Select(x => x.Name).ToList();
                    var sectionActionNames = sectionTemplate.PageTemplates.Select(x => x.ActionName).ToList();

                    var sectionTemplateAttribute = (CmsSectionTemplateAttribute)templateController.GetCustomAttributes(typeof(CmsSectionTemplateAttribute), false).FirstOrDefault();

                    if (sectionTemplateAttribute == null)
                        throw new ArgumentException(string.Format(Messages.ControllerDoesNotHaveCmsSectionTempalteAttribute, templateController.Name));

                    //update properties on the section template
                    _sectionTemplateService.Update(sectionTemplate, sectionTemplateAttribute.Name, sectionTemplateAttribute.IconImageName, false);

                    var pageTemplatesToRemove = sectionTemplate.PageTemplates.Where(x => !controllerActionNames.Contains(x.ActionName)).ToList();
                    var newActionsToAdd = controllerActions.Where(x => !sectionActionNames.Contains(x.Name)).ToList();
                    var pageTemplatesToUpdate = sectionTemplate.PageTemplates.Where(x => controllerActionNames.Contains(x.ActionName)).ToList();

                    //remove any page templates that don't have an action anymore
                    RemovePageTemplatesWithNoExistingAction(pageTemplatesToRemove);

                    //create page templates for any actions that don't have a template with that action name
                    CreatePageTemplatesForActions(sectionTemplate, newActionsToAdd, assembly);

                    //update any page templates that still have an action
                    UpdatePageTemplates(assembly, pageTemplatesToUpdate, controllerActions);
                }
            }
        }

        #endregion

        #region Add/Update/Delete Page Templates

        private void RemovePageTemplatesWithNoExistingAction(IEnumerable<PageTemplate> pageTemplates)
        {
            if (pageTemplates != null && pageTemplates.Any())
            {
                foreach (var pageTemplate in pageTemplates.ToList())
                {
                    _pageTemplateService.Delete(pageTemplate.Id, false);
                }
            }
        }

        private void CreatePageTemplatesForActions(SectionTemplate sectionTemplate, IEnumerable<MethodInfo> controllerActions, Assembly assembly)
        {
            foreach (var action in controllerActions)
            {
                var attribute = (CmsPageTemplateAttribute)action.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).FirstOrDefault();
                var model = GetModelType(assembly, attribute.ModelType.Name);

                var pageTemplate = _pageTemplateService.CreateForSectionTemplate(sectionTemplate, action.Name, model.Name, attribute.Name, attribute.IconImageName, false);

                var modelProperties = GetModelProperties(model);
                CreatePropertyTemplatesForProperties(modelProperties, pageTemplate);
            }
        }

        private void UpdatePageTemplates(Assembly assembly, IEnumerable<PageTemplate> pageTemplates, IEnumerable<MethodInfo> actions)
        {
            if (pageTemplates != null && pageTemplates.Any())
            {
                foreach (var pageTemplate in pageTemplates)
                {
                    var templateAction = actions.FirstOrDefault(x => x.Name == pageTemplate.ActionName);

                    if (templateAction == null)
                        throw new ArgumentException(string.Format(Messages.ActionNotFound, pageTemplate.ActionName));

                    var pageTemplateAttribute = (CmsPageTemplateAttribute)templateAction.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).FirstOrDefault();

                    if (pageTemplateAttribute == null)
                        throw new ArgumentException(string.Format(Messages.ActionDoesNotHaveCmsPageTemplateAttribute, templateAction.Name));

                    var model = GetModelType(assembly, pageTemplate.ModelName);
                    var modelProperties = GetModelProperties(model).ToList();
                    
                    _pageTemplateService.Update(pageTemplate, model.Name, pageTemplateAttribute.Name, pageTemplateAttribute.IconImageName, false);

                    var modelPropertyNames = modelProperties.Select(x => x.Name).ToList();
                    var pageTemplatePropertyNames = pageTemplate.PropertyTemplates.Select(x => x.PropertyName).ToList();

                    var propertyTemplatesToRemove = pageTemplate.PropertyTemplates.Where(x => !modelPropertyNames.Contains(x.PropertyName)).ToList();
                    var newPropertiesToAdd = modelProperties.Where(x => !pageTemplatePropertyNames.Contains(x.Name)).ToList();
                    var propertyTemplatesToUpdate = pageTemplate.PropertyTemplates.Where(x => modelPropertyNames.Contains(x.PropertyName)).ToList();

                    //remove any property templates that don't have a property anymore
                    RemovePropertyTemplatesWithNoExistingModelProperties(propertyTemplatesToRemove);

                    //create property templates for any properties that don't have a template with that property name
                    CreatePropertyTemplatesForProperties(newPropertiesToAdd, pageTemplate);

                    //update any property templates that still have a model property
                    UpdatePropertyTemplates(propertyTemplatesToUpdate, modelProperties);
                }
            }
        }

        #endregion

        #region Add/Update/Delete Property Templates

        private void RemovePropertyTemplatesWithNoExistingModelProperties(IEnumerable<PropertyTemplate> propertyTemplates)
        {
            if (propertyTemplates != null && propertyTemplates.Any())
            {
                foreach (var propertyTemplate in propertyTemplates.ToList())
                {
                    _propertyTemplateService.Delete(propertyTemplate.Id, false);
                }
            }
        }

        private void CreatePropertyTemplatesForProperties(IEnumerable<PropertyInfo> properties, PageTemplate pageTemplate)
        {
            if (properties != null && properties.Any())
            {
                foreach (var property in properties)
                {
                    var attribute = (CmsModelPropertyAttribute)property.GetCustomAttributes(typeof(CmsModelPropertyAttribute), false).FirstOrDefault();
                    var propertyName = property.Name;
                    var displayName = attribute.DisplayName;
                    var propertyType = attribute.PropertyType;
                    var description = attribute.Description;
                    var tabName = attribute.TabName ?? CmsConstants.DefaultTabName;
                    var tabOrder = attribute.TabOrder;
                    var required = attribute.Required;

                    _propertyTemplateService.Create(pageTemplate, propertyName, propertyType, tabOrder, tabName, required, description, displayName, false);
                }
            }
        }

        private void UpdatePropertyTemplates(IEnumerable<PropertyTemplate> propertyTemplates, IEnumerable<PropertyInfo> properties)
        {
            if (propertyTemplates != null && propertyTemplates.Any())
            {
                foreach (var propertyTemplate in propertyTemplates)
                {
                    var templateProperty = properties.FirstOrDefault(x => x.Name == propertyTemplate.PropertyName);

                    if (templateProperty == null)
                        throw new ArgumentException(string.Format("The property with the name {0} does not exist", propertyTemplate.PropertyName));

                    var propertyTemplateAttribute = (CmsModelPropertyAttribute)templateProperty.GetCustomAttributes(typeof(CmsModelPropertyAttribute), false).FirstOrDefault();

                    if (propertyTemplateAttribute == null)
                        throw new ArgumentException(string.Format("The property {0} does not have a CmsModelProperty attribute", templateProperty.Name));

                    propertyTemplate.DisplayName = propertyTemplateAttribute.DisplayName;
                    propertyTemplate.TabName = propertyTemplateAttribute.TabName;
                    propertyTemplate.TabOrder = propertyTemplateAttribute.TabOrder;
                    propertyTemplate.Description = propertyTemplateAttribute.Description;
                    propertyTemplate.Required = propertyTemplateAttribute.Required;     //what are the reprocussions of changing a property template to required?

                    if (propertyTemplate.CmsPropertyType != propertyTemplateAttribute.PropertyType.ToString())
                    {
                        propertyTemplate.CmsPropertyType = propertyTemplateAttribute.PropertyType.ToString();

                        var existingProperties = propertyTemplate.Properties;
                        foreach (var existingProperty in existingProperties)
                        {
                            existingProperty.Text = "";
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Helpers

        private static IEnumerable<Type> GetControllers(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CmsBaseController)) && t.GetCustomAttributes(typeof(CmsSectionTemplateAttribute), false).Length > 0).ToList();
        }

        private static IEnumerable<MethodInfo> GetActionsForController(Type controller)
        {
            return controller == null
                        ? null
                        : controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(action => action.ReturnType == typeof(ActionResult) && action.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).Length > 0);
        }

        private static IEnumerable<Type> GetModels(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(CmsModelTemplateAttribute), false).Length > 0).ToList();
        }

        private static IEnumerable<PropertyInfo> GetModelProperties(Type model)
        {
            return model.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttributes(typeof(CmsModelPropertyAttribute), false).Length > 0).ToList();
        }

        //private void HackToInstantiatePageTemplatesCollection(SectionTemplate sectionTemplate)
        //{
        //    //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
        //    if (sectionTemplate.PageTemplates == null)
        //    {
        //        var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();
        //        var newPageTemplate = pageTemplateDbSet.Create();
        //        newPageTemplate.ParentSectionTemplate = sectionTemplate;
        //        pageTemplateDbSet.Add(newPageTemplate);
        //        pageTemplateDbSet.Remove(newPageTemplate);
        //    }
        //}

        //private void HackToInstantiatePropertyTemplatesCollection(PageTemplate pageTemplate)
        //{
        //    //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
        //    if (pageTemplate.PropertyTemplates == null)
        //    {
        //        var propertyTemplateDbSet = _unitOfWork.Context.GetDbSet<PropertyTemplate>();
        //        var newPropertyTemplate = propertyTemplateDbSet.Create();
        //        newPropertyTemplate.ParentPageTemplate = pageTemplate;
        //        propertyTemplateDbSet.Add(newPropertyTemplate);
        //        propertyTemplateDbSet.Remove(newPropertyTemplate);
        //    }
        //}

        private void HackToInstantiateAllowedChildPageTemplatesCollection(PageTemplate pageTemplate)
        {
            //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
            if (pageTemplate.PageTemplates == null)
            {
                var pageTemplateDbSet = _unitOfWork.Context.GetDbSet<PageTemplate>();
                var newPageTemplate = pageTemplateDbSet.Create();
                newPageTemplate.ParentPageTemplate = pageTemplate;
                pageTemplateDbSet.Add(newPageTemplate);
                pageTemplateDbSet.Remove(newPageTemplate);
            }
        }

        //private void AddPropertiesToPageNodeForNewPropertyTemplates(PageTemplate pageTemplate)
        //{
        //    if (pageTemplate.PageNodes != null && pageTemplate.PageNodes.Any())
        //    {
        //        foreach (var pageNode in pageTemplate.PageNodes)
        //        {
        //            var pageNodePropertyNames = pageNode.Properties.Select(x => x.PropertyTemplate.PropertyName);

        //            var propertiesToAdd = pageTemplate.PropertyTemplates.Where(x => !pageNodePropertyNames.Contains(x.PropertyName));

        //            var propertyDbSet = _unitOfWork.Context.GetDbSet<Property>();

        //            foreach (var propertyTemplate in propertiesToAdd)
        //            {
        //                var newProperty = propertyDbSet.Create();
        //                newProperty.PropertyTemplate = propertyTemplate;
        //                newProperty.Order = pageNode.Properties.Count + 1;
        //                pageNode.Properties.Add(newProperty);
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}