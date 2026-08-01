namespace syc_pm_server.Domain.Entities
{
    public class PwEntry
    {
        public Guid Id { get; set; }
        public ICollection<PwEntryAccess> AuthorizedUsers { get; set; } = [];
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
