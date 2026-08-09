using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Execute(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return false;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            PasswordSalt = request.PasswordSalt,
            Pbkdf2Salt = request.Pbkdf2Salt,
            PublicKey = request.PublicKey,
            EncryptedPrivateKey = request.EncryptedPrivateKey
        };

        await _userRepository.AddAsync(user);
        return true;
    }
}