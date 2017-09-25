using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreUserService.Entities
{
    /// <summary>
    /// Address Entity
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The unique identifier for the address
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

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

        /// <summary>
        /// The User Id FK
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// The Associated User
        /// </summary>
        public User User { get; set; }
    }
}
