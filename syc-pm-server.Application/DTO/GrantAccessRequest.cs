namespace syc_pm_server.Application.DTO
{
    public class GrantAccessRequest
    {
        public Guid TargetUserId { get; set; }
        public string EncryptedEntryKey { get; set; } = null!;
    }
}