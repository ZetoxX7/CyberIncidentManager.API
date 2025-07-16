using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.Auth
{
    public class MfaRequest
    {
        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Le code MFA est requis.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Le code MFA doit contenir exactement 6 chiffres.")]
        public string Code { get; set; }
    }
}
