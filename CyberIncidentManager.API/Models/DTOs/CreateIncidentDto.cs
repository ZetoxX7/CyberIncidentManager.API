using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateIncidentDto
    {
        [Required(ErrorMessage = "Le titre est requis.")]
        [MaxLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; }
        // Titre de l’incident : obligatoire, ≤ 100 caractères

        [Required(ErrorMessage = "La description est requise.")]
        [MaxLength(1000, ErrorMessage = "La description ne doit pas dépasser 1000 caractères.")]
        public string Description { get; set; }
        // Description détaillée : obligatoire, ≤ 1000 caractères

        [Required(ErrorMessage = "La sévérité est requise.")]
        public string Severity { get; set; }
        // Niveau de sévérité (ex. “Low”, “Medium”, “High”) : obligatoire
        // → Remplacer par un enum pour éviter les valeurs arbitraires

        [Required(ErrorMessage = "Le statut est requis.")]
        public string Status { get; set; }
        // Statut de l’incident (ex. “Open”, “Resolved”) : obligatoire
        // → Idem, préférez un enum pour plus de sécurité

        [Required(ErrorMessage = "Le type d'incident est requis.")]
        public int TypeId { get; set; }
        // Clé étrangère vers IncidentType : identifie le type d’incident

        public int? AssignedTo { get; set; }
        // Optionnel : identifiant de l’utilisateur assigné
        // → Récupérer l’ID depuis le contexte authentifié si besoin

        public int? AssetId { get; set; }
        // Optionnel : identifiant de l’actif lié à l’incident

        [Required(ErrorMessage = "L'utilisateur rapporteur est requis.")]
        public int ReportedBy { get; set; }
        // Clé étrangère vers l’utilisateur créateur de l’incident
        // → À récupérer depuis le token JWT plutôt que du client
    }
}