using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases;

public class CreatePwEntryUseCase
{
    private readonly IPwEntryRepository _pwEntryRepository;

    public CreatePwEntryUseCase(IPwEntryRepository pwEntryRepository)
    {
        _pwEntryRepository = pwEntryRepository;
    }

    public async Task<bool> Execute(Guid userId, CreatePwEntryRequest request)
    {
        var pwEntry = new Domain.Entities.PwEntry
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Url = request.Url,
            Username = request.Username,
            EncryptedPassword = request.EncryptedPassword,
            Description = request.Description
        };
        return await _pwEntryRepository.CreateAsync(pwEntry, userId);
    }
}

