using System;
using System.ComponentModel.DataAnnotations;

namespace CmsLite.Domains.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zipcode { get; set; }

        public byte[] ProfilePicData { get; set; }

        public string ProfilePicMimeType { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsActivated { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime? LastLockedOutDate { get; set; }

        public string LastLockedOutReason { get; set; }

        public string NewPasswordKey { get; set; }

        public DateTime? NewPasswordRequested { get; set; }

        public string NewEmail { get; set; }

        public string NewEmailKey { get; set; }

        public DateTime? NewEmailRequested { get; set; }

        public enum UserStatus
        {
            CannotBeFound,
            PasswordDoesNotMatch,
            NotActivated,
            LockedOut,
            Validated
        }
    }
}
