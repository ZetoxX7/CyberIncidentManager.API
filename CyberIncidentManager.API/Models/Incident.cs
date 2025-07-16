using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Incident
    {
        public int Id { get; set; }

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
        public IncidentType Type { get; set; }

        [Required]
        public int ReportedBy { get; set; }
        public User ReportedByUser { get; set; }

        public int? AssignedTo { get; set; }
        public User AssignedToUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public int? AssetId { get; set; }
        public Asset Asset { get; set; }

        public ICollection<Response> Responses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}