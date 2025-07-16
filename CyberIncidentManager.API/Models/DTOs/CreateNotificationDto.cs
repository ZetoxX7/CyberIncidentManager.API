using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }
        // Clé étrangère vers l’utilisateur destinataire
        // → Ne pas faire confiance au client : récupère plutôt l’ID depuis le JWT

        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        public int IncidentId { get; set; }
        // Clé étrangère vers l’incident concerné

        [Required(ErrorMessage = "Le titre est requis.")]
        [MaxLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; }
        // Titre de la notification, ≤ 100 caractères

        [Required(ErrorMessage = "Le message est requis.")]
        [MaxLength(500, ErrorMessage = "Le message ne doit pas dépasser 500 caractères.")]
        public string Message { get; set; }
        // Contenu de la notification, ≤ 500 caractères
    }
}