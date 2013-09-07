using CmsLite.Core.Attributes;
using CmsLite.Utilities.Cms;

namespace CmsLite.TestApp.Models.Pages
{
    [CmsModelTemplate]
    public class HomeModel
    {
        #region General Properties

        [CmsModelProperty(
            DisplayName = "Title",
            PropertyType = CmsPropertyType.TextString,
            Description = "The title of this page.",
            TabName = "General",
            TabOrder = 1
            )]
        public string Title { get; set; }

        [CmsModelProperty(
            DisplayName = "Title",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "The title of this page.",
            TabName = "General",
            TabOrder = 1
            )]
        public string Poopshannks { get; set; }

        #endregion

        #region Home Properties

        [CmsModelProperty(
            DisplayName = "Banner Text Middle",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go under the banner",
            TabName = "Home",
            TabOrder = 2
            )]
        public string BannerTextMiddle { get; set; }

        [CmsModelProperty(
            DisplayName = "Banner Text Left",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go under the banner",
            TabName = "Home",
            TabOrder = 2
            )]
        public string BannerTextLeft { get; set; }

        #endregion
    }
}