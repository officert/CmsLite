using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CmsLite.Domains.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
