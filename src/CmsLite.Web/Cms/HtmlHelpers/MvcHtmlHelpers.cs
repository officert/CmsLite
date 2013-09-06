using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace CmsLite.Web.Cms.HtmlHelpers
{
    public static class MvcHtmlHelpers
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, string className = "description")
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            var stringBuilder = new StringBuilder();
            var tag = new TagBuilder("span");
            tag.AddCssClass(className);
            tag.InnerHtml = description;
            stringBuilder.Append(tag.ToString());

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static MvcHtmlString NotesFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, string className = "notes")
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var notes = (List<string>)metadata.AdditionalValues["notes"];
            if (notes != null)
            {
                var stringBuilder = new StringBuilder();
                foreach (var note in notes)
                {
                    var tag = new TagBuilder("span");
                    tag.AddCssClass(className);
                    tag.InnerHtml = note;
                    stringBuilder.Append(tag.ToString());
                }

                return MvcHtmlString.Create(stringBuilder.ToString());
            }
            return null;
        }

        public static MvcHtmlString FileInputFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            //var fullHtmlFieldName = self
            //    .ViewContext
            //    .ViewData
            //    .TemplateInfo
            //    .GetFullHtmlFieldName(name);

            var stringBuilder = new StringBuilder();
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "file";
            tagBuilder.Attributes["name"] = self.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            stringBuilder.Append(tagBuilder.ToString());

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
}
