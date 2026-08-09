using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.Interfaces;

public interface IRequestRepository
{
    Task<Request> CreateAsync(Request request);
    Task<List<Request>> GetAllAsync(Guid userId);
    Task<bool> DeleteAsync(Guid userId, Guid id);
    Task<Request?> GetByIdAsync(Guid id);
}
