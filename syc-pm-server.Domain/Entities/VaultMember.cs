namespace syc_pm_server.Domain.Entities
{
    public class VaultMember
    {
        public Guid VaultId { get; set; }
        public Vault Vault { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string EncryptedVaultKey { get; set; } = null!;
        public string Role { get; set; } = "user";
    }
}
