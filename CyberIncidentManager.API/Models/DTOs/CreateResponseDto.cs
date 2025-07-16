using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateResponseDto
    {
        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        public int IncidentId { get; set; }
        // Clé étrangère vers l’incident : indispensable pour lier la réponse à un incident existant

        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }
        // Identifiant de l’utilisateur auteur de la réponse
        // → Idéalement, récupéré depuis le token JWT plutôt que passé par le client

        [Required(ErrorMessage = "L'action est requise.")]
        [MaxLength(100, ErrorMessage = "L'action ne doit pas dépasser 100 caractères.")]
        public string Action { get; set; }
        // Brève description de l’action effectuée, ≤ 100 caractères

        [MaxLength(500, ErrorMessage = "Les détails ne doivent pas dépasser 500 caractères.")]
        public string Details { get; set; }
        // Informations complémentaires optionnelles, ≤ 500 caractères

        [Required(ErrorMessage = "Le statut de réussite est requis.")]
        public bool IsSuccessful { get; set; }
        // Indique si l’opération a abouti (true) ou non (false)
        // → Permet d’alimenter la logique métier et les statistiques de performance
    }
}