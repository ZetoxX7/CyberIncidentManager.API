using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models.Auth
{
    public class AuthRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}