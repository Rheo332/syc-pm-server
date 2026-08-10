using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class GrantAccessUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public GrantAccessUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<bool> ExecuteAsync(Guid entryId, Guid adminUserId, GrantAccessRequest request)
    {
        return await _pwEntryRepository.GrantAccessAsync(entryId, adminUserId, request.TargetUserId, request.EncryptedEntryKey);
    }
}
