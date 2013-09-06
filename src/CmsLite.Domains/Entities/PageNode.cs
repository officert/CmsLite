using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    [Table("PageNodes")]
    public class PageNode : Node, IHavePageNodes
    {
        [ForeignKey("PageTemplate")]
        public int PageTemplateId { get; set; }
        public virtual PageTemplate PageTemplate { get; set; }

        [ForeignKey("ParentSectionNode")]
        public int? ParentSectionNodeId { get; set; }
        public virtual SectionNode ParentSectionNode { get; set; }

        [ForeignKey("ParentPageNode")]
        public int? ParentPageNodeId { get; set; }
        public virtual PageNode ParentPageNode { get; set; }

        public virtual ICollection<PageNode> PageNodes { get; set; }

        [InverseProperty("ParentPageNode")]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
