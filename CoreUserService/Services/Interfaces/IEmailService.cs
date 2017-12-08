namespace CoreUserService.Services.Interfaces
{
    /// <summary>
    /// IEmailService interface
    /// Contract for email send/recieve funtionality
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Method implementation will send an email to the desired recipient
        /// </summary>
        void Send(string toAddress, string messageBody);
    }
}
