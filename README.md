CmsLite
=======================================
A CMS built on MVC. Easy to use, strongly typed templates using controllers, actions, and models. Write your templates in Visual Studio and CmsLite will turn them into templates to use in your CMS.

## Section Templates from Controllers

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
