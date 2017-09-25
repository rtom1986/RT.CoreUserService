using System.ComponentModel.DataAnnotations;

namespace CoreUserService.Models
{
    /// <summary>
    /// The User Credential Data Transfer Object
    /// </summary>
    public class UserCredentialDto
    {
        /// <summary>
        /// The current username for the User
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string CurrentUsername { get; set; }

        /// <summary>
        /// The current hashed password for the User
        /// </summary>
        [Required]
        [MinLength(7)]
        [MaxLength(50)]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The new unique username for the User
        /// </summary>
        [MinLength(3)]
        [MaxLength(50)]
        public string NewUsername { get; set; }

        /// <summary>
        /// The new hashed password for the User
        /// </summary>
        [MinLength(7)]
        [MaxLength(50)]
        public string NewPassword { get; set; }

        /// <summary>
        /// The new unique email for the User
        /// </summary>
        [EmailAddress]
        public string NewEmail { get; set; }

 
    }
}
