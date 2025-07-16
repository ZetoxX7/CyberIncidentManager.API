using MailKit.Net.Smtp;               // Client SMTP de MailKit
using MailKit.Security;               // Options de sécurité pour la connexion SMTP
using MimeKit;                        // Construction des messages MIME

namespace CyberIncidentManager.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        // Injection de la configuration pour récupérer les paramètres SMTP
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Envoie un email en texte brut à l’adresse spécifiée
        public async Task SendAsync(string to, string subject, string body)
        {
            // Récupère la section "Smtp" du fichier de configuration
            var smtpSection = _configuration.GetSection("Smtp");

            // Construction du message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smtpSection["Sender"]));  // Expéditeur configuré
            email.To.Add(MailboxAddress.Parse(to));                       // Destinataire passé en paramètre
            email.Subject = subject;                                      // Sujet du message
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)      // Corps en texte brut
            {
                Text = body
            };

            // Envoi via SMTP sécurisé
            using var smtp = new SmtpClient();
            // 1. Connexion au serveur SMTP avec STARTTLS
            await smtp.ConnectAsync(
                smtpSection["Host"],
                int.Parse(smtpSection["Port"]),
                SecureSocketOptions.StartTls
            );
            // 2. Authentification auprès du serveur SMTP
            await smtp.AuthenticateAsync(
                smtpSection["User"],
                smtpSection["Pass"]
            );
            // 3. Envoi du message
            await smtp.SendAsync(email);
            // 4. Déconnexion propre (QUIT + fermeture de la connexion)
            await smtp.DisconnectAsync(true);
        }
    }
}