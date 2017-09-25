using System.ComponentModel.DataAnnotations;

namespace CoreUserService.Models
{
    /// <summary>
    /// The Address Data Transfer Object
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// The first line of the address
        /// </summary>
        [Required]
        [MaxLength(120)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The second line of the address
        /// </summary>
        [MaxLength(120)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// The city of the address
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        /// <summary>
        /// The state/province of the address
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        /// <summary>
        /// The country of the address
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        /// <summary>
        /// The postal code of the address
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }
    }
}
