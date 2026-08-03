namespace LegendaryCruises.Interfaces
{
    public interface IEmailService
    {
      
            Task SendAsync(string to, string subject, string body);
            Task SendAutoReplyAsync(string to);
        

        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlContent,
            string? fromEmail = null,
            string? fromName = null);

        Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string htmlContent,
            byte[] attachmentData,
            string attachmentFileName,
            string? fromEmail = null,
            string? fromName = null);
    }
}
