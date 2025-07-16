using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        public int UserId { get; set; }
        // Clé étrangère vers l’utilisateur destinataire

        public User User { get; set; }
        // Navigation vers l’entité User

        [Required]
        public int IncidentId { get; set; }
        // Clé étrangère vers l’incident concerné

        public Incident Incident { get; set; }
        // Navigation vers l’entité Incident

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        // Titre de la notification, ≤ 100 caractères

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
        // Contenu de la notification, ≤ 1000 caractères

        public bool IsRead { get; set; }
        // Indicateur de lecture (false = non lu, true = lu)

        public DateTime CreatedAt { get; set; }
        // Date/heure de création (à initialiser à DateTime.UtcNow)
    }
}