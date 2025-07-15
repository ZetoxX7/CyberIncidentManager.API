namespace CyberIncidentManager.API.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public string Owner { get; set; }
        public string Location { get; set; }
        public string Criticality { get; set; }
    }
}