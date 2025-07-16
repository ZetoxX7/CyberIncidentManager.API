using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class CreateIncidentDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string Severity { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int TypeId { get; set; }

        public int? AssignedTo { get; set; }
        public int? AssetId { get; set; }

        [Required]
        public int ReportedBy { get; set; }
    }
}