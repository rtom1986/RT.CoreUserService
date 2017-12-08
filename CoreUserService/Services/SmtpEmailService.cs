using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using CoreUserService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreUserService.Services
{
    /// <summary>
    /// IEmailService implementation used to send emails
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        //The IConfiguration object, used for reading application settings
        private readonly IConfiguration _configuration;

        //The SmtpClient object used to send emails
        private SmtpClient _smtpClient;

        /// <summary>
        /// ILogger property
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration">IConfiguration implementation to be injected</param>
        /// <param name="logger">The ILogger implementation</param>
        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            //Set injected IConfiguration value
            _configuration = configuration;

            //Set logger to injected instance
            Logger = logger;
        }

        /// <summary>
        /// Sends an email to the desired recipient
        /// </summary>
        /// <param name="toAddress">The address the email will be sent to</param>
        /// <param name="messageBody">The body of the message</param>
        public void Send(string toAddress, string messageBody)
        {
            Logger.LogInformation("Building SMTP email for {0}", toAddress);

            //Instantiate the smtp client 
            _smtpClient = new SmtpClient(_configuration["SmtpSettings:SmtpServer"]);
            _smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SmtpSettings:SmtpEnableSsl"]);
            _smtpClient.Port = Convert.ToInt16(_configuration["SmtpSettings:SmtpPort"]);
            _smtpClient.Credentials = new NetworkCredential(_configuration["SmtpSettings:SmtpFromAddress"], 
                _configuration["SmtpSettings:SmtpPassword"]);

            //Listen for SendCompleted events
            _smtpClient.SendCompleted += EmailSendCompleted;

            //Build the email message
            var message = new MailMessage(_configuration["SmtpSettings:SmtpFromAddress"], toAddress, 
                _configuration["SmtpSettings:SmtpSubject"], messageBody);

            //Send the email message async
            _smtpClient.SendAsync(message, toAddress);

            Logger.LogInformation("SMTP email to {0} is sending", toAddress);
        }

        /// <summary>
        /// Event handler invoked when email is sent
        /// </summary>
        /// <param name="sender">The object invoking the event</param>
        /// <param name="e">AsyncCompletedEventArgs type</param>
        public void EmailSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Stop listening for SendCompleted events
            _smtpClient.SendCompleted -= EmailSendCompleted;

            // Get the identifier for this operation
            var sendAddress = (string)e.UserState;

            //Process result
            if (e.Error != null)
            {
                Logger.LogError("SMTP email was not sent to {0} successfully | error: {1}", sendAddress, e.Error.ToString());
            }
            else
            {
                Logger.LogInformation("SMTP email was sent to {0} successfully", sendAddress);
            }
        }
    }
}
