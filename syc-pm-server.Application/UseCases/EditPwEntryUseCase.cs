using syc_pm_server.Application.DTO;
using syc_pm_server.Application.Interfaces;
using syc_pm_server.Domain.Entities;

namespace syc_pm_server.Application.UseCases
{
    public class EditPwEntryUseCase
    {
        private readonly IPwEntryRepository _pwEntryRepository;

        public EditPwEntryUseCase(IPwEntryRepository pwEntryRepository)
        {
            _pwEntryRepository = pwEntryRepository;
        }

        public async Task<bool> ExecuteAsync(Guid userId, Guid entryId, CreatePwEntryRequest request)
        {
            var pwEntry = new PwEntry
            {
                Id = entryId,
                Title = request.Title,
                Url = request.Url,
                Username = request.Username,
                EncryptedPassword = request.EncryptedPassword,
                Description = request.Description
            };
            return await _pwEntryRepository.UpdateAsync(pwEntry, userId);
        }
    }
}