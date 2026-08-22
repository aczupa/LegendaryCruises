using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LegendaryCruises.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _smtp;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpOptions> smtpOptions, ILogger<EmailService> logger)
        {
            _smtp = smtpOptions.Value;
            _logger = logger;
        }




        public async Task SendAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_smtp.Username, "Legendary Cruises"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
        public async Task SendAutoReplyAsync(string to)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                _logger.LogWarning("Auto-reply skipped: empty email.");
                return;
            }

            string subject = "Votre message a été reçu";
            string body = @"
        <p>Bonjour,</p>
        <p>Nous avons reçu votre message et nous y répondrons en moins de 48 heures.</p>
        <p>Cordialement,<br/>Legendary Cruises</p>
    ";

            await SendAsync(to, subject, body);
        }




        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlContent,
            string? fromEmail = null,
            string? fromName = null)
        {
            try
            {
                await SendEmailWithAttachmentAsync(toEmail, subject, htmlContent, null, null, fromEmail, fromName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string htmlContent,
            byte[]? attachmentData,
            string? attachmentFileName,
            string? fromEmail = null,
            string? fromName = null)
        {
            var smtpClient = new SmtpClient(_smtp.Host)
            {
                Port = _smtp.Port,
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                EnableSsl = true
            };

            var senderEmail = fromEmail ?? _smtp.From;
            var senderName = fromName ?? _smtp.FromName ?? "World Cruises";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            if (attachmentData != null && attachmentFileName != null)
            {
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachmentData), attachmentFileName));
            }

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email successfully sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
    }

}
