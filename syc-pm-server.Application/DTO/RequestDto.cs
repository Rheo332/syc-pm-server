namespace syc_pm_server.Application.DTO
{
    public class RequestDto
    {
        public string Type { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }

    public class RequestResponseDto : RequestDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}