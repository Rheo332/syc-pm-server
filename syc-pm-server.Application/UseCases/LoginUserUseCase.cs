using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using System.Security.Cryptography;

namespace syc_pm_server.Application.UseCases;

public class LoginUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwt;

    public LoginUserUseCase(IUserRepository userRepository, IJwtTokenService jwt)
    {
        _userRepository = userRepository;
        _jwt = jwt;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null)
            return new LoginResponse { Success = false, Message = "Login nicht erfolgreich" };

        var expected = Convert.FromBase64String(user.PasswordHash);
        var provided = Convert.FromBase64String(request.AuthHash);

        if (expected.Length != provided.Length || !CryptographicOperations.FixedTimeEquals(expected, provided))
            return new LoginResponse { Success = false, Message = "Login nicht erfolgreich" };

        var token = _jwt.CreateToken(user);

        return new LoginResponse
        {
            Success = true,
            PublicKey = user.PublicKey,
            EncryptedPrivateKey = user.EncryptedPrivateKey,
            Token = token,
            Message = "Login erfolgreich"
        };
    }
}