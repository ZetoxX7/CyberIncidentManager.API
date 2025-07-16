using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.IncidentType
{
    public class CreateIncidentTypeDto
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; }
        // Nom du type d’incident : obligatoire, ≤ 100 caractères

        [MaxLength(250, ErrorMessage = "La description ne doit pas dépasser 250 caractères.")]
        public string Description { get; set; }
        // Description optionnelle : ≤ 250 caractères

        [Required(ErrorMessage = "La sévérité par défaut est requise.")]
        public string DefaultSeverity { get; set; }
        // Sévérité par défaut (ex. “Low”, “Medium”, “High”) : obligatoire
        // → Envisager un enum pour contraindre les valeurs et éviter les fautes

        [Required(ErrorMessage = "La couleur est requise.")]
        public string Color { get; set; }
        // Code couleur hexadécimal (ex. “#FF0000”) : obligatoire
        // → Ajouter une validation par expression régulière pour garantir le format
    }
}