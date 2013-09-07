CmsLite
=======================================
A CMS built on MVC. Easy to use, strongly typed templates using controllers, actions, and models. 
Write your templates in Visual Studio and CmsLite will turn them into templates to use in your CMS.

## Section Templates from Controllers
A section template allows you to create Section Nodes with your CMS. A Section Node is a folder used to 
group together Page Nodes for your website. Any MVC ActionResults in your Controller that are marked up with 
the CmsPageTemplate attribute will generate [Page Templates](.#2---Page Templates from Actions).

```csharp
[CmsSectionTemplate(Name = "Home Template")]
public class HomeController : CmsController
{
    public HomeController(IActionInvoker actionInvoker, ICmsModelHelper cmsModelHelper) : base(actionInvoker, cmsModelHelper)
    {
    }
}
```

## Page Templates from Actions

```csharp
[CmsPageTemplate(
    Name = "Home Template",
    ModelType = typeof(HomeModel)
)]
public override ActionResult Index()
{
    var model = CmsModelHelper.GetModel<HomeModel>(HttpContext);
    return View(model);
}
```

## Property Templates from Model properties

```csharp
[CmsModelProperty(
    DisplayName = "Title",
    PropertyType = CmsPropertyType.TextString,
    Description = "The title of this page.",
    TabName = "General",
    TabOrder = 1
    )]
public string Title { get; set; }
```
