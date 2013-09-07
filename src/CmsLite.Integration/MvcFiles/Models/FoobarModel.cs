using CmsLite.Core.Cms.Attributes;
using CmsLite.Utilities.Cms;

namespace CmsLite.Integration.MvcFiles.Models
{
    public class FoobarModel
    {
        [CmsModelProperty(
            DisplayName = "Banner Text Left",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go inside of the banner.",
            TabName = "Banner",
            TabOrder = 1,
            Required = false
            )]
        public string BannerTextLeft { get; set; }

        [CmsModelProperty(
            DisplayName = "Foobar1",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go inside of the banner.",
            TabName = "Foobar",
            TabOrder = 1
            )]
        private string Foobar1 { get; set; }                     //because this is a private property it won't be used as a property template

        public string Foobar2 { get; set; }                     //because this doesn't have the CmsModelProperty attribute it won't be used as a property template
    }
}
