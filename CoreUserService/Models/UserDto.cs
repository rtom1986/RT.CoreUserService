using System.ComponentModel.DataAnnotations;

namespace CoreUserService.Models
{
    /// <summary>
    /// The User Data Transfer Object
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// The unique identifier for the User
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The unique username for the User
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// The unique email for the User
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The hashed password for the User
        /// </summary>
        [Required]
        [MinLength(7)]
        [MaxLength(50)]
        public string Password { get; set; }

        /// <summary>
        /// The first name of the User
        /// </summary>
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the User
        /// </summary>
        [MaxLength(120)]
        public string LastName { get; set; }

        /// <summary>
        /// The phone number of the User
        /// </summary>
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// The location of the User's image
        /// </summary>
        [Url]
        public string Image { get; set; }

        /// <summary>
        /// The User's bio/description
        /// </summary>
        [MaxLength(1000)]
        public string Bio { get; set; }

        /// <summary>
        /// The address of the User
        /// </summary>
        public AddressDto Address { get; set; }
    }
}
