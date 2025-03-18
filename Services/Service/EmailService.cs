using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;

namespace Services.Service
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailRegisterAccountAsync(string emailRequest, string subjectEmail, string fullName)
        {
            try
            {
                var emailBody = _configuration["EmailSetting:EmailRegisterAccount"];
                emailBody = emailBody.Replace("{PROJECT_NAME}", _configuration["EmailSetting:PROJECT_NAME"]);
                emailBody = emailBody.Replace("{FULL_NAME}", fullName);
                emailBody = emailBody.Replace("{PHONE_NUMBER}", _configuration["EmailSetting:PHONE_NUMBER"]);
                emailBody = emailBody.Replace("{EMAIL_ADDRESS}", _configuration["EmailSetting:EMAIL_ADDRESS"]);

                var emailHost = _configuration["EmailSetting:EmailHost"];
                var userName = _configuration["EmailSetting:EmailUsername"];
                var password = _configuration["EmailSetting:EmailPassword"];
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailHost));
                email.To.Add(MailboxAddress.Parse(emailRequest));
                email.Subject = subjectEmail;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailBody
                };
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(userName, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}