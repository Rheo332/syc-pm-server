namespace syc_pm_server.Domain.Entities
{
    public class PwEntry
    {
        public Guid Id { get; set; }
        public ICollection<PwEntryAccess> AuthorizedUsers { get; set; } = new List<PwEntryAccess>();
        public string Title { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
    }
}
