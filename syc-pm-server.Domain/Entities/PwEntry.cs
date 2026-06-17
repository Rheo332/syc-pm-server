namespace syc_pm_server.Domain.Entities
{
    public class PwEntry
    {
        public Guid Id { get; set; }
        public Guid VaultId { get; set; }
        public Vault Vault { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
    }
}
