using CoreUserService.Entities;
using CoreUserService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoreUserService.Repositories
{
    /// <summary>
    /// An IUserRepository implementation
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Database Context
        /// </summary>
        private readonly DomainDbContext _dbContext;

        /// <summary>
        /// UserRepository constructor
        /// </summary>
        public UserRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Repository method to fetch a User entity
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>User entity</returns>
        public User GetUser(long id)
        {
            return _dbContext.Users.Where(x => x.Id == id)
                .Include(x => x.Address)
                .FirstOrDefault();
        }

        /// <summary>
        /// Repository method to fetch a User entity by username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The hashed password</param>
        /// <returns>User entity</returns>
        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return _dbContext.Users
                .Where(x => x.Username == username && x.Password == password)
                .Include(x => x.Address)
                .FirstOrDefault();
        }

        /// <summary>
        /// Repository method to fetch a User entity by email
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns>User entity</returns>
        public User GetUserByEmail(string email)
        {
            return _dbContext.Users.Where(x => x.Email == email).Include(x => x.Address).FirstOrDefault();
        }

        /// <summary>
        /// Repository method to fetch a User entity by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>User entity</returns>
        public User GetUserByUsername(string username)
        {
            return _dbContext.Users.Where(x => x.Username == username).Include(x => x.Address).FirstOrDefault();
        }

        /// <summary>
        /// Repository method to create a User entity
        /// </summary>
        /// <param name="user">The entity</param>
        public void CreateUser(User user)
        {
            //Add the new user
            _dbContext.Users.Add(user);
        }

        /// <summary>
        /// Repository method to persist the context changes to the database
        /// </summary>
        /// <returns>bool, true if successful</returns>
        public bool Save()
        {
            return _dbContext.Save();
        }

        /// <summary>
        /// Repository method to delete a User entity
        /// </summary>
        /// <param name="user">The entity</param>
        public void DeleteUser(User user)
        {
            //Delete the user
            _dbContext.Users.Remove(user);
        }
    }
}
