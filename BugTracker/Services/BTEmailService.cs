using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using BugTracker.Models;
using MailKit.Net.Smtp;

namespace BugTracker.Services
{
    public class BTEmailService : IEmailSender
    {
        #region Properties
        private readonly Models.MailSettings _mailSettings;

        #endregion

        #region Constructor
        public BTEmailService(IOptions<Models.MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        #endregion

        #region Send Email
        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            MimeMessage email = new();

            try
            {
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };

                email.Body = builder.ToMessageBody();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion   
    }
}
