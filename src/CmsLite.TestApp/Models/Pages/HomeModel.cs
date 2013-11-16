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
            PropertyType = CmsPropertyType.Number,
            Description = "Some number.",
            TabName = "General",
            TabOrder = 1
            )]
        public string Foobar { get; set; }

        #endregion

        #region Home Properties

        [CmsModelProperty(
            DisplayName = "Banner Text Middle",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Some stuff goes here",
            TabName = "Home",
            TabOrder = 2
            )]
        public string BannerTextMiddle { get; set; }

        [CmsModelProperty(
            DisplayName = "Banner Text Left",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go under the banner asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as",
            TabName = "Home",
            TabOrder = 2
            )]
        public string BannerTextLeft { get; set; }

        #endregion

        #region Additional Info Properties

        [CmsModelProperty(
            DisplayName = "Foobar 1",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Some stuff goes here",
            TabName = "Additional Info",
            TabOrder = 3
            )]
        public string Foobar1 { get; set; }

        [CmsModelProperty(
            DisplayName = "Foobar 2",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text to go under the banner asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as asdfas fsf asfasfdsadfsfsf as",
            TabName = "Additional Info",
            TabOrder = 3
            )]
        public string Foobar2 { get; set; }

        #endregion
    }
}