using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;

namespace CmsLite.Services.Extensions
{
   public static class EfCollectionExtensions
    {
       public static void AddPageTemplate(this SectionTemplate sectionTemplate, IUnitOfWork unitOfWork, PageTemplate pageTemplate)
       {
           //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
           if (sectionTemplate.PageTemplates == null)
           {
               var pageTemplateDbSet = unitOfWork.Context.GetDbSet<PageTemplate>();
               var newPageTemplate = pageTemplateDbSet.Create();
               newPageTemplate.ParentSectionTemplate = sectionTemplate;
               pageTemplateDbSet.Add(newPageTemplate);
               pageTemplateDbSet.Remove(newPageTemplate);
           }

           sectionTemplate.PageTemplates.Add(pageTemplate);
       }

       public static void AddPageNode(this SectionNode sectionNode, IUnitOfWork unitOfWork, PageNode pageNode)
       {
           //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
           if (sectionNode.PageNodes == null)
           {
               var pageNodeDbSet = unitOfWork.Context.GetDbSet<PageNode>();
               var newPageNode = pageNodeDbSet.Create();
               newPageNode.ParentSectionNode = sectionNode;
               pageNodeDbSet.Add(newPageNode);
               pageNodeDbSet.Remove(newPageNode);
           }

           sectionNode.PageNodes.Add(pageNode);
       }

       public static void AddProperty(this PageNode pageNode, IUnitOfWork unitOfWork, PageProperty property)
       {
           //when creating entities EF doesn't instantiate proxy collections, so we need to force it to be instantiated using this hack :(
           if (pageNode.Properties == null)
           {
               var propertyDbSet = unitOfWork.Context.GetDbSet<PageProperty>();
               var newProperty = propertyDbSet.Create();
               newProperty.ParentPageNode = pageNode;
               propertyDbSet.Add(newProperty);
               propertyDbSet.Remove(newProperty);
           }

           pageNode.Properties.Add(property);
       }
    }
}
