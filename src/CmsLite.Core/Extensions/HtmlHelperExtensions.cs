using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CmsLite.Core.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            var results = new List<string>();

            foreach (var key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    var htmlResult = template(null).ToHtmlString();

                    if (htmlResult != null && !results.Contains(htmlResult))
                    {
                        results.Add(htmlResult);
                        htmlHelper.ViewContext.Writer.Write(htmlResult);
                    }
                }
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString FileInputFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);

            var stringBuilder = new StringBuilder();
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "file";
            tagBuilder.Attributes["name"] = self.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            stringBuilder.Append(tagBuilder);

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
}