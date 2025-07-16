using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateAssetDto
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; }
        // Nom de l’actif : obligatoire, ≤ 100 caractères

        [Required(ErrorMessage = "Le type est requis.")]
        [MaxLength(50, ErrorMessage = "Le type ne doit pas dépasser 50 caractères.")]
        public string Type { get; set; }
        // Type d’actif (ex. “Server”, “Switch”) : obligatoire, ≤ 50 caractères
        // → Envisager un enum pour contraindre les valeurs

        [Required(ErrorMessage = "L'adresse IP est requise.")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Adresse IP invalide.")]
        public string IpAddress { get; set; }
        // Adresse IPv4 : obligatoire, validée par regex
        // → Adapter ou étendre la validation pour IPv6 si nécessaire

        [Required(ErrorMessage = "Le propriétaire est requis.")]
        [MaxLength(100, ErrorMessage = "Le propriétaire ne doit pas dépasser 100 caractères.")]
        public string Owner { get; set; }
        // Responsable de l’actif (nom ou équipe) : obligatoire, ≤ 100 caractères

        [MaxLength(100, ErrorMessage = "L'emplacement ne doit pas dépasser 100 caractères.")]
        public string Location { get; set; }
        // Emplacement physique/logique : optionnel, ≤ 100 caractères

        [Required(ErrorMessage = "La criticité est requise.")]
        public string Criticality { get; set; }
        // Niveau de criticité (ex. “Low”, “Medium”, “High”) : obligatoire
        // → Privilégier un enum pour garantir l’intégrité des valeurs
    }
}