using CmsLite.Utilities.Cms;
using CmsLite.Web.Cms.Attributes;

namespace CmsLite.TestApp.Models.Pages
{
    [CmsModelTemplate]
    public class OfficeModel
    {
        [CmsModelProperty(
            DisplayName = "Address",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "The address of the office.",
            TabOrder = 1
            )]
        public string Address { get; set; }

        [CmsModelProperty(
            DisplayName = "State",
            PropertyType = CmsPropertyType.TextString,
            Description = "The state of the office.",
            TabOrder = 1
            )]
        public string State { get; set; }
    }
}