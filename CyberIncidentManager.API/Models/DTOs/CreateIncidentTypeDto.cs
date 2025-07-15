using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs.IncidentType
{
    public class CreateIncidentTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        public string DefaultSeverity { get; set; }

        [Required]
        public string Color { get; set; }
    }
}