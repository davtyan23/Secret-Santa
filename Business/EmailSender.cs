using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Business
{
    public class EmailSender: IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            // Fetch the port value
            string? portValue = smtpSettings["Port"];
            if (portValue == null)
            {
                throw new InvalidOperationException("SMTP Port is not configured.");
            }

            // Attempt to parse the port value
            if (!int.TryParse(portValue, out int port))
            {
                throw new InvalidOperationException("SMTP Port is not a valid number.");
            }

            // Set up the SMTP client
            var smtpClient = new SmtpClient(smtpSettings["Server"] ?? throw new InvalidOperationException("SMTP Server is not configured."))
            {
                Port = port,
                Credentials = new NetworkCredential(
                    smtpSettings["SenderEmail"] ?? throw new InvalidOperationException("Sender Email is not configured."),
                    smtpSettings["SenderPassword"] ?? throw new InvalidOperationException("Sender Password is not configured.")
                ),
                EnableSsl = bool.TryParse(smtpSettings["EnableSsl"], out bool enableSsl) && enableSsl
            };

            // Ensure that the sender email is not null
            string senderEmail = smtpSettings["SenderEmail"]
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

