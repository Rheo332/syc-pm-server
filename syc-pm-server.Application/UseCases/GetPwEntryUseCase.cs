using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases;

public class GetPwEntryUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public GetPwEntryUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<List<PwEntry>?> Execute()
    {
        return await _pwEntryRepository.GetAllPwEntries();
    }
}

