namespace CyberIncidentManager.API.Models
{
    public class Response
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }
        public Incident Incident { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
    }
}