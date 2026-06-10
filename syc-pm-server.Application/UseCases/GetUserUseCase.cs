using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases;

public class GetUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Execute(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }
}

