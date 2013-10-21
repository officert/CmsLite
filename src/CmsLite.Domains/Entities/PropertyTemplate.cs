using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsLite.Domains.Entities
{
    public class PropertyTemplate
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Description("The name visible in the cms.")]
        public string DisplayName { get; set; }

        [StringLength(255)]
        [Required]
        [Description("The name of the cms property type. ex. Rich Text Editor")]
        public string CmsPropertyType { get; set; }

        [Description("The name of property in the model.")]
        [Required]
        public string PropertyName { get; set; }

        [Description("The description of the property visible in the cms.")]
        public string Description { get; set; }

        [StringLength(255)]
        public string TabName { get; set; }

        public bool Required { get; set; }

        [ForeignKey("ParentPageTemplate")]
        public int ParentPageTemplateId { get; set; }

        public virtual PageTemplate ParentPageTemplate { get; set; }

        [InverseProperty("PropertyTemplate")]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
