using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    [Table("SectionNodes")]
    public class SectionNode : Node, IHavePageNodes
    {
        [ForeignKey("SectionTemplate")]
        public int SectionTemplateId { get; set; }

        public SectionTemplate SectionTemplate { get; set; }

        [InverseProperty("ParentSectionNode")]
        public virtual ICollection<PageNode> PageNodes { get; set; }
    }
}
