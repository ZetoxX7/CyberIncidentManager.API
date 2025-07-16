using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.Auth
{
    public class RefreshToken
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        public int UserId { get; set; }
        // Clé étrangère vers l’utilisateur propriétaire du refresh token

        public User User { get; set; }
        // Navigation vers l’entité User

        [Required]
        [MaxLength(200)]
        public string Token { get; set; }
        // Valeur du refresh token (Base64 ou GUID), ≤ 200 caractères
        // → Stocker de manière sécurisée (hashé en base si possible)

        [Required]
        public DateTime ExpiresAt { get; set; }
        // Date d’expiration du refresh token (à générer via DateTime.UtcNow.AddDays(x))

        [Required]
        public bool IsRevoked { get; set; }
        // Indicateur de révocation manuelle (false = actif, true = révoqué)
    }
}