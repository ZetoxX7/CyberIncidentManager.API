using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.Auth
{
    public class MfaRequest
    {
        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }
        // Clé étrangère vers l’utilisateur pour lequel on valide le MFA
        // → Récupérer cet ID uniquement depuis le contexte authentifié (JWT), pas du client

        [Required(ErrorMessage = "Le code MFA est requis.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Le code MFA doit contenir exactement 6 chiffres.")]
        public string Code { get; set; }
        // Code MFA à 6 chiffres envoyé à l’utilisateur (par SMS, email, app)
        // → Vérifier la validité et la date d’expiration côté serveur
    }
}