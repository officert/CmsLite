using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using CmsLite.Core.Helpers;

namespace CmsLite.Core.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static ContentHelper Content(this HtmlHelper htmlHelper)
        {
            return ContentHelper.GetInstance(htmlHelper);
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