using System.Web.Optimization;

namespace CmsLite.Core.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/Libs").Include(
                        "~/scripts/libs/jquery-1.10.2.js",
                        "~/scripts/libs/jquery-ui-1.10.3.custom.js",
                        "~/scripts/libs/jquery.validate.js",
                        "~/scripts/source/UnobtrusiveValidationOverrides.js",
                        "~/scripts/libs/jquery.validate.unobtrusive.js",
                        "~/scripts/libs/knockout-2.2.1.debug.js",
                        "~/scripts/libs/bootstrap.js"));


            bundles.Add(new ScriptBundle("~/bundles/Source").Include(
                        "~/scripts/source/core.cms.js",
                        "~/scripts/source/core.utils.js",
                        "~/scripts/source/core.mapping.js",
                        "~/scripts/source/ko.bindinghandlers/ko.bootstrapmodal.js",
                        "~/scripts/source/ko.bindinghandlers/ko.bootstrappopover.js",
                        "~/scripts/source/ko.bindinghandlers/ko.slidevisible.js",
                        "~/scripts/source/plugins/jquery.accordion.js",
                        "~/scripts/source/plugins/jquery.contextmenu.js"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}