using CoreUserService.Entities;

namespace CoreUserService.Repositories.Interfaces
{
    /// <summary>
    /// The ITemporaryPasscodeRepository Contract
    /// </summary>
    public interface ITemporarytPasscodeRepository
    {
        /// <summary>
        /// Repository method to fetch a TemporaryPasscode entity
        /// </summary>
        /// <param name="code">The passcode</param>
        /// <returns>TemporaryPasscode entity</returns>
        TemporaryPasscode GetTemporaryPasscode(string code);

        /// <summary>
        /// Repository method to create a TemporaryPasscode entity
        /// </summary>
        /// <param name="tempPasscode">The entity</param>
        void CreateTemporaryPasscode(TemporaryPasscode tempPasscode);

        /// <summary>
        /// Repository method to persist the context changes to the database
        /// </summary>
        /// <returns>bool, true if successful</returns>
        bool Save();
    }
}
