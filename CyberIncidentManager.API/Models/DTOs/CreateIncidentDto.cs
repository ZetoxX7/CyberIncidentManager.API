using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateIncidentDto
    {
        [Required(ErrorMessage = "Le titre est requis.")]
        [MaxLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La description est requise.")]
        [MaxLength(1000, ErrorMessage = "La description ne doit pas dépasser 1000 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La sévérité est requise.")]
        public string Severity { get; set; }

        [Required(ErrorMessage = "Le statut est requis.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Le type d'incident est requis.")]
        public int TypeId { get; set; }

        public int? AssignedTo { get; set; }
        public int? AssetId { get; set; }

        [Required(ErrorMessage = "L'utilisateur rapporteur est requis.")]
        public int ReportedBy { get; set; }
    }
}