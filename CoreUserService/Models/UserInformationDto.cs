using System.ComponentModel.DataAnnotations;

namespace CoreUserService.Models
{
    /// <summary>
    /// The User Information Data Transfer Object
    /// </summary>
    public class UserInformationDto
    {
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
