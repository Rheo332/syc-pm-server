using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Application.Security;

namespace syc_pm_server.Application.UseCases;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return new LoginResponse { Success = false, Message = "User nicht gefunden" };
        }

        bool ok = _passwordHasher.Verify(request.Password, user.PasswordHash);

        return ok
            ? new LoginResponse { Success = true, Message = "Login erfolgreich" }
            : new LoginResponse { Success = false, Message = "Falsches Passwort" };
    }
}