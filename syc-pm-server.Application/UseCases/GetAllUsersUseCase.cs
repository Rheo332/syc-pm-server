using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases;

public class GetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> Execute(Guid userId)
    {
        return await _userRepository.GetAllUsersAsync(userId);
    }
}
