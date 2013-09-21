using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CmsLite.Domains.Interfaces;

namespace CmsLite.Domains.Entities
{
    [Table("SectionNodes")]
    public class SectionNode : Node, INode
    {
        [ForeignKey("SectionTemplate")]
        public int SectionTemplateId { get; set; }

        public SectionTemplate SectionTemplate { get; set; }

        [InverseProperty("ParentSectionNode")]
        public virtual ICollection<PageNode> PageNodes { get; set; }
    }
}
