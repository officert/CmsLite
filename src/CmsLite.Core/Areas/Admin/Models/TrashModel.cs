using System.Collections.Generic;
using CmsLite.Core.Areas.Admin.ViewModels;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class TrashModel : AdminLayoutModel
    {
        public IEnumerable<SectionNodeLightViewModel> TrashedSections { get; set; }
    }
}