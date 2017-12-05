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
        /// Method implementation will generate a serialized token
        /// </summary>
        string GenerateToken(long userId, string username);

        /// <summary>
        /// Method implementation will renew a token using existing contextual claims
        /// </summary>
        string RenewToken(ClaimsPrincipal principal);

        /// <summary>
        /// Method implementation will validate token claims of the current authenticated user
        /// </summary>
        bool ValidateToken(ClaimsPrincipal principal, long userId);
    }
}
