namespace syc_pm_server.Application.DTO
{
    public class CreatePwEntryRequest
    {
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
