using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class IncidentType
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        // Nom du type d’incident, obligatoire, ≤ 100 caractères

        [MaxLength(500)]
        public string Description { get; set; }
        // Description optionnelle, ≤ 500 caractères

        [Required]
        public string DefaultSeverity { get; set; }
        // Sévérité par défaut (ex. “Low”, “Medium”, “High”), obligatoire
        // → À envisager : enum pour garantir l’intégrité (`enum Severity { Low, Medium, High }`)

        [Required]
        public string Color { get; set; }
        // Code couleur (ex. “#FF0000”) pour affichage UI, obligatoire
        // → ValidateFormat pour s’assurer du format hexadécimal
    }
}