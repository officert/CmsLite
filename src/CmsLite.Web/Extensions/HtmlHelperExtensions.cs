using System.Text;
using System.Web.Mvc;

namespace CmsLite.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        private const string BootstrapModalCssClasses = "modal hide";

        public static string BootstrapModalForm(this HtmlHelper htmlHelper, string action, string id)
        {
            var stringBuilder = new StringBuilder();

            var tagBuilder = new TagBuilder("form");
            tagBuilder.Attributes.Add("action", action);
            tagBuilder.Attributes.Add("id", id);
            tagBuilder.Attributes.Add("tabindex", "-1");            //TODO: not sure what this does, in bootstrap's example
            tagBuilder.Attributes.Add("role", "dialog");            //TODO: not sure what this does, in bootstrap's example
            tagBuilder.AddCssClass(BootstrapModalCssClasses);


            return stringBuilder.ToString();
        }
    }
}