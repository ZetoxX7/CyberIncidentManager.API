using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class IncidentType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public string DefaultSeverity { get; set; }

        [Required]
        public string Color { get; set; }
    }
}