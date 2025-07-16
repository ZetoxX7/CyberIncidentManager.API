using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CyberIncidentManager.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smtpSection["Sender"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpSection["Host"], int.Parse(smtpSection["Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpSection["User"], smtpSection["Pass"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}