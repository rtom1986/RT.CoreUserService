using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreUserService.Entities
{
    /// <summary>
    /// TemporaryPasscode entity
    /// </summary>
    public class TemporaryPasscode
    {
        /// <summary>
        /// The unique identifier for the TemporaryPasscode
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// The Temporary Passcode
        /// </summary>
        [Required]
        public string Passcode { get; set; }

        /// <summary>
        /// The TemporaryPasscode Expiration
        /// </summary>
        [Required]
        public DateTime PasscodeExpiration { get; set; }

        /// <summary>
        /// The User Id FK
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// The Associated User
        /// </summary>
        public User User { get; set; }
    }
}
