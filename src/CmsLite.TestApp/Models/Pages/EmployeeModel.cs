using CmsLite.Core.Attributes;
using CmsLite.Utilities.Cms;

namespace CmsLite.TestApp.Models.Pages
{
    [CmsModelTemplate]
    public class EmployeeModel
    {
        [CmsModelProperty(
            DisplayName = "First Name Foobar",
            PropertyType = CmsPropertyType.TextString,
            Description = "The first name of the employee.",
            TabName = "Employee Info",
            TabOrder = 1
            )]
        public string FirstName { get; set; }

        [CmsModelProperty(
            DisplayName = "Last Name",
            PropertyType = CmsPropertyType.TextString,
            Description = "The last name of the employee.",
            TabName = "Employee Info",
            TabOrder = 1
            )]
        public string LastName { get; set; }

        [CmsModelProperty(
            DisplayName = "Employee Bio",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "The bio for this employee.",
            TabName = "Employee Info",
            TabOrder = 1
            )]
        public string EmployeeBio { get; set; }

        [CmsModelProperty(
            DisplayName = "Employee Picture",
            PropertyType = CmsPropertyType.ImagePicker,
            Description = "The picture for this employee.",
            TabName = "Employee Info",
            TabOrder = 1
            )]
        public string EmployeePic { get; set; }
    }
}