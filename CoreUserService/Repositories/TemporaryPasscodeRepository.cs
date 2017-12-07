using System;
using System.Linq;
using CoreUserService.Entities;
using CoreUserService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreUserService.Repositories
{
    /// <summary>
    /// An ITemporarytPasscodeRepository implementation
    /// </summary>
    public class TemporaryPasscodeRepository : ITemporarytPasscodeRepository
    {
        /// <summary>
        /// Database Context
        /// </summary>
        private readonly DomainDbContext _dbContext;

        /// <summary>
        /// TemporaryPasscodeRepository constructor
        /// </summary>
        public TemporaryPasscodeRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Repository method to fetch a TemporaryPasscode entity
        /// </summary>
        /// <param name="code">The passcode</param>
        /// <returns>TemporaryPasscode entity</returns>
        public TemporaryPasscode GetTemporaryPasscode(string code)
        {
            return _dbContext.TemporaryPasscodes.Where(x => x.Passcode == code && x.PasscodeExpiration > DateTime.Now)
                .Include(x => x.User)
                .FirstOrDefault();
        }

        /// <summary>
        /// Repository method to create a TemporaryPasscode entity
        /// </summary>
        /// <param name="tempPasscode">The entity</param>
        public void CreateTemporaryPasscode(TemporaryPasscode tempPasscode)
        {
            //Add the new temporary passcode
            _dbContext.TemporaryPasscodes.Add(tempPasscode);
        }

        /// <summary>
        /// Repository method to persist the context changes to the database
        /// </summary>
        /// <returns>bool, true if successful</returns>
        public bool Save()
        {
            return _dbContext.Save();
        }
    }
}
