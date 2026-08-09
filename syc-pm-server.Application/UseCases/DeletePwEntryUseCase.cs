using syc_pm_server.Application.Interfaces;

namespace syc_pm_server.Application.UseCases
{
    public class DeletePwEntryUseCase
    {
        private readonly IPwEntryRepository _pwEntryRepository;

        public DeletePwEntryUseCase(IPwEntryRepository pwEntryRepository)
        {
            _pwEntryRepository = pwEntryRepository;
        }

        public async Task<bool> ExecuteAsync(Guid userId, Guid entryId)
        {
            return await _pwEntryRepository.DeleteAsync(entryId, userId);
        }
    }
}