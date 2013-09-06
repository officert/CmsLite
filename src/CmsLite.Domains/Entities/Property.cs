using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ParentPageNode")]
        public int? ParentPageNodeId { get; set; }

        public virtual PageNode ParentPageNode { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }

        [ForeignKey("PropertyTemplate")]
        [Required]
        public int PropertyTemplateId { get; set; }

        public virtual PropertyTemplate PropertyTemplate { get; set; }
    }
}
