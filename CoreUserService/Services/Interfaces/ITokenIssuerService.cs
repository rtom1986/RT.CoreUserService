using System.Security.Claims;

namespace CoreUserService.Services.Interfaces
{
    /// <summary>
    /// ITokenIssuerService interface
    /// Contract for session token management functionalities
    /// </summary>
    public interface ITokenIssuerService
    {
        /// <summary>
        /// Method implementation will generate a string session token
        /// </summary>
        string GenerateToken(long userId, string username);

        /// <summary>
        /// Method will validate claims of the current authenticated user
        /// </summary>
        bool ValidateTokenClaim(ClaimsPrincipal principal, long userId);
    }
}
