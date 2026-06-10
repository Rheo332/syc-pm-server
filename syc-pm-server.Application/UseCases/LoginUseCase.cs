using syc_pm_server.Application.Security;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases;

public class LoginUseCase
{
    private readonly IPasswordHasher _passwordHasher;

    private readonly User _masterUser;

    public LoginUseCase(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;

        _masterUser = new User
        {
            Username = "admin",
            PasswordHash = _passwordHasher.Hash("1234")
        };
    }

    public LoginResponse Execute(LoginRequest request)
    {
        if (request.Username != _masterUser.Username)
        {
            return new LoginResponse { Success = false, Message = "User nicht gefunden" };
        }

        bool ok = _passwordHasher.Verify(request.Password, _masterUser.PasswordHash);

        return ok
            ? new LoginResponse { Success = true, Message = "Login erfolgreich" }
            : new LoginResponse { Success = false, Message = "Falsches Passwort" };
    }
}