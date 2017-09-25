using CoreUserService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CoreUserService.Services
{
    /// <summary>
    /// ITokenIssuerService implementation used to manage JWT issuance and validation
    /// </summary>
    public class JwtTokenIssuerService : ITokenIssuerService
    {
        //The IConfiguration object, used for reading application settings
        private readonly IConfiguration _configuration;

        /// <summary>
        /// ILogger property
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration">IConfiguration implementation to be injected</param>
        /// <param name="logger">The ILogger implementation</param>
        public JwtTokenIssuerService(IConfiguration configuration, ILogger<JwtTokenIssuerService> logger)
        {
            //Set injected IConfiguration value
            _configuration = configuration;

            //Set logger to injected instance
            Logger = logger;
        }

        /// <summary>
        /// Generates a JSON Web Token
        /// </summary>
        /// <param name="userId">The userId of the User</param>
        /// <param name="username">The username of the User</param>
        /// <returns>Serialized JWT</returns>
        public string GenerateToken(long userId, string username)
        {
            Logger.LogInformation("Begin JWT generation for user id [{0}]", userId);

            //Ensure userId and username are valid
            if (userId <= 0 || string.IsNullOrWhiteSpace(username))
            {
                Logger.LogError("JWT generation fail, invalid user id or username");
                return string.Empty;
            }

            //Define JWT claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString()),
            };

            //Get configured PK as SymmetricSecurityKey object
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            //Get signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Get configured JWT issuer
            var issuer = _configuration["Tokens:Issuer"];

            //Get configured JWT audience
            var audience = _configuration["Tokens:Audience"];

            //Get configured JWT expiration
            var expiration = DateTime.Now.AddMinutes(Convert.ToInt64(_configuration["Tokens:ExpirationMinutes"]));

            //Create a JSON Web Token
            var token = new JwtSecurityToken(issuer, audience, claims, expires: expiration, signingCredentials: creds);

            //Return the serialized JSON Web Token
            Logger.LogInformation("JWT successfully generated for user id [{0}]", userId);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validate JWT claims against desired user id
        /// This checks to see if the JWT has the proper authorization rights
        /// </summary>
        /// <param name="principal">ClaimsPrincipal object</param>
        /// <param name="userId">User id to validate against</param>
        /// <returns>Boolean, true if claim is validated</returns>
        public bool ValidateTokenClaim(ClaimsPrincipal principal, long userId)
        {
            //Init return value
            var validTokenClaim = false;

            //Ensure Claims object is not null
            if (principal != null && principal.Claims != null)
            {
                //Get the jti claim if it exists
                var claim = principal.Claims.FirstOrDefault(x => x.Type == "jti");
                if (claim != null && userId.ToString() == claim.Value)
                {
                    //Claim validated, JWT jti value equals target user id
                    validTokenClaim = true;
                }
                else
                {
                    Logger.LogError("User id [{0}] attempted to access an unauthorized resource for user id [{1}]", claim.Value, userId);
                }
            }

            //Return validation result
            return validTokenClaim;
        }
    }
}
