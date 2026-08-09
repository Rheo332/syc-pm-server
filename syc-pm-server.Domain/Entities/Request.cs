namespace syc_pm_server.Domain.Entities
{
    public class Request
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Username { get; set; } = null!;

        public string Payload { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}