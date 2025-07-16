using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Response
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        public int IncidentId { get; set; }
        // Clé étrangère vers l’incident concerné

        public Incident Incident { get; set; }
        // Navigation vers l’entité Incident

        [Required]
        public int UserId { get; set; }
        // Clé étrangère vers l’utilisateur ayant fait la réponse

        public User User { get; set; }
        // Navigation vers l’entité User

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }
        // Brève description de l’action (ex. “Lock account”), ≤ 100 caractères

        [MaxLength(500)]
        public string Details { get; set; }
        // Détails optionnels de l’intervention, ≤ 500 caractères

        [Required]
        public DateTime Timestamp { get; set; }
        // Date et heure de la réponse (à initialiser à DateTime.UtcNow)

        [Required]
        public bool IsSuccessful { get; set; }
        // Indique si l’action a réussi (true) ou non (false)
    }
}