namespace syc_pm_server.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string EncryptedPrivateKey { get; set; } = null!;
        public ICollection<PwEntryAccess> PwEntryAccesses { get; set; } = new List<PwEntryAccess>();
    }
}
