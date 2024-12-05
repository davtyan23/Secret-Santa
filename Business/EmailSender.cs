using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SecretSantaAPI;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Business
{
    public class EmailSender: IEmailSender
    {
        private readonly SmtpOptions _options;

        public EmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
        {
            //var smtpSettings = _configuration.GetSection("SmtpSettings");

            // Fetch the port value
            //string? portValue = _options.Port;
            if (_options.Port == default)
            {
                throw new InvalidOperationException("SMTP Port is not configured.");
            }

            // Attempt to parse the port value
            //if (!int.TryParse(portValue, out int port))
            //{
            //    throw new InvalidOperationException("SMTP Port is not a valid number.");
            //}

            // Set up the SMTP client
            var smtpClient = new SmtpClient(_options.Server ?? throw new InvalidOperationException("SMTP Server is not configured."))
            {
                Port = _options.Port,//port,
                Credentials = new NetworkCredential(
                    _options.SenderEmail ?? throw new InvalidOperationException("Sender Email is not configured."),
                    _options.SenderPassword ?? throw new InvalidOperationException("Sender Password is not configured.")
                ),
                EnableSsl = _options.EnableSsl// bool.TryParse(_options.EnableSsl, out bool enableSsl) && enableSsl
            };

            // Ensure that the sender email is not null
            string senderEmail = _options.SenderEmail
                ?? throw new InvalidOperationException("Sender Email is not configured.");

            // Create the mail message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = messageBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(recipientEmail);

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

