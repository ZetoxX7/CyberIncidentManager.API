using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        public int IncidentId { get; set; }

        [Required(ErrorMessage = "Le titre est requis.")]
        [MaxLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Le message est requis.")]
        [MaxLength(500, ErrorMessage = "Le message ne doit pas dépasser 500 caractères.")]
        public string Message { get; set; }
    }
}