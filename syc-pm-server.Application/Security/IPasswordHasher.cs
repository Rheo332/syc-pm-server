namespace syc_pm_server.Application.Security;

public interface IPasswordHasher
{
    string Hash(string password, string salt);
    bool Verify(string password, string salt, string hash);
}