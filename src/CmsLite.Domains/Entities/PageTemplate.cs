using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    public class PageTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Description("The name of the action method.")]
        [StringLength(255)]
        public string ActionName { get; set; }   //TODO: need to add a UNIQUE constraint in db scripts on this column

        [Description("The name visible in cms menus for users to select templates for a page.")]
        [StringLength(255)]
        public string Name { get; set; }

        [ForeignKey("ParentSectionTemplate")]
        public int? ParentSectionTemplateId { get; set; }

        public virtual SectionTemplate ParentSectionTemplate { get; set; }

        [ForeignKey("ParentPageTemplate")]
        public int? ParentPageTemplateId { get; set; }

        public virtual PageTemplate ParentPageTemplate { get; set; }

        [InverseProperty("PageTemplate")]
        public virtual ICollection<PageNode> PageNodes { get; set; }

        public virtual ICollection<PageTemplate> PageTemplates { get; set; }

        [Required]
        [StringLength(255)]
        [Description("The name of the Model class.")]
        public string ModelName { get; set; }

        [InverseProperty("ParentModelTemplate")]
        public virtual ICollection<PropertyTemplate> PropertyTemplates { get; set; }

        [StringLength(128)]
        public string IconImageName { get; set; }
    }
}
