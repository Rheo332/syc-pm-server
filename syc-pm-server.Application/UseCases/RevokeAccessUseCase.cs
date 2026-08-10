using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class RevokeAccessUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public RevokeAccessUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<bool> Execute(Guid entryId, Guid adminUserId, Guid targetUserId)
    {
        return await _pwEntryRepository.RevokeAccessAsync(entryId, adminUserId, targetUserId);
    }
}
