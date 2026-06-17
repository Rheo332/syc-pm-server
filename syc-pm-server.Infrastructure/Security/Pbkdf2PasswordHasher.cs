using syc_pm_server.Application.Security;
using System.Security.Cryptography;

namespace syc_pm_server.Infrastructure.Security;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 200000;
    private const int KeySize = 32;

    public string Hash(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        return Convert.ToBase64String(hashBytes);
    }

    public bool Verify(string password, string salt, string hash)
    {
        var computedHash = Hash(password, salt);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(hash)
        );
    }
}