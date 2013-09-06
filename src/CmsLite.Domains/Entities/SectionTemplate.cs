using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    public class SectionTemplate
    {
        [Key, Column(Order = 1)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ControllerName { get; set; }   //TODO: need to add a UNIQUE constraint in db scripts on this column

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<PageTemplate> PageTemplates { get; set; }

        public virtual ICollection<SectionNode> SectionNodes { get; set; }

        [StringLength(128)]
        public string IconImageName { get; set; }
    }
}
