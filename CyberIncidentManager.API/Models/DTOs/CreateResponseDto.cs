using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateResponseDto
    {
        [Required(ErrorMessage = "L'identifiant de l'incident est requis.")]
        public int IncidentId { get; set; }

        [Required(ErrorMessage = "L'identifiant utilisateur est requis.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "L'action est requise.")]
        [MaxLength(100, ErrorMessage = "L'action ne doit pas dépasser 100 caractères.")]
        public string Action { get; set; }

        [MaxLength(500, ErrorMessage = "Les détails ne doivent pas dépasser 500 caractères.")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Le statut de réussite est requis.")]
        public bool IsSuccessful { get; set; }
    }
}