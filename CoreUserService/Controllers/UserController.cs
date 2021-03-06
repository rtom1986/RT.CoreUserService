﻿using System;
using AutoMapper;
using CoreUserService.Entities;
using CoreUserService.Models;
using CoreUserService.Repositories.Interfaces;
using CoreUserService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreUserService.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [Route("api/users")]
    [Produces("application/json", "application/xml")]
    public class UserController : Controller
    {
        //UserRepository field
        private readonly IUserRepository _userRepo;

        //UserRepository field
        private readonly ITemporarytPasscodeRepository _tempPasscodeRepo;

        //TokenIssuerService field
        private readonly ITokenIssuerService _tokenIssuerService;

        //Base36GeneratorService field
        private readonly IBase36GeneratorService _base36GeneratorService;

        //IEmailService field
        private readonly IEmailService _emailService;

        /// <summary>
        /// ILogger property
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="userRepo">The IUserRepository implementation</param>
        /// <param name="tempPasscodeRepo">The ITemporarytPasscodeRepository implementation</param>
        /// <param name="tokenIssuerService">The ITokenIssuerService implementation</param>
        /// <param name="base36GeneratorService">The IBase36GeneratorService implementation</param>
        /// <param name="emailService">The IEmailService implementation</param>
        /// <param name="logger">The ILogger implementation</param>
        public UserController(IUserRepository userRepo, 
            ITemporarytPasscodeRepository tempPasscodeRepo, 
            ITokenIssuerService tokenIssuerService,
            IBase36GeneratorService base36GeneratorService,
            IEmailService emailService,
            ILogger<UserController> logger)
        {
            //Set UserRepository to injected instance
            _userRepo = userRepo;

            //Set UserGuidRepository to injected instance
            _tempPasscodeRepo = tempPasscodeRepo;

            //Set TokenIssuerService to injected instance
            _tokenIssuerService = tokenIssuerService;

            //Set Base36GeneratorService to injected instance
            _base36GeneratorService = base36GeneratorService;

            //Set EmailService to injected instance
            _emailService = emailService;

            //Set logger to injected instance
            Logger = logger;
        }

        /// <summary>
        /// Authenticates a User based on username and password
        /// If successful, a JWT is issued and added to the response header.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>The User, if authenticated</returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            Logger.LogInformation("Begin login attempt for username [{0}]", username);

            //Fetch user entity
            var userEntity = _userRepo.GetUserByUsernameAndPassword(username, password);

            //Check if user entity was found
            if (userEntity == null)
            {
                Logger.LogInformation("Login attempt for username [{0}] failed, invalid credentials", username);

                //Handle login failure
                return Unauthorized();
            }

            //Success! Issue a new JWT and return user dto object
            Logger.LogInformation("Login attempt for user id [{0}] successful, user authenticated", userEntity.Id);
            AddJwtToResponseHeader(_tokenIssuerService.GenerateToken(userEntity.Id, userEntity.Username));
            return Ok(Mapper.Map<UserDto>(userEntity));
        }

        /// <summary>
        /// Gets a User by Id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>The User, if found</returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(long id)
        {
            Logger.LogInformation("Begin fetch attempt for user id [{0}]", id);

            //Fetch user entity
            var userEntity = _userRepo.GetUser(id);

            //Check if user entity was found
            if (userEntity == null)
            {
                //Handle 404 Not Found
                Logger.LogInformation("Fetch attempt for user id [{0}] failed, user not found", id);
                return NotFound();
            }

            //Success! Issue a new JWT and return user dto object
            Logger.LogInformation("Fetch attempt for user id [{0}] successful, user found", id);
            AddJwtToResponseHeader(_tokenIssuerService.RenewToken(User));
            return Ok(Mapper.Map<UserDto>(userEntity));
        }

        /// <summary>
        /// Creates a User
        /// </summary>
        /// <param name="userDto">User</param>
        /// <returns>The User, if created</returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Server Error</response>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDto userDto)
        {
            Logger.LogInformation("Begin create attempt for new user");

            //Ensure request body could be deserialized into the desired type
            if (userDto == null)
            {
                //Handle 400 Bad Request
                Logger.LogInformation("Create attempt for new user failed, bad request");
                return BadRequest();
            }

            //Ensure username is unique
            if (_userRepo.GetUserByUsername(userDto.Username) != null)
            {
                ModelState.AddModelError("Username", $"The provided Username '{userDto.Username}' is not unique");
            }

            //Ensure email is unique
            if (_userRepo.GetUserByEmail(userDto.Email) != null)
            {
                ModelState.AddModelError("Email", $"The provided Email '{userDto.Email}' is not unique");
            }

            //Ensure model validation succeeds
            if (!ModelState.IsValid)
            {
                //Handle 400 Bad Request
                Logger.LogInformation("Create attempt for new user failed, data validation fail");
                return BadRequest(ModelState);
            }

            //Save the new user entity
            var userEntity = Mapper.Map<User>(userDto);
            _userRepo.CreateUser(userEntity);

            //Ensure entity is persisted successfully
            if (!_userRepo.Save())
            {
                Logger.LogError("Create attempt for new user failed, server error");
                return StatusCode(500, "An error occurred while creating the User");
            }

            //Issue a new JWT and return user dto object
            userDto.Id = userEntity.Id;
            userDto.Password = null;
            Logger.LogInformation("Create attempt for new user id [{0}] successful, user created", userEntity.Id);
            AddJwtToResponseHeader(_tokenIssuerService.GenerateToken(userEntity.Id, userEntity.Username));
            return CreatedAtRoute("GetUser", new { id = userEntity.Id }, userDto);
        }

        /// <summary>
        /// Updates a User's basic profile information
        /// Username, Email, and/or password cannot be updated via this method
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="userInformationDto">User</param>
        /// <returns>Empty Response</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Server Error</response>
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(long id, [FromBody] UserInformationDto userInformationDto)
        {
            Logger.LogInformation("Begin update attempt for user id [{0}]", id);

            //Validate token's claim to the specified user id
            if (_tokenIssuerService.ValidateToken(User, id))
            {
                //Ensure request body could be deserialized into the desired type
                if (userInformationDto == null)
                {
                    //Handle 400 Bad Request
                    Logger.LogInformation("Update attempt for user id [{0}] failed, bad request", id);
                    return BadRequest();
                }

                //Ensure user entity exists
                var userEntity = _userRepo.GetUser(id);
                if (userEntity == null)
                {
                    //Handle 404 Not Found
                    Logger.LogInformation("Update attempt for user id [{0}] failed, user not found", id);
                    return NotFound();
                }

                //Map dto values to entity
                Mapper.Map(userInformationDto, userEntity);

                //Ensure entity is persisted successfully
                if (!_userRepo.Save())
                {
                    Logger.LogError("Update attempt for user id [{0}] failed, server error", id);
                    return StatusCode(500, "An error occurred while updating the User");
                }

                //Success! Issue a new JWT and return no content
                Logger.LogInformation("Update attempt for user id [{0}] successful, user updated", id);
                AddJwtToResponseHeader(_tokenIssuerService.RenewToken(User));
                return NoContent();
            }

            //Handle authorization failure
            Logger.LogInformation("Update attempt for user id [{0}] failed, not authorized", id);
            return Unauthorized();
        }

        /// <summary>
        /// Updates a User's credentials
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="userCredentialDto">User</param>
        /// <returns>Empty Response</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Server Error</response>
        [Authorize]
        [HttpPut("credentials/{id}")]
        public IActionResult UpdateUserCredentials(long id, [FromBody] UserCredentialDto userCredentialDto)
        {
            Logger.LogInformation("Begin update attempt for user id [{0}]", id);

            //Validate token's claim to the specified user id
            if (_tokenIssuerService.ValidateToken(User, id))
            {
                //Ensure request body could be deserialized into the desired type
                if (userCredentialDto == null)
                {
                    //Handle 400 Bad Request
                    Logger.LogInformation("Update attempt for user id [{0}] failed, bad request", id);
                    return BadRequest();
                }

                //Ensure user entity exists
                var userEntity = _userRepo.GetUser(id);
                if (userEntity == null)
                {
                    //Handle 404 Not Found
                    Logger.LogInformation("Update attempt for user id [{0}] failed, user not found", id);
                    return NotFound();
                }

                //Ensure existing credentials are valid
                if (_userRepo.GetUserByUsernameAndPassword(userCredentialDto.CurrentUsername, userCredentialDto.CurrentPassword) == null)
                {
                    //Handle authorization failure
                    Logger.LogInformation("Update attempt for user id [{0}] failed, existing credentials invalid", id);
                    return Unauthorized();
                }

                //Ensure username is unique (if changed)
                if (userCredentialDto.NewUsername != userEntity.Username 
                    && _userRepo.GetUserByUsername(userCredentialDto.NewUsername) != null)
                {
                    ModelState.AddModelError("Username", $"The updated Username '{userCredentialDto.NewUsername}' is not unique");
                }

                //Ensure email is unique (if changed)
                if (userCredentialDto.NewEmail != userEntity.Email && _userRepo.GetUserByEmail(userCredentialDto.NewEmail) != null)
                {
                    ModelState.AddModelError("Email", $"The updated Email '{userCredentialDto.NewEmail}' is not unique");
                }

                //Ensure model validation succeeds
                if (!ModelState.IsValid)
                {
                    //Handle 400 Bad Request
                    Logger.LogInformation("Update attempt for user id [{0}] failed, data validation fail", id);
                    return BadRequest(ModelState);
                }

                //Handle password updates
                if (!string.IsNullOrWhiteSpace(userCredentialDto.NewPassword))
                {
                    //Set password to updated value
                    userEntity.Password = userCredentialDto.NewPassword;
                }

                //Handle email updates
                if (!string.IsNullOrWhiteSpace(userCredentialDto.NewEmail))
                {
                    //Set email to updated value
                    userEntity.Email = userCredentialDto.NewEmail;
                }

                //Handle username updates
                if (!string.IsNullOrWhiteSpace(userCredentialDto.NewUsername))
                {
                    //Set username to updated value
                    userEntity.Username = userCredentialDto.NewUsername;
                }

                //Ensure entity is persisted successfully
                if (!_userRepo.Save())
                {
                    Logger.LogError("Update attempt for user id [{0}] failed, server error", id);
                    return StatusCode(500, "An error occurred while updating the User");
                }

                //Success! Issue a new JWT and return no content
                Logger.LogInformation("Update attempt for user id [{0}] successful, user updated", id);
                AddJwtToResponseHeader(_tokenIssuerService.GenerateToken(userEntity.Id, userEntity.Username));
                return NoContent();
            }

            //Handle authorization failure
            Logger.LogInformation("Update attempt for user id [{0}] failed, not authorized", id);
            return Unauthorized();
        }

        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Empty response</returns>
        /// <response code="204">No Content</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Server Error</response>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(long id)
        {
            Logger.LogInformation("Begin delete attempt for user id [{0}]", id);

            //Validate token's claim to the specified user id
            if (_tokenIssuerService.ValidateToken(User, id))
            {
                //Fetch user entity
                var userEntity = _userRepo.GetUser(id);

                //Check if user entity was found
                if (userEntity == null)
                {
                    //Handle 404 Not Found
                    Logger.LogInformation("Delete attempt for user id [{0}] failed, user not found", id);
                    return NotFound();
                }

                //Hard delete user entity
                _userRepo.DeleteUser(userEntity);

                //Ensure entity is persisted successfully
                if (!_userRepo.Save())
                {
                    //Handle user save fail
                    Logger.LogError("Delete attempt for user id [{0}] failed, server error", id);
                    return StatusCode(500, "An error occurred while deleting the User");
                }

                //Delete success!
                Logger.LogInformation("Delete attempt for user id [{0}] successful, user deleted", id);
                return NoContent();
            }

            //Handle authorization failure
            Logger.LogInformation("Delete attempt for user id [{0}] failed, not authorized", id);
            return Unauthorized();
        }

        /// <summary>
        /// Creates a temporary passcode and sends it to the User's email
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <returns>No content</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Server Error</response>
        [AllowAnonymous]
        [HttpPut("tempPasscode")]
        public IActionResult RequestTemporaryPasscode([FromQuery] string username = null, [FromQuery] string email = null)
        {
            //Ensure at least one of the required parameters is present
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                //Handle 400 Bad Request
                Logger.LogInformation("Issue temporary passcode failed, bad request");
                return BadRequest();
            }

            //User requesting temporary passcode
            User user = null;

            //Attempt to fetch a user with the provided information
            if (!string.IsNullOrWhiteSpace(username))
            {
                user = _userRepo.GetUserByUsername(username);
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                user = _userRepo.GetUserByEmail(email);
            }

            //Ensure a User entity was fetched
            if (user == null)
            {
                //User not found (Normally a 404, but we do not want to disclose in this case)
                Logger.LogInformation("Unable to find a User with the information provided, returning no content");
                return NoContent();
            }

            //Create temporary passcode
            var tempPasscode = new TemporaryPasscode()
            {
                UserId = user.Id,
                Passcode = _base36GeneratorService.Generate(),
                PasscodeExpiration = DateTime.Now.AddMinutes(10)      
            };
            _tempPasscodeRepo.CreateTemporaryPasscode(tempPasscode);

            //Ensure entity is persisted successfully
            if (!_tempPasscodeRepo.Save())
            {
                //Handle temporary passcode save fail
                Logger.LogError("Temporary passcode creation failed, server error", tempPasscode.Passcode);
                return StatusCode(500, "An error occurred while creating the temporary passcode");
            }

            //Send email to User
            Logger.LogInformation("Created temporary passcode [{0}] for username [{1}]", tempPasscode.Passcode, user.Username);
            _emailService.Send(user.Email, $"Your Username is {user.Username}.\n\nYour temporary passcode is {tempPasscode.Passcode}");

            return NoContent();
        }

        /// <summary>
        /// Authenticates a User based on a temporary passcode
        /// If successful, a JWT is issued and added to the response header.
        /// </summary>
        /// <param name="passcode">Temporary Passcode</param>
        /// <returns>The User, if authenticated</returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [AllowAnonymous]
        [HttpGet("tempPasscode")]
        public IActionResult RedeemTemporaryPasscode([FromQuery] string passcode)
        {
            Logger.LogInformation("Begin temp passcode redeem attempt for passcode [{0}]", passcode);

            //Fetch temporary passcode entity
            var passcodeEntity = _tempPasscodeRepo.GetTemporaryPasscode(passcode);

            //Check if temporary passcode entity was found
            if (passcodeEntity == null)
            {
                Logger.LogInformation("Temp passcode redeem attempt for passcode [{0}] failed, invalid code", passcode);

                //Handle redeem failure
                return Unauthorized();
            }

            //Success! Issue a new JWT and return user dto object
            Logger.LogInformation("Temp passcode redeem attempt for user id [{0}] successful, user authenticated", passcodeEntity.UserId);
            AddJwtToResponseHeader(_tokenIssuerService.GenerateToken(passcodeEntity.UserId, passcodeEntity.User.Username));
            return Ok(Mapper.Map<UserDto>(passcodeEntity.User));
        }

        /// <summary>
        /// Adds a JWT to the current response header
        /// </summary>
        /// <param name="jwt"></param>
        private void AddJwtToResponseHeader(string jwt)
        {
            //Add header to response containing the JWT value
            Response.Headers.Add("JWT", jwt);

            //Add a Access-Control-Expose-Headers header to the response to allow access in CORS AJAX scenarios
            Response.Headers.Add("Access-Control-Expose-Headers", "JWT");
        }
    }
}
