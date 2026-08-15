using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;

    public DeleteUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Execute(Guid adminId, string username)
    {
        return await _userRepository.DeleteAsync(adminId, username);
    }
}
