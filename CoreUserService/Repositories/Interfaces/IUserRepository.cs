using CoreUserService.Entities;

namespace CoreUserService.Repositories.Interfaces
{
    /// <summary>
    /// The IUserRepository Contract
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Repository method to fetch a User entity
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>User entity</returns>
        User GetUser(long id);

        /// <summary>
        /// Repository method to fetch a User entity by username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The hashed password</param>
        /// <returns>User entity</returns>
        User GetUserByUsernameAndPassword(string username, string password);

        /// <summary>
        /// Repository method to fetch a User entity by email
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns>User entity</returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Repository method to fetch a User entity by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>User entity</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Repository method to create a User entity
        /// </summary>
        /// <param name="user">The entity</param>
        void CreateUser(User user);

        /// <summary>
        /// Repository method to persist the context changes to the database
        /// </summary>
        /// <returns>bool, true if successful</returns>
        bool Save();
    }
}
