using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace EmployeeHub.Common.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpUser))
            {
                throw new ArgumentException("SMTP user (email address) must be provided in the configuration.");
            }

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
            {
                throw new ArgumentException("SMTP host must be provided in the configuration.");
            }

            if (_emailSettings.SmtpPort <= 0)
            {
                throw new ArgumentException("SMTP port must be a positive number.");
            }
        }
        public async Task<bool> SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                smtpClient.EnableSsl = _emailSettings.EnableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SmtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                Console.WriteLine($"Connecting to SMTP server: {_emailSettings.SmtpHost}:{_emailSettings.SmtpPort}");
                Console.WriteLine($"Using SSL: {_emailSettings.EnableSsl}");
                Console.WriteLine($"Sending email from: {_emailSettings.SmtpUser} to: {toEmail}");

                await smtpClient.SendMailAsync(mailMessage);

                return true; // Email sent successfully
            }
            catch (SmtpException smtpEx)
            {
                // Log detailed SMTP error information
                Console.WriteLine($"SMTP error: {smtpEx.Message}");
                if (smtpEx.StatusCode != SmtpStatusCode.GeneralFailure)
                {
                    Console.WriteLine($"SMTP status code: {smtpEx.StatusCode}");
                }
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}