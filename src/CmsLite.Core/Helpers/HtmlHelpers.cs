using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CmsLite.Core.Helpers
{
    public static class HtmlHelpers
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

    public class ContentHelper
    {
        private const string StyleFormat = "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />";
        private const string ScriptFormat = "<script src=\"{0}\"></script>";
        private const string HttpContextItemsInstanceKey = "ContentHelperInstance";

        public FileUrlRegistrar PluginScripts { get; private set; }

        public ContentHelper()
        {
            PluginScripts = new FileUrlRegistrar(ScriptFormat);
        }

        public static ContentHelper GetInstance(HtmlHelper htmlHelper)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            if (httpContext == null) return null;

            var assetsHelper = (ContentHelper)httpContext.Items[HttpContextItemsInstanceKey];

            if (assetsHelper == null)
                httpContext.Items.Add(HttpContextItemsInstanceKey, assetsHelper = new ContentHelper());

            return assetsHelper;
        }
    }

    public class FileUrlRegistrar
    {
        private readonly string _format;
        private readonly IList<string> _items;

        public FileUrlRegistrar(string format)
        {
            _format = format;
            _items = new List<string>();
        }

        public FileUrlRegistrar Add(string url)
        {
            if (!_items.Contains(url))
                _items.Add(url);

            return null;
        }

        public IHtmlString Render(HtmlHelper htmlHelper)
        {
            var context = htmlHelper.ViewContext.HttpContext;
            var stringBuilder = new StringBuilder();

            foreach (var item in _items)
            {
                var format = string.Format(_format, UrlHelper.GenerateContentUrl(string.Format("~/Areas/Admin/scripts/propertyplugins/{0}", item), context));
                stringBuilder.AppendLine(format);
            }

            return new HtmlString(stringBuilder.ToString());
        }
    }
}