using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [MinLength(8), MaxLength(100)]
        public string? NewPassword { get; set; } // Nullable : uniquement si on veut changer

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}