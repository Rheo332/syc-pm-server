namespace syc_pm_server.Domain.Entities
{
    public class PwEntryAccess
    {
        public Guid PwEntryId { get; set; }
        public PwEntry PwEntry { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string EncryptedEntryKey { get; set; } = null!;
    }
}
