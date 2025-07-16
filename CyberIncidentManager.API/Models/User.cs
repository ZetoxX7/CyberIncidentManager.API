using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public ICollection<Incident> ReportedIncidents { get; set; }
        public ICollection<Incident> AssignedIncidents { get; set; }
        public ICollection<Response> Responses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}