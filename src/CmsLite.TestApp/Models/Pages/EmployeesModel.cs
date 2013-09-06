using CmsLite.Utilities.Cms;
using CmsLite.Web.Cms.Attributes;

namespace CmsLite.TestApp.Models.Pages
{
    [CmsModelTemplate(
        AllowedChildModelTypes = new [] { typeof(EmployeeModel) })]
    public class EmployeesModel
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

        #endregion

        #region Employees Properties

        [CmsModelProperty(
            DisplayName = "Employees Text",
            PropertyType = CmsPropertyType.RichTextEditor,
            Description = "Text about the employees of this company.",
            TabName = "Employees",
            TabOrder = 2
            )]
        public string EmployeesText { get; set; }

        #endregion
    }
}