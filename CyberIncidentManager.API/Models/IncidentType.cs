namespace CyberIncidentManager.API.Models
{
    public class IncidentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultSeverity { get; set; }
        public string Color { get; set; }
    }
}