using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CmsLite.Core.Areas.Admin.Models;

namespace CmsLite.Core.Areas.Admin.ViewModels
{
    public class EditPageNodeViewModel : AdminLayoutModel
    {
        [Required]
        public int Id { get; set; }

        public IEnumerable<PropertyViewModel> Properties { get; set; }
    }
}