using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task AddAsync(User user);
}