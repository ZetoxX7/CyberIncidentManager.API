namespace CyberIncidentManager.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Incident> ReportedIncidents { get; set; }
        public ICollection<Incident> AssignedIncidents { get; set; }
        public ICollection<Response> Responses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }

}