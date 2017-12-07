using System;
using System.ComponentModel.DataAnnotations;

namespace CoreUserService.Models
{
    public class TemporaryPasscodeDto
    {
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
    }
}
