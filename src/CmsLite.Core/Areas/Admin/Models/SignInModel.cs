using System.ComponentModel.DataAnnotations;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class SignInModel : AdminLayoutModel
    {
        [Required]
        [Display(Name = "Enter your email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter your password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}