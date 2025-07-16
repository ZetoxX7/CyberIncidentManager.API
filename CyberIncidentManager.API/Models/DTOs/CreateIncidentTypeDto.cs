using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.IncidentType
{
    public class CreateIncidentTypeDto
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [MaxLength(250, ErrorMessage = "La description ne doit pas dépasser 250 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La sévérité par défaut est requise.")]
        public string DefaultSeverity { get; set; }

        [Required(ErrorMessage = "La couleur est requise.")]
        public string Color { get; set; }
    }
}