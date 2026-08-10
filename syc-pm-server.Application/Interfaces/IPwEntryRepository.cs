using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.Interfaces;

public interface IPwEntryRepository
{
    Task<List<PwEntryAccess>> GetUserEntriesAsync(Guid userId);
    Task<bool> CreateAsync(PwEntry pwEntry, Guid userId);
    Task<bool> UpdateAsync(PwEntry pwEntry, Guid userId);
    Task<bool> DeleteAsync(Guid entryId, Guid userId);
    Task<bool> GrantAccessAsync(Guid entryId, Guid adminUserId, Guid targetUserId, string encryptedEntryKey);
    Task<List<Guid>> GetUserAccessAsync(Guid userId);
    Task<bool> RevokeAccessAsync(Guid entryId, Guid adminUserId, Guid targetUserId);
}

