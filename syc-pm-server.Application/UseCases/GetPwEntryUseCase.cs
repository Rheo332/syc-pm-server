using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class GetPwEntryUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public GetPwEntryUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<PwEntryResponse> Execute(Guid userId)
    {
        var accesses = await _pwEntryRepository.GetUserEntriesAsync(userId);

        var entries = accesses.Select(a => a.PwEntry).ToList();

        return new PwEntryResponse { PwEntries = entries };
    }
}

