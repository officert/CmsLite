using System.ComponentModel.DataAnnotations;

namespace CmsLite.Domains.Entities
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        [Required]
        public string MimeType { get; set; }
    }
}
