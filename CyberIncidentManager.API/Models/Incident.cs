using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Incident
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        // Titre de l’incident, obligatoire, ≤ 100 caractères

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
        // Description détaillée, obligatoire, ≤ 1000 caractères

        [Required]
        public string Severity { get; set; }
        // Sévérité actuelle (ex. “Low”, “Medium”, “High”), obligatoire
        // → À envisager : enum pour contraindre les valeurs

        [Required]
        public string Status { get; set; }
        // Statut de l’incident (ex. “Open”, “InProgress”, “Resolved”), obligatoire
        // → Idem, un enum serait plus sûr ici

        [Required]
        public int TypeId { get; set; }
        // Clé étrangère vers IncidentType

        public IncidentType Type { get; set; }
        // Navigation vers l’entité IncidentType

        [Required]
        public int ReportedBy { get; set; }
        // Clé étrangère vers l’utilisateur ayant rapporté

        public User ReportedByUser { get; set; }
        // Navigation vers l’utilisateur rapporteur

        public int? AssignedTo { get; set; }
        // Clé étrangère optionnelle vers l’utilisateur assigné

        public User AssignedToUser { get; set; }
        // Navigation vers l’utilisateur assigné

        [Required]
        public DateTime CreatedAt { get; set; }
        // Date de création (à initialiser à DateTime.UtcNow)

        public DateTime? ResolvedAt { get; set; }
        // Date de résolution (null tant que non résolu)

        public int? AssetId { get; set; }
        // Clé étrangère optionnelle vers un Asset lié

        public Asset Asset { get; set; }
        // Navigation vers l’actif concerné

        public ICollection<Response> Responses { get; set; }
        // Réponses liées à cet incident
        // → À initialiser par défaut pour éviter les NullReference

        public ICollection<Notification> Notifications { get; set; }
        // Notifications générées pour cet incident
        // → À initialiser par défaut également
    }
}