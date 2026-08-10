using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class GetUserAccessUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public GetUserAccessUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<List<Guid>> Execute(Guid userId)
    {
        return await _pwEntryRepository.GetUserAccessAsync(userId);
    }
}
