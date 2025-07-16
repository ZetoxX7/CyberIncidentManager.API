using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Response
    {
        public int Id { get; set; }

        [Required]
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(500)]
        public string Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }
    }
}