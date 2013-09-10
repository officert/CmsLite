using System.ComponentModel.DataAnnotations;
using CmsLite.Resources;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class SignInModel : AdminLayoutModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EmailRequired")]
        [Display(Name = "Enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Enter your password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}