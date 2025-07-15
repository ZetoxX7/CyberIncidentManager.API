namespace CyberIncidentManager.API.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }

        public int TypeId { get; set; }
        public IncidentType Type { get; set; }

        public int ReportedBy { get; set; }
        public User ReportedByUser { get; set; }

        public int? AssignedTo { get; set; }
        public User AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public int? AssetId { get; set; }
        public Asset Asset { get; set; }

        public ICollection<Response> Responses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}