using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;

namespace AccountManaging.Services.Implementation
{
    public class EmailService : IEmailService, IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration)
        {
            _smtpClient = smtpClient;
            _configuration = configuration;
        }

        public void SendMail(string subject, string body, string emailTo)
        {
            try
            {
                _smtpClient.Send(new MailMessage(
                    from: _configuration.GetValue<string>("Email:Smtp:Username"),
                    to: emailTo,
                    subject: subject,
                    body: body
                ));
            }
            catch (SmtpException smtpException)
            {
                Console.WriteLine(smtpException.Message);
            }
        }

        public void Dispose()
        {
            this._smtpClient.Dispose();
        }
    }
}
