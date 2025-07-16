using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public string Permissions { get; set; }

        public ICollection<User> Users { get; set; }
    }
}