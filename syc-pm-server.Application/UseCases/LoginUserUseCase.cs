using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Application.Security;

namespace syc_pm_server.Application.UseCases;

public class LoginUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwt;

    public LoginUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwt)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
            return new LoginResponse { Success = false, Message = "Login nicht erfolgreich" };

        var ok = _passwordHasher.Verify(request.Password, user.PasswordSalt, user.PasswordHash);

        if (!ok)
            return new LoginResponse { Success = false, Message = "Login nicht erfolgreich" };

        var token = _jwt.CreateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            PublicKey = user.PublicKey,
            EncryptedPrivateKey = user.EncryptedPrivateKey,
            Token = token,
            Success = true,
            Message = "Login erfolgreich"
        };
    }
}