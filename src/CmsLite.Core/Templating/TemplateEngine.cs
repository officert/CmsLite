using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CmsLite.Core.Attributes;
using CmsLite.Core.Extensions;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;
using CmsLite.Interfaces.Templating;
using CmsLite.Resources;

namespace CmsLite.Core.Templating
{
    public class TemplateEngine : ITemplateEngine
    {
        public string Foo;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISectionTemplateService _sectionTemplateService;
        private readonly IPageTemplateService _pageTemplateService;
        private readonly IPropertyTemplateService _propertyTemplateService;

        private IEnumerable<Type> _controllers;
        private Assembly _assembly;

        public TemplateEngine(IUnitOfWork unitOfWork,
            ISectionTemplateService sectionTemplateService,
            IPageTemplateService pageTemplateService,
            IPropertyTemplateService propertyTemplateService)
        {
            _unitOfWork = unitOfWork;
            _sectionTemplateService = sectionTemplateService;
            _pageTemplateService = pageTemplateService;
            _propertyTemplateService = propertyTemplateService;
        }

        public void GenerateTemplates(Assembly assembly)
        { 
            _assembly = assembly;
            _controllers = assembly.GetControllers();

            ProcessControllers();

            _unitOfWork.Commit();

            PostProcessActions();

            _unitOfWork.Commit();
        }

        private void ProcessControllers()
        {
            _controllers = _controllers.ToList();
            var sectionTemplateDbSet = _sectionTemplateService.GetAllSectionTemplates().ToList();

            var controllerNames = _controllers.Select(y => y.Name).ToList();
            var sectionTemplateControllerNames = sectionTemplateDbSet.Select(x => x.ControllerName).ToList();

            var sectionTemplatesToRemove = sectionTemplateDbSet.Where(x => !controllerNames.Contains(x.ControllerName)).ToList();
            var newControllerTypes = _controllers.Where(x => !sectionTemplateControllerNames.Contains(x.Name)).ToList();
            var sectionTemplatesToUpdate = sectionTemplateDbSet.Where(x => controllerNames.Contains(x.ControllerName)).ToList();

            //remove any templates that don't have a controller anymore
            RemoveSectionTemplatesWithNoExistingController(sectionTemplatesToRemove);

            //create templates for any controllers that don't have a template with that controller name
            CreateSectionTemplatesForNewControllers(newControllerTypes);

            //update any templates that still have a controller
            UpdateSectionTemplates(sectionTemplatesToUpdate, _controllers);
        }

        private void PostProcessActions()
        {
            var sectionTemplates = _unitOfWork.Context.GetDbSet<SectionTemplate>();

            if (sectionTemplates != null && sectionTemplates.Any())
            {
                var pageTemplateGroupings = sectionTemplates.SelectMany(x => x.PageTemplates).GroupBy(x => x.ParentSectionTemplate.ControllerName).ToList();

                if (pageTemplateGroupings.Any())
                {
                    foreach (var pageTemplateGroup in pageTemplateGroupings)
                    {
                        var count = pageTemplateGroupings.Count;
                        var controller = _controllers.FirstOrDefault(x => x.Name == pageTemplateGroup.First().ParentSectionTemplate.ControllerName);
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

        private void CreateSectionTemplatesForNewControllers(IEnumerable<Type> controllers)
        {
            if (controllers != null && controllers.Any())
            {
                foreach (var controller in controllers)
                {
                    var attribute = (CmsSectionTemplateAttribute)controller.GetCustomAttributes(typeof(CmsSectionTemplateAttribute), false).FirstOrDefault();

                    var sectionTemplate = _sectionTemplateService.Create(controller.Name, attribute.Name, commit: false);

                    var controllerActions = GetActionsForController(controller);

                    CreatePageTemplatesForActions(sectionTemplate, controllerActions);
                }
            }
        }

        private void UpdateSectionTemplates(IEnumerable<SectionTemplate> sectionTemplates, IEnumerable<Type> controllers)
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
                    CreatePageTemplatesForActions(sectionTemplate, newActionsToAdd);

                    //update any page templates that still have an action
                    UpdatePageTemplates(pageTemplatesToUpdate, controllerActions);
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

        private void CreatePageTemplatesForActions(SectionTemplate sectionTemplate, IEnumerable<MethodInfo> controllerActions)
        {
            foreach (var action in controllerActions)
            {
                var attribute = (CmsPageTemplateAttribute)action.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).FirstOrDefault();
                var model = _assembly.GetModels().FirstOrDefault(x => x.Name == attribute.ModelType.Name);

                var pageTemplate = _pageTemplateService.CreateForSectionTemplate(sectionTemplate, action.Name, model.Name, attribute.Name, attribute.IconImageName, false);

                var modelProperties = GetModelProperties(model);
                CreatePropertyTemplatesForProperties(modelProperties, pageTemplate);
            }
        }

        private void UpdatePageTemplates(IEnumerable<PageTemplate> pageTemplates, IEnumerable<MethodInfo> actions)
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

                    var model = _assembly.GetModels().FirstOrDefault(x => x.Name == pageTemplate.ModelName);
                    var modelProperties = GetModelProperties(model).ToList();

                    _pageTemplateService.Update(pageTemplate, pageTemplateAttribute.ModelType.Name, pageTemplateAttribute.Name, pageTemplateAttribute.IconImageName, false);

                    //if the modal name changes the pageTemplateService will remove all propertytemplates
                    //so we need to add any new propertytemplates from the new model
                    if (pageTemplate.ModelName != pageTemplateAttribute.ModelType.Name)
                    {
                        var newModelProperties = GetModelProperties(pageTemplateAttribute.ModelType).ToList();
                        CreatePropertyTemplatesForProperties(newModelProperties, pageTemplate);
                    }
                    //if this happens there is no need to do the rest of this else {} code - this is only necessary when the model is the same but
                    //some properties in the modal have changes (ie. updated, added, removed)
                    else
                    {
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

                    _propertyTemplateService.Create(pageTemplate,
                        property.Name,
                        attribute.PropertyType,
                        attribute.TabOrder,
                        attribute.TabName,
                        attribute.Required,
                        attribute.Description,
                        attribute.DisplayName,
                        false);
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

        private static IEnumerable<MethodInfo> GetActionsForController(Type controller)
        {
            return controller == null
                        ? null
                        : controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(action => action.ReturnType == typeof(ActionResult) && action.GetCustomAttributes(typeof(CmsPageTemplateAttribute), false).Length > 0);
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