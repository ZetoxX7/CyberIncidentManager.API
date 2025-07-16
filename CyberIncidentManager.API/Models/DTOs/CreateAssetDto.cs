using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateAssetDto
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le type est requis.")]
        [MaxLength(50, ErrorMessage = "Le type ne doit pas dépasser 50 caractères.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "L'adresse IP est requise.")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Adresse IP invalide.")]
        public string IpAddress { get; set; }

        [Required(ErrorMessage = "Le propriétaire est requis.")]
        [MaxLength(100, ErrorMessage = "Le propriétaire ne doit pas dépasser 100 caractères.")]
        public string Owner { get; set; }

        [MaxLength(100, ErrorMessage = "L'emplacement ne doit pas dépasser 100 caractères.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "La criticité est requise.")]
        public string Criticality { get; set; }
    }
}