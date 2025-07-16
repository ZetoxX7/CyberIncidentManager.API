using System.ComponentModel.DataAnnotations;

namespace CyberIncidentManager.API.Models
{
    public class User
    {
        public int Id { get; set; }
        // Clé primaire auto-incrémentée

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        // Adresse e‑mail unique de l’utilisateur, validée et limitée à 100 caractères

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; }
        // Hash du mot de passe, stocké en clair (hash), max 200 caractères

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        // Prénom, limité à 50 caractères

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        // Nom de famille, limité à 50 caractères

        [Required]
        public int RoleId { get; set; }
        // Clé étrangère vers la table des rôles

        public Role Role { get; set; }
        // Navigation vers l’entité Role (chargement lazy ou eager)

        [Required]
        public DateTime CreatedAt { get; set; }
        // Date de création du compte (à initialiser à DateTime.UtcNow)

        [Required]
        public bool IsActive { get; set; }
        // Indicateur d’activation du compte (désactivation plutôt que suppression)

        public ICollection<Incident> ReportedIncidents { get; set; }
        // Incidents remontés par cet utilisateur

        public ICollection<Incident> AssignedIncidents { get; set; }
        // Incidents dont cet utilisateur est en charge

        public ICollection<Response> Responses { get; set; }
        // Réponses apportées par l’utilisateur à des incidents

        public ICollection<Notification> Notifications { get; set; }
        // Notifications liées à cet utilisateur
    }
}