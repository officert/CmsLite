using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CmsLite.Domains.Entities
{
    public abstract class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        [StringLength(255)]
        [Description("The name visible in the cms.")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(255)]
        [Description("The internal name, used to match against url for routing purposes.")]
        public string UrlName { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool InTrash { get; set; }
    }
}