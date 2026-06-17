namespace syc_pm_server.Domain.Entities
{
    public class Vault
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<VaultMember> Members { get; set; } = new List<VaultMember>();
        public ICollection<PwEntry> PasswordEntries { get; set; } = new List<PwEntry>();
    }
}
