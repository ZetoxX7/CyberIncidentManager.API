using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Asset
    {
        public int Id { get; set; }
        // Clé primaire auto‑incrémentée

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        // Nom de l’actif (ex. “Server01”), obligatoire, ≤ 100 caractères

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        // Type d’actif (ex. “Server”, “Switch”), obligatoire, ≤ 50 caractères
        // → À envisager : enum pour contraindre les valeurs autorisées

        [Required]
        [MaxLength(45)]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Adresse IP invalide.")]
        public string IpAddress { get; set; }
        // Adresse IPv4, obligatoire, validée par expression régulière

        [Required]
        [MaxLength(100)]
        public string Owner { get; set; }
        // Responsable de l’actif (nom ou équipe), obligatoire, ≤ 100 caractères

        [MaxLength(100)]
        public string Location { get; set; }
        // Emplacement physique ou logique (ex. “DataCenter A”), optionnel, ≤ 100 caractères

        [Required]
        public string Criticality { get; set; }
        // Niveau de criticité (ex. “Low”, “Medium”, “High”), obligatoire
        // → Là aussi, un enum serait plus fiable que du string libre
    }
}