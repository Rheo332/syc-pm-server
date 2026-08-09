namespace syc_pm_server.Application.DTO;

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string Pbkdf2Salt { get; set; } = null!;
    public string PublicKey { get; set; } = null!;
    public string EncryptedPrivateKey { get; set; } = null!;
}